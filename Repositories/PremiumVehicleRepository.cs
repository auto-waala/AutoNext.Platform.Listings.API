using AutoNext.Platform.Listings.API.Configurations;
using AutoNext.Platform.Listings.API.Models.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AutoNext.Platform.Listings.API.Repositories
{
    public class PremiumVehicleRepository : IPremiumVehicleRepository
    {
        private readonly IMongoCollection<PremiumVehicle> _collection;

        public PremiumVehicleRepository(MongoDbContext context)
        {
            _collection = context.PremiumVehicles;
        }

        // Basic Queries

        public async Task<PremiumVehicle?> GetByIdAsync(string id)
        {
            return await _collection.Find(v => v.Id == id && v.IsActive).FirstOrDefaultAsync();
        }

        public async Task<PremiumVehicle?> GetBySlugAsync(string slug)
        {
            return await _collection
                .Find(v => v.Slug == slug && v.IsActive)
                .FirstOrDefaultAsync();
        }

        public async Task<PremiumVehicle?> GetByModelSlugAsync(string modelSlug)
        {
            return await _collection
                .Find(v => v.ModelSlug == modelSlug && v.IsActive)
                .FirstOrDefaultAsync();
        }

        // List Queries with Filtering

        public async Task<IEnumerable<PremiumVehicle>> GetAllAsync(int page, int pageSize, string? sortBy = null, string? sortOrder = null)
        {
            var filter = Builders<PremiumVehicle>.Filter.Eq(v => v.IsActive, true);

            var sortDefinition = BuildSortDefinition(sortBy ?? "priority", sortOrder ?? "desc");

            return await _collection
                .Find(filter)
                .Sort(sortDefinition)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<PremiumVehicle>> GetActivePremiumVehiclesAsync(int limit = 50)
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

        public async Task<IEnumerable<PremiumVehicle>> GetExpiredPremiumVehiclesAsync()
        {
            var now = DateTime.UtcNow;

            return await _collection
                .Find(v => v.IsActive && v.EndDate.HasValue && v.EndDate.Value < now)
                .ToListAsync();
        }

        // Filtered Queries

        public async Task<IEnumerable<PremiumVehicle>> GetByBrandAsync(string brandName, int limit = 20)
        {
            return await _collection
                .Find(v => v.IsActive && v.BrandName.ToLower() == brandName.ToLower())
                .SortByDescending(v => v.Priority)
                .Limit(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<PremiumVehicle>> GetByVehicleTypeAsync(string vehicleType, int limit = 20)
        {
            return await _collection
                .Find(v => v.IsActive && v.VehicleType.ToLower() == vehicleType.ToLower())
                .SortByDescending(v => v.Priority)
                .Limit(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<PremiumVehicle>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice, int limit = 20)
        {
            return await _collection
                .Find(v => v.IsActive &&
                          v.Price.Amount >= minPrice &&
                          v.Price.Amount <= maxPrice)
                .SortBy(v => v.Price.Amount)
                .Limit(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<PremiumVehicle>> GetByBadgesAsync(List<string> badges, int limit = 20)
        {
            var filter = Builders<PremiumVehicle>.Filter.And(
                Builders<PremiumVehicle>.Filter.Eq(v => v.IsActive, true),
                Builders<PremiumVehicle>.Filter.AnyIn(v => v.Badges, badges)
            );

            return await _collection
                .Find(filter)
                .SortByDescending(v => v.Priority)
                .Limit(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<PremiumVehicle>> GetByCityAsync(string city, int limit = 20)
        {
            return await _collection
                .Find(v => v.IsActive && v.Location.City.ToLower() == city.ToLower())
                .SortByDescending(v => v.Priority)
                .Limit(limit)
                .ToListAsync();
        }

        // Priority-based Queries

        public async Task<IEnumerable<PremiumVehicle>> GetTopPriorityPremiumVehiclesAsync(int limit = 10)
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

        public async Task<IEnumerable<PremiumVehicle>> GetPremiumVehiclesByPriorityRangeAsync(int minPriority, int maxPriority)
        {
            return await _collection
                .Find(v => v.IsActive && v.Priority >= minPriority && v.Priority <= maxPriority)
                .SortBy(v => v.Priority)
                .ToListAsync();
        }

        // Date-based Queries

        public async Task<IEnumerable<PremiumVehicle>> GetActiveOnDateAsync(DateTime date)
        {
            return await _collection
                .Find(v => v.IsActive &&
                          v.StartDate <= date &&
                          (!v.EndDate.HasValue || v.EndDate.Value >= date))
                .SortBy(v => v.Priority)
                .ToListAsync();
        }

        public async Task<IEnumerable<PremiumVehicle>> GetUpcomingPremiumVehiclesAsync(int days = 7)
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

        // Search

        public async Task<IEnumerable<PremiumVehicle>> SearchAsync(string searchTerm, int limit = 20)
        {
            var filter = Builders<PremiumVehicle>.Filter.And(
                Builders<PremiumVehicle>.Filter.Eq(v => v.IsActive, true),
                Builders<PremiumVehicle>.Filter.Or(
                    Builders<PremiumVehicle>.Filter.Regex(v => v.Title, new BsonRegularExpression(searchTerm, "i")),
                    Builders<PremiumVehicle>.Filter.Regex(v => v.BrandName, new BsonRegularExpression(searchTerm, "i")),
                    Builders<PremiumVehicle>.Filter.Regex(v => v.ModelName, new BsonRegularExpression(searchTerm, "i")),
                    Builders<PremiumVehicle>.Filter.Regex(v => v.Descriptions, new BsonRegularExpression(searchTerm, "i"))
                )
            );

            return await _collection
                .Find(filter)
                .SortByDescending(v => v.Priority)
                .Limit(limit)
                .ToListAsync();
        }

        // Counts

        public async Task<long> GetTotalCountAsync(bool? isActive = null)
        {
            var filter = isActive.HasValue
                ? Builders<PremiumVehicle>.Filter.Eq(v => v.IsActive, isActive.Value)
                : Builders<PremiumVehicle>.Filter.Empty;

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

        // Commands

        public async Task<PremiumVehicle> CreateAsync(PremiumVehicle vehicle)
        {
            vehicle.CreatedAt = DateTime.UtcNow;
            vehicle.UpdatedAt = DateTime.UtcNow;

            if (vehicle.Priority == 0)
            {
                var maxPriority = await GetMaxPriorityAsync();
                vehicle.Priority = maxPriority + 1;
            }

            await _collection.InsertOneAsync(vehicle);
            return vehicle;
        }

        public async Task<PremiumVehicle?> UpdateAsync(string id, PremiumVehicle vehicle)
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
            var update = Builders<PremiumVehicle>.Update
                .Set(v => v.Priority, priority)
                .Set(v => v.UpdatedAt, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(v => v.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> ActivateAsync(string id, DateTime? endDate = null)
        {
            var update = Builders<PremiumVehicle>.Update
                .Set(v => v.IsActive, true)
                .Set(v => v.StartDate, DateTime.UtcNow)
                .Set(v => v.UpdatedAt, DateTime.UtcNow)
                .Set(v => v.ListingDetails.IsAvailable, true);

            if (endDate.HasValue)
            {
                update = update.Set(v => v.EndDate, endDate.Value);
            }

            var result = await _collection.UpdateOneAsync(v => v.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeactivateAsync(string id)
        {
            var update = Builders<PremiumVehicle>.Update
                .Set(v => v.IsActive, false)
                .Set(v => v.UpdatedAt, DateTime.UtcNow)
                .Set(v => v.ListingDetails.IsAvailable, false);

            var result = await _collection.UpdateOneAsync(v => v.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> BulkUpdatePriorityAsync(Dictionary<string, int> priorityUpdates)
        {
            var bulkOps = new List<WriteModel<PremiumVehicle>>();

            foreach (var updateItem in priorityUpdates)
            {
                var filter = Builders<PremiumVehicle>.Filter.Eq(v => v.Id, updateItem.Key);
                var updateDefinition = Builders<PremiumVehicle>.Update
                    .Set(v => v.Priority, updateItem.Value)
                    .Set(v => v.UpdatedAt, DateTime.UtcNow);

                bulkOps.Add(new UpdateOneModel<PremiumVehicle>(filter, updateDefinition));
            }

            if (bulkOps.Count == 0) return false;

            var result = await _collection.BulkWriteAsync(bulkOps);
            return result.ModifiedCount == priorityUpdates.Count;
        }

        // Engagement Updates

        public async Task<bool> IncrementViewsAsync(string id)
        {
            var update = Builders<PremiumVehicle>.Update
                .Inc(v => v.Engagement.Views, 1)
                .Set(v => v.UpdatedAt, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(v => v.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> IncrementLikesAsync(string id)
        {
            var update = Builders<PremiumVehicle>.Update
                .Inc(v => v.Engagement.Likes, 1)
                .Set(v => v.UpdatedAt, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(v => v.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> IncrementSharesAsync(string id)
        {
            var update = Builders<PremiumVehicle>.Update
                .Inc(v => v.Engagement.Shares, 1)
                .Set(v => v.UpdatedAt, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(v => v.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> IncrementEnquiriesAsync(string id)
        {
            var update = Builders<PremiumVehicle>.Update
                .Inc(v => v.Engagement.Enquiries, 1)
                .Set(v => v.UpdatedAt, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(v => v.Id == id, update);
            return result.ModifiedCount > 0;
        }

        // Rating

        public async Task<bool> AddRatingAsync(string id, UserRating rating)
        {
            var update = Builders<PremiumVehicle>.Update
                .Push(v => v.UserRatings, rating)
                .Set(v => v.UpdatedAt, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(v => v.Id == id, update);

            if (result.ModifiedCount > 0)
            {
                await RecalculateAverageRatingAsync(id);
            }

            return result.ModifiedCount > 0;
        }

        // Bulk Operations

        public async Task<bool> BulkActivateAsync(List<string> ids, DateTime? endDate = null)
        {
            var filter = Builders<PremiumVehicle>.Filter.In(v => v.Id, ids);
            var update = Builders<PremiumVehicle>.Update
                .Set(v => v.IsActive, true)
                .Set(v => v.StartDate, DateTime.UtcNow)
                .Set(v => v.UpdatedAt, DateTime.UtcNow)
                .Set(v => v.ListingDetails.IsAvailable, true);

            if (endDate.HasValue)
            {
                update = update.Set(v => v.EndDate, endDate.Value);
            }

            var result = await _collection.UpdateManyAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> BulkDeactivateAsync(List<string> ids)
        {
            var filter = Builders<PremiumVehicle>.Filter.In(v => v.Id, ids);
            var update = Builders<PremiumVehicle>.Update
                .Set(v => v.IsActive, false)
                .Set(v => v.UpdatedAt, DateTime.UtcNow)
                .Set(v => v.ListingDetails.IsAvailable, false);

            var result = await _collection.UpdateManyAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> BulkDeleteAsync(List<string> ids)
        {
            var filter = Builders<PremiumVehicle>.Filter.In(v => v.Id, ids);
            var result = await _collection.DeleteManyAsync(filter);
            return result.DeletedCount > 0;
        }

        // Private Helpers

        private static SortDefinition<PremiumVehicle> BuildSortDefinition(string sortBy, string sortOrder)
        {
            var builder = Builders<PremiumVehicle>.Sort;
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
                .Find(Builders<PremiumVehicle>.Filter.Empty)
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
                var update = Builders<PremiumVehicle>.Update
                    .Set(v => v.Rating, Math.Round(averageRating, 1))
                    .Set(v => v.UpdatedAt, DateTime.UtcNow);

                await _collection.UpdateOneAsync(v => v.Id == id, update);
            }
        }
    }
}
