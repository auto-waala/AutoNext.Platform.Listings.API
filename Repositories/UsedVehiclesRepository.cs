using AutoNext.Platform.Listings.API.Configurations;
using AutoNext.Platform.Listings.API.Models.Common;
using AutoNext.Platform.Listings.API.Models.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Servers;

namespace AutoNext.Platform.Listings.API.Repositories
{
    public class UsedVehiclesRepository : IUsedVehiclesRepository
    {
        private readonly IMongoCollection<UsedVehicles> _collection;

        public UsedVehiclesRepository(MongoDbContext context)
        {
            _collection = context.UsedVehicles;
        }

        // ── Basic Queries ───────────────────────────────────────────────────────────

        public async Task<UsedVehicles?> GetByIdAsync(string id)
        {
            return await _collection.Find(v => v.Id == id && v.IsActive).FirstOrDefaultAsync();
        }

        public async Task<UsedVehicles?> GetBySlugAsync(string slug)
        {
            return await _collection
                .Find(v => v.Slug == slug && v.IsActive)
                .FirstOrDefaultAsync();
        }

        public async Task<UsedVehicles?> GetByModelSlugAsync(string modelSlug)
        {
            return await _collection
                .Find(v => v.ModelSlug == modelSlug && v.IsActive)
                .FirstOrDefaultAsync();
        }

        // ── List Queries with Filtering ─────────────────────────────────────────────

