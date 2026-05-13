using AutoNext.Platform.Listings.API.Configurations;
using AutoNext.Platform.Listings.API.Models.Entities;
using MongoDB.Driver;

namespace AutoNext.Platform.Listings.API.Repositories
{
    public class FeaturedVehicleRepository : IFeaturedVehicleRepository
    {
        private readonly IMongoCollection<FeaturedVehicle> _collection;

        public FeaturedVehicleRepository(MongoDbContext context)
        {
            _collection = context.FeaturedVehicles;
        }

        // ── Basic Queries ───────────────────────────────────────────────────────────

        public async Task<FeaturedVehicle?> GetByIdAsync(string id)
        {
            return await _collection.Find(v => v.Id == id && v.IsActive).FirstOrDefaultAsync();
        }

        public async Task<FeaturedVehicle?> GetBySlugAsync(string slug)
        {
            return await _collection
                .Find(v => v.Slug == slug && v.IsActive)
                .FirstOrDefaultAsync();
        }

        public async Task<FeaturedVehicle?> GetByModelSlugAsync(string modelSlug)
        {
            return await _collection
                .Find(v => v.ModelSlug == modelSlug && v.IsActive)
                .FirstOrDefaultAsync();
        }

        // ── List Queries with Filtering ─────────────────────────────────────────────

