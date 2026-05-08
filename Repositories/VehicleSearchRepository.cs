using AutoNext.Platform.Listings.API.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;

namespace AutoNext.Platform.Listings.API.Repositories
{
    public class VehicleSearchRepository : IVehicleSearchRepository
    {
        private readonly IMongoCollection<Vehicle> _vehiclesCollection;
        private readonly ILogger<VehicleSearchRepository>? _logger;

        public VehicleSearchRepository(IMongoDatabase database, ILogger<VehicleSearchRepository>? logger = null)
        {
            _vehiclesCollection = database.GetCollection<Vehicle>("vehicles");
            _logger = logger;
        }

        public async Task<IEnumerable<Vehicle>> AdvancedSearchAsync(VehicleAdvancedSearchCriteria criteria)
        {
            try
            {
                if (criteria == null)
                    throw new ArgumentNullException(nameof(criteria));

                var filters = new List<FilterDefinition<Vehicle>>();

                // Base filters for active listings
                filters.Add(Builders<Vehicle>.Filter.Eq(v => v.IsActive, true));
                filters.Add(Builders<Vehicle>.Filter.Eq(v => v.Status.CurrentStatus, "active"));

                // Advanced search criteria
                if (!string.IsNullOrWhiteSpace(criteria.Make))
                    filters.Add(Builders<Vehicle>.Filter.Eq(v => v.Specifications.Make, criteria.Make));

                if (!string.IsNullOrWhiteSpace(criteria.Model))
                    filters.Add(Builders<Vehicle>.Filter.Eq(v => v.Specifications.Model, criteria.Model));

                if (criteria.MinYear.HasValue)
                    filters.Add(Builders<Vehicle>.Filter.Gte(v => v.Specifications.Year, criteria.MinYear.Value));

                if (criteria.MaxYear.HasValue)
                    filters.Add(Builders<Vehicle>.Filter.Lte(v => v.Specifications.Year, criteria.MaxYear.Value));

                if (criteria.MinPrice.HasValue)
                    filters.Add(Builders<Vehicle>.Filter.Gte(v => v.Price.Raw, criteria.MinPrice.Value));

                if (criteria.MaxPrice.HasValue)
                    filters.Add(Builders<Vehicle>.Filter.Lte(v => v.Price.Raw, criteria.MaxPrice.Value));

                if (!string.IsNullOrWhiteSpace(criteria.FuelType))
                    filters.Add(Builders<Vehicle>.Filter.Eq(v => v.Specifications.FuelType, criteria.FuelType));

                if (!string.IsNullOrWhiteSpace(criteria.Transmission))
                    filters.Add(Builders<Vehicle>.Filter.Eq(v => v.Specifications.Transmission, criteria.Transmission));

                if (criteria.MinKilometers.HasValue)
                    filters.Add(Builders<Vehicle>.Filter.Gte(v => v.Specifications.KilometersDriven, criteria.MinKilometers.Value));

                if (criteria.MaxKilometers.HasValue)
                    filters.Add(Builders<Vehicle>.Filter.Lte(v => v.Specifications.KilometersDriven, criteria.MaxKilometers.Value));

                if (!string.IsNullOrWhiteSpace(criteria.Color))
                    filters.Add(Builders<Vehicle>.Filter.Eq(v => v.Specifications.Color, criteria.Color));

                if (criteria.MinEngineCC.HasValue)
                    filters.Add(Builders<Vehicle>.Filter.Gte(v => v.Specifications.EngineCC, criteria.MinEngineCC.Value));

                if (criteria.MaxEngineCC.HasValue)
                    filters.Add(Builders<Vehicle>.Filter.Lte(v => v.Specifications.EngineCC, criteria.MaxEngineCC.Value));

                if (!string.IsNullOrWhiteSpace(criteria.CityName))
                    filters.Add(Builders<Vehicle>.Filter.ElemMatch(v => v.Locations,
                        loc => loc.CityName == criteria.CityName));

                if (!string.IsNullOrWhiteSpace(criteria.VehicleType))
                    filters.Add(Builders<Vehicle>.Filter.Eq(v => v.VehicleType.Type, criteria.VehicleType));

                if (criteria.IsFeaturedOnly)
                    filters.Add(Builders<Vehicle>.Filter.Eq(v => v.Monetization.IsFeatured, true));

                if (criteria.MinOwnerCount.HasValue)
                    filters.Add(Builders<Vehicle>.Filter.Gte(v => v.Specifications.NoOfOwners, criteria.MinOwnerCount.Value));

                var finalFilter = filters.Any()
                    ? Builders<Vehicle>.Filter.And(filters)
                    : Builders<Vehicle>.Filter.Empty;

                var query = _vehiclesCollection.Find(finalFilter);

                // Apply sorting
                if (!string.IsNullOrWhiteSpace(criteria.SortBy))
                {
                    var sortDefinition = criteria.SortOrder?.ToLower() == "desc"
                        ? Builders<Vehicle>.Sort.Descending(criteria.SortBy)
                        : Builders<Vehicle>.Sort.Ascending(criteria.SortBy);
                    query = query.Sort(sortDefinition);
                }
                else
                {
                    query = query.SortByDescending(v => v.CreatedOn);
                }

                // Apply pagination
                if (criteria.Page.HasValue && criteria.PageSize.HasValue)
                {
                    query = query.Skip((criteria.Page.Value - 1) * criteria.PageSize.Value)
                                 .Limit(criteria.PageSize.Value);
                }

                var vehicles = await query.ToListAsync();
                return vehicles;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in advanced search with criteria: {@Criteria}", criteria);
                throw new Exception($"Error in advanced search: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Vehicle>> SearchByLocationAsync(double lat, double lng, double radiusInKm, int limit = 50)
        {
            try
            {
                var filter = Builders<Vehicle>.Filter.And(
                    Builders<Vehicle>.Filter.Eq(v => v.IsActive, true),
                    Builders<Vehicle>.Filter.Eq(v => v.Status.CurrentStatus, "active"),
                    Builders<Vehicle>.Filter.Near(v => v.Locations[0].Coordinates, lng, lat, radiusInKm * 1000) // Convert km to meters
                );

                var vehicles = await _vehiclesCollection
                    .Find(filter)
                    .Limit(limit)
                    .ToListAsync();

                return vehicles;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error searching by location: Lat={Lat}, Lng={Lng}, Radius={Radius}", lat, lng, radiusInKm);
                throw new Exception($"Error searching by location: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Vehicle>> SearchByKeywordAsync(string keyword, int limit = 50)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    return new List<Vehicle>();
