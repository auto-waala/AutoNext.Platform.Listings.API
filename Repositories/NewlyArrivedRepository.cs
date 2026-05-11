using AutoNext.Platform.Listings.API.Configurations;
using AutoNext.Platform.Listings.API.Models.Entities;
using MongoDB.Driver;

namespace AutoNext.Platform.Listings.API.Repositories
{
    public class NewlyArrivedRepository : INewlyArrivedRepository
    {
        private readonly IMongoCollection<NewlyArrivedVehicle> _collection;

        public NewlyArrivedRepository(MongoDbContext context)
        {
            _collection = context.NewlyArrivedVehicles;
        }

        // ── Queries ───────────────────────────────────────────────────────────

        public async Task<NewlyArrivedVehicle?> GetByIdAsync(string id)
        {
            return await _collection
                .Find(v => v.Id == id && v.IsActive && v.Status)
                .FirstOrDefaultAsync();
        }

        public async Task<NewlyArrivedVehicle?> GetByModelSlugAsync(string modelSlug)
        {
            return await _collection
                .Find(v => v.ModelSlug == modelSlug && v.IsActive && v.Status)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<NewlyArrivedVehicle>> GetAllAsync(int page, int pageSize)
        {
            return await _collection
                .Find(v => v.IsActive && v.Status)
                .SortByDescending(v => v.ArrivalDate)
                .ThenBy(v => v.DisplayOrder)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        // ── Period-specific queries ──────────────────────────────────────────

        public async Task<IEnumerable<NewlyArrivedVehicle>> GetByArrivalPeriodAsync(string period, int limit = 20)
        {
            return await _collection
                .Find(v => v.IsActive && v.Status && v.ArrivalPeriod == period && v.ExpiryDate >= DateTime.UtcNow)
                .SortByDescending(v => v.ArrivalDate)
                .Limit(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<NewlyArrivedVehicle>> GetWeeklyArrivalsAsync(int weekNumber, int year)
        {
            var (startDate, endDate) = GetWeekRange(weekNumber, year);

            return await _collection
                .Find(v => v.IsActive && v.Status &&
                           v.ArrivalDate >= startDate && v.ArrivalDate <= endDate)
                .SortByDescending(v => v.ArrivalDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<NewlyArrivedVehicle>> GetMonthlyArrivalsAsync(int month, int year)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            return await _collection
                .Find(v => v.IsActive && v.Status &&
                           v.ArrivalDate >= startDate && v.ArrivalDate <= endDate)
                .SortByDescending(v => v.ArrivalDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<NewlyArrivedVehicle>> GetYearlyArrivalsAsync(int year)
        {
            var startDate = new DateTime(year, 1, 1);
            var endDate = new DateTime(year, 12, 31);

            return await _collection
                .Find(v => v.IsActive && v.Status &&
                           v.ArrivalDate >= startDate && v.ArrivalDate <= endDate)
                .SortByDescending(v => v.ArrivalDate)
                .ToListAsync();
        }

        // ── Featured ──────────────────────────────────────────────────────────

        public async Task<IEnumerable<NewlyArrivedVehicle>> GetFeaturedArrivalsAsync(int limit = 10)
        {
            return await _collection
                .Find(v => v.IsActive && v.Status && v.Featured && v.ExpiryDate >= DateTime.UtcNow)
                .SortBy(v => v.DisplayOrder)
                .ThenByDescending(v => v.ArrivalDate)
                .Limit(limit)
                .ToListAsync();
        }

        // ── Counts ────────────────────────────────────────────────────────────

        public async Task<long> GetTotalCountAsync()
        {
            return await _collection.CountDocumentsAsync(v => v.IsActive && v.Status);
        }

       

        public async Task<NewlyArrivedVehicle> CreateAsync(NewlyArrivedVehicle vehicle)
        {
            vehicle.CreatedAt = DateTime.UtcNow;
            vehicle.UpdatedAt = DateTime.UtcNow;
            vehicle.Status = true;
            vehicle.IsActive = true;

            // Set expiry date based on arrival period
            vehicle.ExpiryDate = CalculateExpiryDate(vehicle.ArrivalDate, vehicle.ArrivalPeriod);

            await _collection.InsertOneAsync(vehicle);
            return vehicle;
        }

        public async Task<NewlyArrivedVehicle?> UpdateAsync(string id, NewlyArrivedVehicle vehicle)
        {
            vehicle.UpdatedAt = DateTime.UtcNow;
            vehicle.ExpiryDate = CalculateExpiryDate(vehicle.ArrivalDate, vehicle.ArrivalPeriod);

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

        public async Task<bool> PublishAsync(string id, string publishedBy)
        {
            var update = Builders<NewlyArrivedVehicle>.Update
                .Set(v => v.IsActive, true)
                .Set(v => v.Status, true)
                .Set(v => v.PublishedBy, publishedBy)
                .Set(v => v.UpdatedAt, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(v => v.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> UnpublishAsync(string id)
        {
            var update = Builders<NewlyArrivedVehicle>.Update
                .Set(v => v.IsActive, false)
                .Set(v => v.Status, false)
                .Set(v => v.UpdatedAt, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(v => v.Id == id, update);
            return result.ModifiedCount > 0;
        }

        // ── Private helpers ───────────────────────────────────────────────────


        private static SortDefinition<NewlyArrivedVehicle> BuildSortDefinition(string sortBy, string sortOrder)
        {
            var builder = Builders<NewlyArrivedVehicle>.Sort;
            var ascending = sortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase);

            return sortBy.ToLower() switch
            {
                "price" => ascending ? builder.Ascending(v => v.MinPrice) : builder.Descending(v => v.MinPrice),
                "arrival_date" => ascending ? builder.Ascending(v => v.ArrivalDate) : builder.Descending(v => v.ArrivalDate),
                "display_order" => ascending ? builder.Ascending(v => v.DisplayOrder) : builder.Descending(v => v.DisplayOrder),
                "rating" => ascending ? builder.Ascending(v => v.Rating) : builder.Descending(v => v.Rating),
                _ => ascending ? builder.Ascending(v => v.ArrivalDate) : builder.Descending(v => v.ArrivalDate)
            };
        }

        private static (DateTime startDate, DateTime endDate) GetWeekRange(int weekNumber, int year)
        {
            var jan1 = new DateTime(year, 1, 1);
            var daysOffset = (int)jan1.DayOfWeek - (int)DayOfWeek.Monday;
            var firstMonday = jan1.AddDays(-daysOffset);
            var startDate = firstMonday.AddDays((weekNumber - 1) * 7);
            var endDate = startDate.AddDays(6);
            return (startDate, endDate);
        }

        private static DateTime? CalculateExpiryDate(DateTime arrivalDate, string arrivalPeriod)
        {
            return arrivalPeriod switch
            {
                "weekly" => arrivalDate.AddDays(7),
                "monthly" => arrivalDate.AddMonths(1),
                "yearly" => arrivalDate.AddYears(1),
                _ => arrivalDate.AddDays(30)
            };
        }
    }
}
