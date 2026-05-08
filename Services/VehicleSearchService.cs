using AutoNext.Platform.Listings.API.Models;
using AutoNext.Platform.Listings.API.Repositories;
using Microsoft.Extensions.Logging;
using MongoDB.Driver.Search;
using System.Text.Json;

namespace AutoNext.Platform.Listings.API.Services
{
    public class VehicleSearchService : IVehicleSearchService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IVehicleSearchRepository _vehicleSearchRepository;
        private readonly ILogger<VehicleSearchService> _logger;

        public VehicleSearchService(
            IVehicleRepository vehicleRepository,
            IVehicleSearchRepository vehicleSearchRepository,
            ILogger<VehicleSearchService> logger)
        {
            _vehicleRepository = vehicleRepository;
            _vehicleSearchRepository = vehicleSearchRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Vehicle>> SearchByKeywordAsync(string keyword, SearchOptions? options = null)
        {
            try
            {
                var limit = options?.Limit ?? 50;
                return await _vehicleSearchRepository.SearchByKeywordAsync(keyword, limit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching by keyword: {Keyword}", keyword);
                throw;
            }
        }

        public async Task<IEnumerable<Vehicle>> SearchByFiltersAsync(VehicleSearchCriteria criteria, SearchOptions? options = null)
        {
            try
            {
                return await _vehicleRepository.SearchVehiclesAsync(criteria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching by filters: {@Criteria}", criteria);
                throw;
            }
        }

        public async Task<IEnumerable<Vehicle>> AdvancedSearchAsync(VehicleAdvancedSearchCriteria criteria)
        {
            try
            {
                return await _vehicleSearchRepository.AdvancedSearchAsync(criteria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in advanced search: {@Criteria}", criteria);
                throw;
            }
        }

        public async Task<SearchResult<Vehicle>> PaginatedSearchAsync(VehicleSearchCriteria criteria, int page = 1, int pageSize = 20)
        {
            try
            {
                var advancedCriteria = new VehicleAdvancedSearchCriteria
                {
                    Make = criteria.Make,
                    Model = criteria.Model,
                    MinYear = criteria.MinYear,
                    MaxYear = criteria.MaxYear,
                    MinPrice = criteria.MinPrice,
                    MaxPrice = criteria.MaxPrice,
                    FuelType = criteria.FuelType,
                    Transmission = criteria.Transmission,
                    CityName = criteria.CityName,
                    VehicleType = criteria.VehicleType,
                    MaxKilometers = criteria.MaxKilometers,
                    Color = criteria.Color,
                    Page = page,
                    PageSize = pageSize
                };

                var vehicles = await _vehicleSearchRepository.AdvancedSearchAsync(advancedCriteria);
                var totalCount = await _vehicleRepository.GetTotalCountAsync();

                return new SearchResult<Vehicle>
                {
                    Items = vehicles,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in paginated search");
                throw;
            }
        }

        public async Task<IEnumerable<Vehicle>> SearchNearbyAsync(double lat, double lng, double radiusInKm, SearchOptions? options = null)
        {
            try
            {
                var limit = options?.Limit ?? 50;
                return await _vehicleSearchRepository.SearchByLocationAsync(lat, lng, radiusInKm, limit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching nearby: Lat={Lat}, Lng={Lng}, Radius={Radius}", lat, lng, radiusInKm);
                throw;
            }
        }

        public async Task<IEnumerable<Vehicle>> SearchByCityAsync(string cityName, SearchOptions? options = null)
        {
            try
            {
                var criteria = new VehicleSearchCriteria { CityName = cityName };
                return await _vehicleRepository.SearchVehiclesAsync(criteria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching by city: {CityName}", cityName);
                throw;
            }
        }

        public async Task<IEnumerable<Vehicle>> SearchByStateAsync(string stateName, SearchOptions? options = null)
        {
            // Implementation would need state field in location
            // For now, return empty
            return await Task.FromResult(new List<Vehicle>());
        }

        public async Task<IEnumerable<Vehicle>> SearchByMakeAndModelAsync(string make, string model, int? year = null)
        {
            try
            {
                return await _vehicleSearchRepository.SearchByMakeAndModelAsync(make, model, year ?? 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching by make and model: Make={Make}, Model={Model}", make, model);
                throw;
            }
        }

        public async Task<IEnumerable<Vehicle>> SearchByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            try
            {
                return await _vehicleRepository.GetVehiclesByPriceRangeAsync(minPrice, maxPrice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching by price range: {MinPrice} - {MaxPrice}", minPrice, maxPrice);
                throw;
            }
        }

        public async Task<IEnumerable<Vehicle>> SearchByYearRangeAsync(int minYear, int maxYear)
        {
            try
            {
                var criteria = new VehicleSearchCriteria { MinYear = minYear, MaxYear = maxYear };
                return await _vehicleRepository.SearchVehiclesAsync(criteria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching by year range: {MinYear} - {MaxYear}", minYear, maxYear);
                throw;
            }
        }

        public async Task<IEnumerable<Vehicle>> SearchByFuelTypeAsync(string fuelType)
        {
            try
            {
                var criteria = new VehicleSearchCriteria { FuelType = fuelType };
                return await _vehicleRepository.SearchVehiclesAsync(criteria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching by fuel type: {FuelType}", fuelType);
                throw;
            }
        }

        public async Task<IEnumerable<Vehicle>> SearchByTransmissionTypeAsync(string transmission)
        {
            try
            {
                var criteria = new VehicleSearchCriteria { Transmission = transmission };
                return await _vehicleRepository.SearchVehiclesAsync(criteria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching by transmission: {Transmission}", transmission);
                throw;
            }
        }

        public async Task<IEnumerable<Vehicle>> SearchByColorAsync(string color)
        {
            try
            {
                var criteria = new VehicleSearchCriteria { Color = color };
                return await _vehicleRepository.SearchVehiclesAsync(criteria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching by color: {Color}", color);
                throw;
            }
        }

        public async Task<FacetedSearchResult> GetFacetedSearchResultsAsync(VehicleSearchCriteria criteria)
        {
            try
            {
                var facets = await _vehicleSearchRepository.GetFacetedSearchCountsAsync(criteria);
                var vehicles = await _vehicleRepository.SearchVehiclesAsync(criteria);

                return new FacetedSearchResult
                {
                    Vehicles = vehicles,
                    Facets = facets.Select(f => new FacetCount { Name = f.Key, Count = f.Value }).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting faceted search results");
                throw;
            }
        }

        public async Task<Dictionary<string, List<FacetCount>>> GetSearchFacetsAsync(VehicleSearchCriteria criteria)
        {
            // This would typically aggregate multiple facet types
            var result = new Dictionary<string, List<FacetCount>>();

            // Get make facets
            var makeCounts = await _vehicleRepository.GetVehicleCountByMakeAsync();
            result["makes"] = makeCounts.Select(m => new FacetCount { Name = m.Key, Count = (int)m.Value }).ToList();

            return result;
        }

        public async Task<IEnumerable<Vehicle>> GetPersonalizedRecommendationsAsync(string userId, int limit = 20)
        {
            try
            {
                return await _vehicleSearchRepository.GetRecommendationsAsync(userId, limit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting personalized recommendations for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<Vehicle>> GetSimilarVehiclesAsync(string vehicleId, int limit = 5)
        {
            try
            {
                return await _vehicleRepository.GetSimilarVehiclesAsync(vehicleId, limit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting similar vehicles for: {VehicleId}", vehicleId);
                throw;
            }
        }

        public async Task<IEnumerable<Vehicle>> GetTrendingVehiclesAsync(int limit = 10)
        {
            try
            {
                // Get vehicles with most views in last 7 days
                var allVehicles = await _vehicleRepository.GetActiveVehiclesAsync(1, 100);
                return allVehicles.OrderByDescending(v => v.Analytics.Views).Take(limit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting trending vehicles");
                throw;
            }
        }

        public async Task SaveSearchHistoryAsync(string userId, VehicleSearchCriteria criteria)
        {
            // Implementation would save to a separate search history collection
            _logger.LogInformation("Saved search for user {UserId}: {@Criteria}", userId, criteria);
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<SavedSearch>> GetUserSavedSearchesAsync(string userId)
        {
            // Implementation would retrieve from saved searches collection
            return await Task.FromResult(new List<SavedSearch>());
        }

        public async Task<bool> DeleteSavedSearchAsync(string userId, string searchId)
        {
            // Implementation would delete from saved searches collection
            return await Task.FromResult(true);
        }

        public async Task<IEnumerable<string>> GetSearchSuggestionsAsync(string prefix, int limit = 10)
        {
            try
            {
                // Get suggestions from popular makes, models, and keywords
                var suggestions = new List<string>();
                var makes = await _vehicleRepository.GetVehicleCountByMakeAsync();

                suggestions.AddRange(makes.Keys.Where(m => m.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)));

                return suggestions.Take(limit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting search suggestions for prefix: {Prefix}", prefix);
                return new List<string>();
            }
        }

        public async Task<IEnumerable<Vehicle>> GetRecentlyViewedVehiclesAsync(string userId, int limit = 10)
        {
            try
            {
                return await _vehicleSearchRepository.GetRecentlyViewedAsync(userId, limit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recently viewed vehicles for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<VehicleComparison> CompareVehiclesAsync(List<string> vehicleIds)
        {
            try
            {
                var vehicles = await _vehicleSearchRepository.GetVehiclesForComparisonAsync(vehicleIds);

                return new VehicleComparison
                {
                    Vehicles = vehicles.ToList(),
                    ComparisonDate = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error comparing vehicles: {@VehicleIds}", vehicleIds);
                throw;
            }
        }

        public async Task<byte[]> ExportSearchResultsAsync(VehicleSearchCriteria criteria, ExportFormat format = ExportFormat.CSV)
        {
            var vehicles = await _vehicleRepository.SearchVehiclesAsync(criteria);

            return format switch
            {
                ExportFormat.CSV => GenerateCsvExport(vehicles),
                ExportFormat.JSON => GenerateJsonExport(vehicles),
                _ => GenerateCsvExport(vehicles)
            };
        }

        private byte[] GenerateCsvExport(IEnumerable<Vehicle> vehicles)
        {
            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Id,Title,Make,Model,Year,Price,Status");

            foreach (var vehicle in vehicles)
            {
                csv.AppendLine($"{vehicle.Id},{vehicle.Title},{vehicle.Specifications.Make},{vehicle.Specifications.Model},{vehicle.Specifications.Year},{vehicle.Price.Raw},{vehicle.Status.CurrentStatus}");
            }

            return System.Text.Encoding.UTF8.GetBytes(csv.ToString());
        }

        private byte[] GenerateJsonExport(IEnumerable<Vehicle> vehicles)
        {
            var json = JsonSerializer.Serialize(vehicles, new JsonSerializerOptions { WriteIndented = true });
            return System.Text.Encoding.UTF8.GetBytes(json);
        }
    }
}