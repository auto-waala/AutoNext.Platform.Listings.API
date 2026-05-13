using AutoNext.Platform.Listings.API.Models.Entities;

namespace AutoNext.Platform.Listings.API.Repositories
{
    public interface IFeaturedVehicleRepository
    {
        // Basic Queries
        Task<FeaturedVehicle?> GetByIdAsync(string id);
        Task<FeaturedVehicle?> GetBySlugAsync(string slug);
        Task<FeaturedVehicle?> GetByModelSlugAsync(string modelSlug);

        // List Queries with Filtering
        Task<IEnumerable<FeaturedVehicle>> GetAllAsync(int page, int pageSize, string? sortBy = null, string? sortOrder = null);
        Task<IEnumerable<FeaturedVehicle>> GetActiveFeaturedVehiclesAsync(int limit = 50);
        Task<IEnumerable<FeaturedVehicle>> GetExpiredFeaturedVehiclesAsync();

        // Filtered Queries
        Task<IEnumerable<FeaturedVehicle>> GetByBrandAsync(string brandName, int limit = 20);
        Task<IEnumerable<FeaturedVehicle>> GetByVehicleTypeAsync(string vehicleType, int limit = 20);
        Task<IEnumerable<FeaturedVehicle>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice, int limit = 20);
        Task<IEnumerable<FeaturedVehicle>> GetByBadgesAsync(List<string> badges, int limit = 20);
        Task<IEnumerable<FeaturedVehicle>> GetByCityAsync(string city, int limit = 20);

        // Priority-based Queries
        Task<IEnumerable<FeaturedVehicle>> GetTopPriorityFeaturedVehiclesAsync(int limit = 10);
        Task<IEnumerable<FeaturedVehicle>> GetFeaturedVehiclesByPriorityRangeAsync(int minPriority, int maxPriority);

        // Date-based Queries
        Task<IEnumerable<FeaturedVehicle>> GetActiveOnDateAsync(DateTime date);
        Task<IEnumerable<FeaturedVehicle>> GetUpcomingFeaturedVehiclesAsync(int days = 7);

        // Search
        Task<IEnumerable<FeaturedVehicle>> SearchAsync(string searchTerm, int limit = 20);

        // Counts
        Task<long> GetTotalCountAsync(bool? isActive = null);
        Task<long> GetActiveCountAsync();
        Task<long> GetExpiredCountAsync();

        // Commands
        Task<FeaturedVehicle> CreateAsync(FeaturedVehicle vehicle);
        Task<FeaturedVehicle?> UpdateAsync(string id, FeaturedVehicle vehicle);
        Task<bool> DeleteAsync(string id);
        Task<bool> UpdatePriorityAsync(string id, int priority);
        Task<bool> ActivateAsync(string id, DateTime? endDate = null);
        Task<bool> DeactivateAsync(string id);
        Task<bool> BulkUpdatePriorityAsync(Dictionary<string, int> priorityUpdates);

        // Engagement Updates
        Task<bool> IncrementViewsAsync(string id);
        Task<bool> IncrementLikesAsync(string id);
        Task<bool> IncrementSharesAsync(string id);
        Task<bool> IncrementEnquiriesAsync(string id);

        // Rating
        Task<bool> AddRatingAsync(string id, UserRating rating);

        // Bulk Operations
        Task<bool> BulkActivateAsync(List<string> ids, DateTime? endDate = null);
        Task<bool> BulkDeactivateAsync(List<string> ids);
        Task<bool> BulkDeleteAsync(List<string> ids);
    }
}