        public async Task<IEnumerable<FeaturedVehicle>> GetAllAsync(int page, int pageSize, string? sortBy = null, string? sortOrder = null)
        {
            var filter = Builders<FeaturedVehicle>.Filter.Eq(v => v.IsActive, true);

            var sortDefinition = BuildSortDefinition(sortBy ?? "priority", sortOrder ?? "desc");

            return await _collection
                .Find(filter)
                .Sort(sortDefinition)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<FeaturedVehicle>> GetActiveFeaturedVehiclesAsync(int limit = 50)
        {
            var now = DateTime.UtcNow;

            return await _collection
                .Find(v => v.IsActive &&
                          v.StartDate <= now &&
                          (!v.EndDate.HasValue || v.EndDate.Value >= now) &&
                          v.ListingDetails.IsAvailable)
                .SortBy(v => v.Priority)
                .ThenByDescending(v => v.Rating)
                .Limit(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<FeaturedVehicle>> GetExpiredFeaturedVehiclesAsync()
        {
            var now = DateTime.UtcNow;

            return await _collection
                .Find(v => v.IsActive && v.EndDate.HasValue && v.EndDate.Value < now)
                .ToListAsync();
        }

        // ── Filtered Queries ────────────────────────────────────────────────────────

        public async Task<IEnumerable<FeaturedVehicle>> GetByBrandAsync(string brandName, int limit = 20)
        {
            return await _collection
                .Find(v => v.IsActive && v.BrandName.ToLower() == brandName.ToLower())
                .SortByDescending(v => v.Priority)
                .Limit(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<FeaturedVehicle>> GetByVehicleTypeAsync(string vehicleType, int limit = 20)
        {
            return await _collection
                .Find(v => v.IsActive && v.VehicleType.ToLower() == vehicleType.ToLower())
                .SortByDescending(v => v.Priority)
                .Limit(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<FeaturedVehicle>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice, int limit = 20)
        {
            return await _collection
                .Find(v => v.IsActive &&
                          v.Price.Amount >= minPrice &&
                          v.Price.Amount <= maxPrice)
                .SortBy(v => v.Price.Amount)
                .Limit(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<FeaturedVehicle>> GetByBadgesAsync(List<string> badges, int limit = 20)
        {
            var filter = Builders<FeaturedVehicle>.Filter.And(
                Builders<FeaturedVehicle>.Filter.Eq(v => v.IsActive, true),
                Builders<FeaturedVehicle>.Filter.AnyIn(v => v.Badges, badges)
            );

            return await _collection
                .Find(filter)
                .SortByDescending(v => v.Priority)
                .Limit(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<FeaturedVehicle>> GetByCityAsync(string city, int limit = 20)
        {
            return await _collection
                .Find(v => v.IsActive && v.Location.City.ToLower() == city.ToLower())
                .SortByDescending(v => v.Priority)
                .Limit(limit)
                .ToListAsync();
        }

        // ── Priority-based Queries ──────────────────────────────────────────────────

        public async Task<IEnumerable<FeaturedVehicle>> GetTopPriorityFeaturedVehiclesAsync(int limit = 10)
        {
            var now = DateTime.UtcNow;

            return await _collection
                .Find(v => v.IsActive &&
                          v.StartDate <= now &&
                          (!v.EndDate.HasValue || v.EndDate.Value >= now))
                .SortBy(v => v.Priority)
                .ThenByDescending(v => v.Rating)
                .Limit(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<FeaturedVehicle>> GetFeaturedVehiclesByPriorityRangeAsync(int minPriority, int maxPriority)
        {
            return await _collection
                .Find(v => v.IsActive && v.Priority >= minPriority && v.Priority <= maxPriority)
                .SortBy(v => v.Priority)
                .ToListAsync();
        }

        // ── Date-based Queries ──────────────────────────────────────────────────────

        public async Task<IEnumerable<FeaturedVehicle>> GetActiveOnDateAsync(DateTime date)
        {
            return await _collection
                .Find(v => v.IsActive &&
                          v.StartDate <= date &&
                          (!v.EndDate.HasValue || v.EndDate.Value >= date))
                .SortBy(v => v.Priority)
                .ToListAsync();
        }

        public async Task<IEnumerable<FeaturedVehicle>> GetUpcomingFeaturedVehiclesAsync(int days = 7)
        {
            var startDate = DateTime.UtcNow;
            var endDate = DateTime.UtcNow.AddDays(days);

            return await _collection
                .Find(v => v.IsActive &&
                          v.StartDate >= startDate &&
                          v.StartDate <= endDate)
                .SortBy(v => v.StartDate)
                .ToListAsync();
        }

        // ── Search ──────────────────────────────────────────────────────────────────

        public async Task<IEnumerable<FeaturedVehicle>> SearchAsync(string searchTerm, int limit = 20)
        {
            var filter = Builders<FeaturedVehicle>.Filter.And(
                Builders<FeaturedVehicle>.Filter.Eq(v => v.IsActive, true),
                Builders<FeaturedVehicle>.Filter.Or(
                    Builders<FeaturedVehicle>.Filter.Regex(v => v.Title, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                    Builders<FeaturedVehicle>.Filter.Regex(v => v.BrandName, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                    Builders<FeaturedVehicle>.Filter.Regex(v => v.ModelName, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                    Builders<FeaturedVehicle>.Filter.Regex(v => v.Descriptions, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
                )
            );

            return await _collection
                .Find(filter)
                .SortByDescending(v => v.Priority)
                .Limit(limit)
                .ToListAsync();
        }

        // ── Counts ──────────────────────────────────────────────────────────────────

        public async Task<long> GetTotalCountAsync(bool? isActive = null)
        {
            var filter = isActive.HasValue
                ? Builders<FeaturedVehicle>.Filter.Eq(v => v.IsActive, isActive.Value)
                : Builders<FeaturedVehicle>.Filter.Empty;

            return await _collection.CountDocumentsAsync(filter);
        }

        public async Task<long> GetActiveCountAsync()
        {
            var now = DateTime.UtcNow;
            return await _collection.CountDocumentsAsync(v => v.IsActive &&
                                                           v.StartDate <= now &&
                                                           (!v.EndDate.HasValue || v.EndDate.Value >= now));
        }

        public async Task<long> GetExpiredCountAsync()
        {
            var now = DateTime.UtcNow;
            return await _collection.CountDocumentsAsync(v => v.IsActive && v.EndDate.HasValue && v.EndDate.Value < now);
        }

        // ── Commands ────────────────────────────────────────────────────────────────

        public async Task<FeaturedVehicle> CreateAsync(FeaturedVehicle vehicle)
        {
            vehicle.CreatedAt = DateTime.UtcNow;
            vehicle.UpdatedAt = DateTime.UtcNow;

            // Set default priority if not set
            if (vehicle.Priority == 0)
            {
                var maxPriority = await GetMaxPriorityAsync();
                vehicle.Priority = maxPriority + 1;
            }

            await _collection.InsertOneAsync(vehicle);
            return vehicle;
        }

        public async Task<FeaturedVehicle?> UpdateAsync(string id, FeaturedVehicle vehicle)
        {
            vehicle.UpdatedAt = DateTime.UtcNow;

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

        public async Task<bool> UpdatePriorityAsync(string id, int priority)
        {
            var update = Builders<FeaturedVehicle>.Update
                .Set(v => v.Priority, priority)
                .Set(v => v.UpdatedAt, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(v => v.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> ActivateAsync(string id, DateTime? endDate = null)
        {
            var update = Builders<FeaturedVehicle>.Update
                .Set(v => v.IsActive, true)
                .Set(v => v.StartDate, DateTime.UtcNow)
                .Set(v => v.UpdatedAt, DateTime.UtcNow);

            if (endDate.HasValue)
            {
                update = update.Set(v => v.EndDate, endDate.Value);
            }

            // Also update ListingDetails
            update = update.Set(v => v.ListingDetails.IsAvailable, true);

            var result = await _collection.UpdateOneAsync(v => v.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeactivateAsync(string id)
        {
            var update = Builders<FeaturedVehicle>.Update
                .Set(v => v.IsActive, false)
                .Set(v => v.UpdatedAt, DateTime.UtcNow)
                .Set(v => v.ListingDetails.IsAvailable, false);

            var result = await _collection.UpdateOneAsync(v => v.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> BulkUpdatePriorityAsync(Dictionary<string, int> priorityUpdates)
        {
            var bulkOps = new List<WriteModel<FeaturedVehicle>>();

            foreach (var update in priorityUpdates)
            {
                var filter = Builders<FeaturedVehicle>.Filter.Eq(v => v.Id, update.Key);
                var updateDefinition = Builders<FeaturedVehicle>.Update
                    .Set(v => v.Priority, update.Value)
                    .Set(v => v.UpdatedAt, DateTime.UtcNow);

                bulkOps.Add(new UpdateOneModel<FeaturedVehicle>(filter, updateDefinition));
            }

            var result = await _collection.BulkWriteAsync(bulkOps);
            return result.ModifiedCount == priorityUpdates.Count;
        }

        // ── Engagement Updates ──────────────────────────────────────────────────────

        public async Task<bool> IncrementViewsAsync(string id)
        {
            var update = Builders<FeaturedVehicle>.Update
                .Inc(v => v.Engagement.Views, 1)
                .Set(v => v.UpdatedAt, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(v => v.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> IncrementLikesAsync(string id)
        {
            var update = Builders<FeaturedVehicle>.Update
                .Inc(v => v.Engagement.Likes, 1)
                .Set(v => v.UpdatedAt, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(v => v.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> IncrementSharesAsync(string id)
        {
            var update = Builders<FeaturedVehicle>.Update
                .Inc(v => v.Engagement.Shares, 1)
                .Set(v => v.UpdatedAt, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(v => v.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> IncrementEnquiriesAsync(string id)
        {
            var update = Builders<FeaturedVehicle>.Update
                .Inc(v => v.Engagement.Enquiries, 1)
                .Set(v => v.UpdatedAt, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(v => v.Id == id, update);
            return result.ModifiedCount > 0;
        }

        // ── Rating ──────────────────────────────────────────────────────────────────

        public async Task<bool> AddRatingAsync(string id, UserRating rating)
        {
            var update = Builders<FeaturedVehicle>.Update
                .Push(v => v.UserRatings, rating)
                .Set(v => v.UpdatedAt, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(v => v.Id == id, update);

            // Recalculate average rating
            if (result.ModifiedCount > 0)
            {
                await RecalculateAverageRatingAsync(id);
            }

            return result.ModifiedCount > 0;
        }

        // ── Bulk Operations ─────────────────────────────────────────────────────────

        public async Task<bool> BulkActivateAsync(List<string> ids, DateTime? endDate = null)
        {
            var filter = Builders<FeaturedVehicle>.Filter.In(v => v.Id, ids);
            var update = Builders<FeaturedVehicle>.Update
                .Set(v => v.IsActive, true)
                .Set(v => v.StartDate, DateTime.UtcNow)
                .Set(v => v.UpdatedAt, DateTime.UtcNow);

            if (endDate.HasValue)
            {
                update = update.Set(v => v.EndDate, endDate.Value);
            }

            var result = await _collection.UpdateManyAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> BulkDeactivateAsync(List<string> ids)
        {
            var filter = Builders<FeaturedVehicle>.Filter.In(v => v.Id, ids);
            var update = Builders<FeaturedVehicle>.Update
                .Set(v => v.IsActive, false)
                .Set(v => v.UpdatedAt, DateTime.UtcNow);

            var result = await _collection.UpdateManyAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> BulkDeleteAsync(List<string> ids)
        {
            var filter = Builders<FeaturedVehicle>.Filter.In(v => v.Id, ids);
            var result = await _collection.DeleteManyAsync(filter);
            return result.DeletedCount > 0;
        }

        // ── Private Helpers ─────────────────────────────────────────────────────────

        private static SortDefinition<FeaturedVehicle> BuildSortDefinition(string sortBy, string sortOrder)
        {
            var builder = Builders<FeaturedVehicle>.Sort;
            var ascending = sortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase);

            return sortBy.ToLower() switch
            {
                "priority" => ascending ? builder.Ascending(v => v.Priority) : builder.Descending(v => v.Priority),
                "price" => ascending ? builder.Ascending(v => v.Price.Amount) : builder.Descending(v => v.Price.Amount),
                "rating" => ascending ? builder.Ascending(v => v.Rating) : builder.Descending(v => v.Rating),
                "created_at" => ascending ? builder.Ascending(v => v.CreatedAt) : builder.Descending(v => v.CreatedAt),
                "start_date" => ascending ? builder.Ascending(v => v.StartDate) : builder.Descending(v => v.StartDate),
                "title" => ascending ? builder.Ascending(v => v.Title) : builder.Descending(v => v.Title),
                _ => builder.Ascending(v => v.Priority)
            };
        }

        private async Task<int> GetMaxPriorityAsync()
        {
            var maxPriority = await _collection
                .Find(Builders<FeaturedVehicle>.Filter.Empty)
                .SortByDescending(v => v.Priority)
                .Limit(1)
                .Project(v => v.Priority)
                .FirstOrDefaultAsync();

            return maxPriority;
        }

        private async Task RecalculateAverageRatingAsync(string id)
        {
            var vehicle = await GetByIdAsync(id);
            if (vehicle != null && vehicle.UserRatings.Any())
            {
                var averageRating = vehicle.UserRatings.Average(r => r.Rating);
                var update = Builders<FeaturedVehicle>.Update
                    .Set(v => v.Rating, Math.Round(averageRating, 1))
                    .Set(v => v.UpdatedAt, DateTime.UtcNow);

                await _collection.UpdateOneAsync(v => v.Id == id, update);
            }
        }
    }
}