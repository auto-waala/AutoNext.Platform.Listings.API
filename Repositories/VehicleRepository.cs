using AutoNext.Platform.Listings.API.Configurations;
using AutoNext.Platform.Listings.API.Models.DTOs;
using AutoNext.Platform.Listings.API.Models.Entities;
using MongoDB.Driver;

namespace AutoNext.Platform.Listings.API.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly IMongoCollection<Vehicle> _collection;

        public VehicleRepository(MongoDbContext context)
        {
            _collection = context.Vehicles;
        }

        // ── Queries ───────────────────────────────────────────────────────────

        public async Task<Vehicle?> GetByIdAsync(string id)
        {
            return await _collection
                .Find(v => v.Id == id && v.IsActive)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetAllAsync(int page, int pageSize)
        {
            return await _collection
                .Find(v => v.IsActive)
                .SortByDescending(v => v.CreatedOn)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Vehicle>> SearchAsync(VehicleSearchRequest request)
        {
            var filter = BuildSearchFilter(request);

            var sort = BuildSortDefinition(request.SortBy, request.SortOrder);

            return await _collection
                .Find(filter)
                .Sort(sort)
                .Skip((request.Page - 1) * request.PageSize)
                .Limit(request.PageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetBySellerAsync(string sellerId, int page, int pageSize)
        {
            return await _collection
                .Find(v => v.Seller.SellerId == sellerId && v.IsActive)
                .SortByDescending(v => v.CreatedOn)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        // ── Counts ────────────────────────────────────────────────────────────

        public async Task<long> GetTotalCountAsync()
        {
            return await _collection.CountDocumentsAsync(v => v.IsActive);
        }

        public async Task<long> GetSearchCountAsync(VehicleSearchRequest request)
        {
            var filter = BuildSearchFilter(request);
            return await _collection.CountDocumentsAsync(filter);
        }

        public async Task<long> GetSellerCountAsync(string sellerId)
        {
            return await _collection
                .CountDocumentsAsync(v => v.Seller.SellerId == sellerId && v.IsActive);
        }

        // ── Commands ──────────────────────────────────────────────────────────

        public async Task<Vehicle> CreateAsync(Vehicle vehicle)
        {
            await _collection.InsertOneAsync(vehicle);
            return vehicle;
        }

        public async Task<Vehicle?> UpdateAsync(string id, Vehicle vehicle)
        {
            var result = await _collection.ReplaceOneAsync(
                v => v.Id == id,
                vehicle,
                new ReplaceOptions { IsUpsert = false });

            return result.ModifiedCount > 0 ? vehicle : null;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _collection.DeleteOneAsync(v => v.Id == id);
            return result.DeletedCount > 0;
        }

        // Atomic increment — no read-modify-write round trip
        public async Task IncrementViewsAsync(string id)
        {
            var update = Builders<Vehicle>.Update
                .Inc(v => v.Views, 1)
                .Set(v => v.ModifiedOn, DateTime.UtcNow);

            await _collection.UpdateOneAsync(v => v.Id == id, update);
        }

        // ── Private helpers ───────────────────────────────────────────────────

        private static FilterDefinition<Vehicle> BuildSearchFilter(VehicleSearchRequest request)
        {
            var builder = Builders<Vehicle>.Filter;
            var filter = builder.Eq(v => v.IsActive, true);

            if (!string.IsNullOrWhiteSpace(request.VehicleType))
                filter &= builder.Eq(v => v.VehicleType, request.VehicleType);

            if (!string.IsNullOrWhiteSpace(request.BodyType))
                filter &= builder.Eq(v => v.BodyType, request.BodyType);

            if (!string.IsNullOrWhiteSpace(request.Make))
                filter &= builder.Regex(v => v.Make, new MongoDB.Bson.BsonRegularExpression(request.Make, "i"));

            if (!string.IsNullOrWhiteSpace(request.Model))
                filter &= builder.Regex(v => v.Model, new MongoDB.Bson.BsonRegularExpression(request.Model, "i"));

            if (!string.IsNullOrWhiteSpace(request.FuelType))
                filter &= builder.Eq(v => v.FuelType, request.FuelType);

            if (!string.IsNullOrWhiteSpace(request.Transmission))
                filter &= builder.Eq(v => v.Transmission, request.Transmission);

            if (!string.IsNullOrWhiteSpace(request.Color))
                filter &= builder.Eq(v => v.Color, request.Color);

            if (!string.IsNullOrWhiteSpace(request.City))
                filter &= builder.Regex(v => v.City, new MongoDB.Bson.BsonRegularExpression(request.City, "i"));

            if (!string.IsNullOrWhiteSpace(request.Locality))
                filter &= builder.Regex(v => v.Locality, new MongoDB.Bson.BsonRegularExpression(request.Locality, "i"));

            if (!string.IsNullOrWhiteSpace(request.SellerType))
                filter &= builder.Eq(v => v.Seller.SellerType, request.SellerType);

            if (request.MinYear.HasValue)
                filter &= builder.Gte(v => v.Year, request.MinYear.Value);

            if (request.MaxYear.HasValue)
                filter &= builder.Lte(v => v.Year, request.MaxYear.Value);

            if (request.MinPrice.HasValue)
                filter &= builder.Gte(v => v.Price.Raw, request.MinPrice.Value);

            if (request.MaxPrice.HasValue)
                filter &= builder.Lte(v => v.Price.Raw, request.MaxPrice.Value);

            if (request.MinKilometers.HasValue)
                filter &= builder.Gte(v => v.Kilometers, request.MinKilometers.Value);

            if (request.MaxKilometers.HasValue)
                filter &= builder.Lte(v => v.Kilometers, request.MaxKilometers.Value);

            return filter;
        }

        private static SortDefinition<Vehicle> BuildSortDefinition(string sortBy, string sortOrder)
        {
            var builder = Builders<Vehicle>.Sort;
            var ascending = sortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase);

            return sortBy.ToLower() switch
            {
                "price" => ascending ? builder.Ascending(v => v.Price.Raw) : builder.Descending(v => v.Price.Raw),
                "year" => ascending ? builder.Ascending(v => v.Year) : builder.Descending(v => v.Year),
                "kilometers" => ascending ? builder.Ascending(v => v.Kilometers) : builder.Descending(v => v.Kilometers),
                "views" => ascending ? builder.Ascending(v => v.Views) : builder.Descending(v => v.Views),
                _ => ascending ? builder.Ascending(v => v.CreatedOn) : builder.Descending(v => v.CreatedOn)
            };
        }
    }
}