        public async Task<IEnumerable<UsedVehicles>> GetAllAsync(int page, int pageSize, string? sortBy = null, string? sortOrder = null)
        {
            var filter = Builders<UsedVehicles>.Filter.Eq(v => v.IsActive, true);

            var sortDefinition = BuildSortDefinition(sortBy ?? "priority", sortOrder ?? "desc");

            return await _collection
                .Find(filter)
                .Sort(sortDefinition)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<UsedVehicles>> GetActiveUsedVehiclesAsync(int limit = 50)
        {
            var now = DateTime.UtcNow;

            return await _collection
                .Find(v => v.IsActive &&
                          v.StartDate <= now &&
                          (!v.EndDate.HasValue || v.EndDate.Value >= now) &&
                          v.ListingDetails.IsAvailable &&
                          !v.ListingDetails.IsSold)
                .SortBy(v => v.Priority)
                .ThenByDescending(v => v.Rating)
                .Limit(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<UsedVehicles>> GetExpiredUsedVehiclesAsync()
        {
            var now = DateTime.UtcNow;

            return await _collection
                .Find(v => v.IsActive && v.EndDate.HasValue && v.EndDate.Value < now)
                .ToListAsync();
        }

        // ── Filtered Queries ────────────────────────────────────────────────────────

        public async Task<IEnumerable<UsedVehicles>> GetByBrandAsync(string brandName, int limit = 20)
        {
            return await _collection
                .Find(v => v.IsActive && v.BrandName.ToLower() == brandName.ToLower() && !v.ListingDetails.IsSold)
                .SortByDescending(v => v.Priority)
                .Limit(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<UsedVehicles>> GetByVehicleTypeAsync(string vehicleType, int limit = 20)
        {
            return await _collection
                .Find(v => v.IsActive && v.VehicleType.ToLower() == vehicleType.ToLower() && !v.ListingDetails.IsSold)
                .SortByDescending(v => v.Priority)
                .Limit(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<UsedVehicles>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice, int limit = 20)
        {
            return await _collection
                .Find(v => v.IsActive &&
                          v.Price.Amount >= minPrice &&
                          v.Price.Amount <= maxPrice &&
                          !v.ListingDetails.IsSold)
                .SortBy(v => v.Price.Amount)
                .Limit(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<UsedVehicles>> GetByCityAsync(string city, int limit = 20)
        {
            return await _collection
                .Find(v => v.IsActive && v.Location.City.ToLower() == city.ToLower() && !v.ListingDetails.IsSold)
                .SortByDescending(v => v.Priority)
                .Limit(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<UsedVehicles>> GetByConditionAsync(bool isNew, int kmDrivenMax, int limit = 20)
        {
            return await _collection
                .Find(v => v.IsActive &&
                          v.Condition.IsNew == isNew &&
                          v.Condition.KMDriven <= kmDrivenMax &&
                          !v.ListingDetails.IsSold)
                .SortByDescending(v => v.Priority)
                .Limit(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<UsedVehicles>> GetByFuelTypeAsync(string fuelType, int limit = 20)
        {
            return await _collection
                .Find(v => v.IsActive &&
                          v.KeySpecifications.FuelType.ToLower() == fuelType.ToLower() &&
                          !v.ListingDetails.IsSold)
                .SortByDescending(v => v.Priority)
                .Limit(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<UsedVehicles>> GetByTransmissionAsync(string transmission, int limit = 20)
        {
            return await _collection
                .Find(v => v.IsActive &&
                          v.KeySpecifications.Transmission.ToLower() == transmission.ToLower() &&
                          !v.ListingDetails.IsSold)
                .SortByDescending(v => v.Priority)
                .Limit(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<UsedVehicles>> GetByYearRangeAsync(int minYear, int maxYear, int limit = 20)
        {
            return await _collection
                .Find(v => v.IsActive &&
                          v.Condition.RegistrationYear >= minYear &&
                          v.Condition.RegistrationYear <= maxYear &&
                          !v.ListingDetails.IsSold)
                .SortByDescending(v => v.Condition.RegistrationYear)
                .Limit(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<UsedVehicles>> GetBySellerTypeAsync(string sellerType, int limit = 20)
        {
            return await _collection
                .Find(v => v.IsActive &&
                          v.Seller.SellerType.ToLower() == sellerType.ToLower() &&
                          !v.ListingDetails.IsSold)
                .SortByDescending(v => v.Priority)
                .Limit(limit)
                .ToListAsync();
        }

        // ── Priority-based Queries ──────────────────────────────────────────────────

        public async Task<IEnumerable<UsedVehicles>> GetTopPriorityUsedVehiclesAsync(int limit = 10)
        {
            var now = DateTime.UtcNow;

            return await _collection
                .Find(v => v.IsActive &&
                          v.StartDate <= now &&
                          (!v.EndDate.HasValue || v.EndDate.Value >= now) &&
                          !v.ListingDetails.IsSold)
                .SortBy(v => v.Priority)
                .ThenByDescending(v => v.Rating)
                .Limit(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<UsedVehicles>> GetUsedVehiclesByPriorityRangeAsync(int minPriority, int maxPriority)
        {
            return await _collection
                .Find(v => v.IsActive && v.Priority >= minPriority && v.Priority <= maxPriority && !v.ListingDetails.IsSold)
                .SortBy(v => v.Priority)
                .ToListAsync();
        }

        // ── Date-based Queries ──────────────────────────────────────────────────────

        public async Task<IEnumerable<UsedVehicles>> GetActiveOnDateAsync(DateTime date)
        {
            return await _collection
                .Find(v => v.IsActive &&
                          v.StartDate <= date &&
                          (!v.EndDate.HasValue || v.EndDate.Value >= date) &&
                          !v.ListingDetails.IsSold)
                .SortBy(v => v.Priority)
                .ToListAsync();
        }

        public async Task<IEnumerable<UsedVehicles>> GetRecentlyPostedAsync(int days = 7, int limit = 20)
        {
            var fromDate = DateTime.UtcNow.AddDays(-days);

            return await _collection
                .Find(v => v.ListingDetails.PostedDate >= fromDate && v.IsActive && !v.ListingDetails.IsSold)
                .SortByDescending(v => v.ListingDetails.PostedDate)
                .Limit(limit)
                .ToListAsync();
        }

        // ── Search ──────────────────────────────────────────────────────────────────

        public async Task<IEnumerable<UsedVehicles>> SearchAsync(string searchTerm, int limit = 20)
        {
            var filter = Builders<UsedVehicles>.Filter.And(
                Builders<UsedVehicles>.Filter.Eq(v => v.IsActive, true),
                Builders<UsedVehicles>.Filter.Eq(v => v.ListingDetails.IsSold, false),
                Builders<UsedVehicles>.Filter.Or(
                    Builders<UsedVehicles>.Filter.Regex(v => v.Title, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                    Builders<UsedVehicles>.Filter.Regex(v => v.BrandName, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                    Builders<UsedVehicles>.Filter.Regex(v => v.ModelName, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                    Builders<UsedVehicles>.Filter.Regex(v => v.Descriptions, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
                )
            );

            return await _collection
                .Find(filter)
                .SortByDescending(v => v.Priority)
                .Limit(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<UsedVehicles>> AdvancedSearchAsync(UsedVehiclesSearchCriteria criteria)
        {
            var filters = new List<FilterDefinition<UsedVehicles>>();

            // Base filters
            filters.Add(Builders<UsedVehicles>.Filter.Eq(v => v.IsActive, true));
            filters.Add(Builders<UsedVehicles>.Filter.Eq(v => v.ListingDetails.IsSold, false));

            // Apply search criteria
            if (!string.IsNullOrEmpty(criteria.BrandName))
                filters.Add(Builders<UsedVehicles>.Filter.Regex(v => v.BrandName, new MongoDB.Bson.BsonRegularExpression(criteria.BrandName, "i")));

            if (!string.IsNullOrEmpty(criteria.ModelName))
                filters.Add(Builders<UsedVehicles>.Filter.Regex(v => v.ModelName, new MongoDB.Bson.BsonRegularExpression(criteria.ModelName, "i")));

            if (!string.IsNullOrEmpty(criteria.VehicleType))
                filters.Add(Builders<UsedVehicles>.Filter.Eq(v => v.VehicleType, criteria.VehicleType));

            if (!string.IsNullOrEmpty(criteria.BodyType))
                filters.Add(Builders<UsedVehicles>.Filter.Eq(v => v.BodyType, criteria.BodyType));

            if (criteria.MinPrice.HasValue)
                filters.Add(Builders<UsedVehicles>.Filter.Gte(v => v.Price.Amount, criteria.MinPrice.Value));

            if (criteria.MaxPrice.HasValue)
                filters.Add(Builders<UsedVehicles>.Filter.Lte(v => v.Price.Amount, criteria.MaxPrice.Value));

            if (!string.IsNullOrEmpty(criteria.City))
                filters.Add(Builders<UsedVehicles>.Filter.Regex(v => v.Location.City, new MongoDB.Bson.BsonRegularExpression(criteria.City, "i")));

            if (!string.IsNullOrEmpty(criteria.State))
                filters.Add(Builders<UsedVehicles>.Filter.Regex(v => v.Location.State, new MongoDB.Bson.BsonRegularExpression(criteria.State, "i")));

            if (criteria.MinYear.HasValue)
                filters.Add(Builders<UsedVehicles>.Filter.Gte(v => v.Condition.RegistrationYear, criteria.MinYear.Value));

            if (criteria.MaxYear.HasValue)
                filters.Add(Builders<UsedVehicles>.Filter.Lte(v => v.Condition.RegistrationYear, criteria.MaxYear.Value));

            if (criteria.MaxKmDriven.HasValue)
                filters.Add(Builders<UsedVehicles>.Filter.Lte(v => v.Condition.KMDriven, criteria.MaxKmDriven.Value));

            if (!string.IsNullOrEmpty(criteria.FuelType))
                filters.Add(Builders<UsedVehicles>.Filter.Eq(v => v.KeySpecifications.FuelType, criteria.FuelType));

            if (!string.IsNullOrEmpty(criteria.Transmission))
                filters.Add(Builders<UsedVehicles>.Filter.Eq(v => v.KeySpecifications.Transmission, criteria.Transmission));

            if (criteria.MinOwnerCount.HasValue)
                filters.Add(Builders<UsedVehicles>.Filter.Gte(v => v.Condition.OwnerCount, criteria.MinOwnerCount.Value));

            if (criteria.MaxOwnerCount.HasValue)
                filters.Add(Builders<UsedVehicles>.Filter.Lte(v => v.Condition.OwnerCount, criteria.MaxOwnerCount.Value));

            if (criteria.IsAccidental.HasValue)
                filters.Add(Builders<UsedVehicles>.Filter.Eq(v => v.Condition.Accidental, criteria.IsAccidental.Value));

            if (criteria.IsVerified.HasValue)
                filters.Add(Builders<UsedVehicles>.Filter.Eq(v => v.ListingDetails.IsVerified, criteria.IsVerified.Value));

            if (!string.IsNullOrEmpty(criteria.SellerType))
                filters.Add(Builders<UsedVehicles>.Filter.Eq(v => v.Seller.SellerType, criteria.SellerType));

            if (criteria.MinRating.HasValue)
                filters.Add(Builders<UsedVehicles>.Filter.Gte(v => v.Rating, criteria.MinRating.Value));

            var finalFilter = Builders<UsedVehicles>.Filter.And(filters);
            var sortDefinition = BuildSortDefinition(criteria.SortBy ?? "priority", criteria.SortOrder ?? "desc");

            return await _collection
                .Find(finalFilter)
                .Sort(sortDefinition)
                .Skip((criteria.Page - 1) * criteria.PageSize)
                .Limit(criteria.PageSize)
                .ToListAsync();
        }

        // ── Counts ──────────────────────────────────────────────────────────────────

        public async Task<long> GetTotalCountAsync(bool? isActive = null)
        {
            var filter = isActive.HasValue
                ? Builders<UsedVehicles>.Filter.Eq(v => v.IsActive, isActive.Value)
                : Builders<UsedVehicles>.Filter.Empty;

            return await _collection.CountDocumentsAsync(filter);
        }

        public async Task<long> GetActiveCountAsync()
        {
            var now = DateTime.UtcNow;
            return await _collection.CountDocumentsAsync(v => v.IsActive &&
                                                           v.StartDate <= now &&
                                                           (!v.EndDate.HasValue || v.EndDate.Value >= now) &&
                                                           !v.ListingDetails.IsSold);
        }

        public async Task<long> GetExpiredCountAsync()
        {
            var now = DateTime.UtcNow;
            return await _collection.CountDocumentsAsync(v => v.IsActive && v.EndDate.HasValue && v.EndDate.Value < now);
        }

        public async Task<Dictionary<string, long>> GetCountByBrandAsync()
        {
            var pipeline = new[]
            {
                new BsonDocument("$match", new BsonDocument("IsActive", true)),
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "$BrandName" },
                    { "Count", new BsonDocument("$sum", 1) }
                }),
                new BsonDocument("$sort", new BsonDocument("Count", -1))
            };

            var results = await _collection.AggregateAsync<BsonDocument>(pipeline);
            var dict = new Dictionary<string, long>();

            await results.ForEachAsync(doc =>
            {
                dict.Add(doc["_id"].AsString, doc["Count"].AsInt64);
            });

            return dict;
        }

        public async Task<Dictionary<string, long>> GetCountByCityAsync()
        {
            var pipeline = new[]
            {
                new BsonDocument("$match", new BsonDocument("IsActive", true)),
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "$Location.City" },
                    { "Count", new BsonDocument("$sum", 1) }
                }),
                new BsonDocument("$sort", new BsonDocument("Count", -1))
            };

            var results = await _collection.AggregateAsync<BsonDocument>(pipeline);
            var dict = new Dictionary<string, long>();

            await results.ForEachAsync(doc =>
            {
                var city = doc["_id"].AsString;
                if (!string.IsNullOrEmpty(city))
                    dict.Add(city, doc["Count"].AsInt64);
            });

            return dict;
        }

        // ── Commands ────────────────────────────────────────────────────────────────

        public async Task<UsedVehicles> CreateAsync(UsedVehicles vehicle)
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

        public async Task<UsedVehicles?> UpdateAsync(string id, UsedVehicles vehicle)
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
            var update = Builders<UsedVehicles>.Update
                .Set(v => v.Priority, priority)
                .Set(v => v.UpdatedAt, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(v => v.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> ActivateAsync(string id, DateTime? endDate = null)
        {
            var update = Builders<UsedVehicles>.Update
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
            var update = Builders<UsedVehicles>.Update
                .Set(v => v.IsActive, false)
                .Set(v => v.UpdatedAt, DateTime.UtcNow)
                .Set(v => v.ListingDetails.IsAvailable, false);

            var result = await _collection.UpdateOneAsync(v => v.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> MarkAsSoldAsync(string id)
        {
            var update = Builders<UsedVehicles>.Update
                .Set(v => v.ListingDetails.IsSold, true)
                .Set(v => v.ListingDetails.IsAvailable, false)
                .Set(v => v.IsActive, false)
                .Set(v => v.UpdatedAt, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(v => v.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> BulkUpdatePriorityAsync(Dictionary<string, int> priorityUpdates)
        {
            var bulkOps = new List<WriteModel<UsedVehicles>>();

            foreach (var update in priorityUpdates)
            {
                var filter = Builders<UsedVehicles>.Filter.Eq(v => v.Id, update.Key);
                var updateDefinition = Builders<UsedVehicles>.Update
                    .Set(v => v.Priority, update.Value)
                    .Set(v => v.UpdatedAt, DateTime.UtcNow);

                bulkOps.Add(new UpdateOneModel<UsedVehicles>(filter, updateDefinition));
            }

            var result = await _collection.BulkWriteAsync(bulkOps);
            return result.ModifiedCount == priorityUpdates.Count;
        }

        // ── Engagement Updates ──────────────────────────────────────────────────────

        public async Task<bool> IncrementViewsAsync(string id)
        {
            var update = Builders<UsedVehicles>.Update
                .Inc(v => v.Engagement.Views, 1)
                .Set(v => v.UpdatedAt, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(v => v.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> IncrementLikesAsync(string id)
        {
            var update = Builders<UsedVehicles>.Update
                .Inc(v => v.Engagement.Likes, 1)
                .Set(v => v.UpdatedAt, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(v => v.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> IncrementSharesAsync(string id)
        {
            var update = Builders<UsedVehicles>.Update
                .Inc(v => v.Engagement.Shares, 1)
                .Set(v => v.UpdatedAt, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(v => v.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> IncrementEnquiriesAsync(string id)
        {
            var update = Builders<UsedVehicles>.Update
                .Inc(v => v.Engagement.Enquiries, 1)
                .Set(v => v.UpdatedAt, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(v => v.Id == id, update);
            return result.ModifiedCount > 0;
        }

        // ── Rating ──────────────────────────────────────────────────────────────────

        public async Task<bool> AddRatingAsync(string id, UserRating rating)
        {
            var update = Builders<UsedVehicles>.Update
                .Push(v => v.UserRatings, rating)
                .Set(v => v.UpdatedAt, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(v => v.Id == id, update);

            if (result.ModifiedCount > 0)
            {
                await RecalculateAverageRatingAsync(id);
            }

            return result.ModifiedCount > 0;
        }

        // ── Bulk Operations ─────────────────────────────────────────────────────────

        public async Task<bool> BulkActivateAsync(List<string> ids, DateTime? endDate = null)
        {
            var filter = Builders<UsedVehicles>.Filter.In(v => v.Id, ids);
            var update = Builders<UsedVehicles>.Update
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
            var filter = Builders<UsedVehicles>.Filter.In(v => v.Id, ids);
            var update = Builders<UsedVehicles>.Update
                .Set(v => v.IsActive, false)
                .Set(v => v.UpdatedAt, DateTime.UtcNow);

            var result = await _collection.UpdateManyAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> BulkDeleteAsync(List<string> ids)
        {
            var filter = Builders<UsedVehicles>.Filter.In(v => v.Id, ids);
            var result = await _collection.DeleteManyAsync(filter);
            return result.DeletedCount > 0;
        }

        public async Task<bool> BulkMarkAsSoldAsync(List<string> ids)
        {
            var filter = Builders<UsedVehicles>.Filter.In(v => v.Id, ids);
            var update = Builders<UsedVehicles>.Update
                .Set(v => v.ListingDetails.IsSold, true)
                .Set(v => v.ListingDetails.IsAvailable, false)
                .Set(v => v.IsActive, false)
                .Set(v => v.UpdatedAt, DateTime.UtcNow);

            var result = await _collection.UpdateManyAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        // ── Private Helpers ─────────────────────────────────────────────────────────

        private static SortDefinition<UsedVehicles> BuildSortDefinition(string sortBy, string sortOrder)
        {
            var builder = Builders<UsedVehicles>.Sort;
            var ascending = sortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase);

            return sortBy.ToLower() switch
            {
                "priority" => ascending ? builder.Ascending(v => v.Priority) : builder.Descending(v => v.Priority),
                "price" => ascending ? builder.Ascending(v => v.Price.Amount) : builder.Descending(v => v.Price.Amount),
                "rating" => ascending ? builder.Ascending(v => v.Rating) : builder.Descending(v => v.Rating),
                "created_at" => ascending ? builder.Ascending(v => v.CreatedAt) : builder.Descending(v => v.CreatedAt),
                "start_date" => ascending ? builder.Ascending(v => v.StartDate) : builder.Descending(v => v.StartDate),
                "title" => ascending ? builder.Ascending(v => v.Title) : builder.Descending(v => v.Title),
                "year" => ascending ? builder.Ascending(v => v.Condition.RegistrationYear) : builder.Descending(v => v.Condition.RegistrationYear),
                "km_driven" => ascending ? builder.Ascending(v => v.Condition.KMDriven) : builder.Descending(v => v.Condition.KMDriven),
                "posted_date" => ascending ? builder.Ascending(v => v.ListingDetails.PostedDate) : builder.Descending(v => v.ListingDetails.PostedDate),
                _ => builder.Ascending(v => v.Priority)
            };
        }

        private async Task<int> GetMaxPriorityAsync()
        {
            var maxPriority = await _collection
                .Find(Builders<UsedVehicles>.Filter.Empty)
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
                var update = Builders<UsedVehicles>.Update
                    .Set(v => v.Rating, Math.Round(averageRating, 1))
                    .Set(v => v.UpdatedAt, DateTime.UtcNow);

                await _collection.UpdateOneAsync(v => v.Id == id, update);
            }
        }

        public async Task<IEnumerable<UsedVehicles>> GetBySellerAsync(string sellerId, int limit = 20)
        {
            return await _collection.Find(v => v.Seller.SellerId.ToLower() == sellerId).SortByDescending(v => v.Priority).Limit(limit).ToListAsync();
        }
    }
}