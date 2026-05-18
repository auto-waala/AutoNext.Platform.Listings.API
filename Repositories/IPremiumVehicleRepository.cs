using AutoNext.Platform.Listings.API.Models.Entities;

namespace AutoNext.Platform.Listings.API.Repositories
{
    public interface IPremiumVehicleRepository
    {
        // Basic Queries
        Task<PremiumVehicle?> GetByIdAsync(string id);
        Task<PremiumVehicle?> GetBySlugAsync(string slug);
        Task<PremiumVehicle?> GetByModelSlugAsync(string modelSlug);

        // List Queries with Filtering
        Task<IEnumerable<PremiumVehicle>> GetAllAsync(int page, int pageSize, string? sortBy = null, string? sortOrder = null);
        Task<IEnumerable<PremiumVehicle>> GetActivePremiumVehiclesAsync(int limit = 50);
        Task<IEnumerable<PremiumVehicle>> GetExpiredPremiumVehiclesAsync();

        // Filtered Queries
        Task<IEnumerable<PremiumVehicle>> GetByBrandAsync(string brandName, int limit = 20);
        Task<IEnumerable<PremiumVehicle>> GetByVehicleTypeAsync(string vehicleType, int limit = 20);
        Task<IEnumerable<PremiumVehicle>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice, int limit = 20);
        Task<IEnumerable<PremiumVehicle>> GetByBadgesAsync(List<string> badges, int limit = 20);
        Task<IEnumerable<PremiumVehicle>> GetByCityAsync(string city, int limit = 20);

        // Priority-based Queries
        Task<IEnumerable<PremiumVehicle>> GetTopPriorityPremiumVehiclesAsync(int limit = 10);
        Task<IEnumerable<PremiumVehicle>> GetPremiumVehiclesByPriorityRangeAsync(int minPriority, int maxPriority);

        // Date-based Queries
        Task<IEnumerable<PremiumVehicle>> GetActiveOnDateAsync(DateTime date);
        Task<IEnumerable<PremiumVehicle>> GetUpcomingPremiumVehiclesAsync(int days = 7);

        // Search
        Task<IEnumerable<PremiumVehicle>> SearchAsync(string searchTerm, int limit = 20);

        // Counts
        Task<long> GetTotalCountAsync(bool? isActive = null);
        Task<long> GetActiveCountAsync();
        Task<long> GetExpiredCountAsync();

        // Commands
        Task<PremiumVehicle> CreateAsync(PremiumVehicle vehicle);
        Task<PremiumVehicle?> UpdateAsync(string id, PremiumVehicle vehicle);
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