using AutoNext.Platform.Listings.API.Models.Common;
using AutoNext.Platform.Listings.API.Models.Entities;

namespace AutoNext.Platform.Listings.API.Repositories
{
    public interface IUsedVehiclesRepository
    {
        Task<UsedVehicles?> GetByIdAsync(string id);
        Task<UsedVehicles?> GetBySlugAsync(string slug);
        Task<UsedVehicles?> GetByModelSlugAsync(string modelSlug);

        // List Queries with Filtering
        Task<IEnumerable<UsedVehicles>> GetAllAsync(int page, int pageSize, string? sortBy = null, string? sortOrder = null);
        Task<IEnumerable<UsedVehicles>> GetActiveUsedVehiclesAsync(int limit = 50);
        Task<IEnumerable<UsedVehicles>> GetExpiredUsedVehiclesAsync();

        // Filtered Queries
        Task<IEnumerable<UsedVehicles>> GetByBrandAsync(string brandName, int limit = 20);
        Task<IEnumerable<UsedVehicles>> GetByVehicleTypeAsync(string vehicleType, int limit = 20);
        Task<IEnumerable<UsedVehicles>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice, int limit = 20);
        Task<IEnumerable<UsedVehicles>> GetByCityAsync(string city, int limit = 20);
        Task<IEnumerable<UsedVehicles>> GetByConditionAsync(bool isNew, int kmDrivenMax, int limit = 20);
        Task<IEnumerable<UsedVehicles>> GetByFuelTypeAsync(string fuelType, int limit = 20);
        Task<IEnumerable<UsedVehicles>> GetByTransmissionAsync(string transmission, int limit = 20);
        Task<IEnumerable<UsedVehicles>> GetByYearRangeAsync(int minYear, int maxYear, int limit = 20);
        Task<IEnumerable<UsedVehicles>> GetBySellerTypeAsync(string sellerType, int limit = 20);
        Task<IEnumerable<UsedVehicles>> GetBySellerAsync(string sellerId, int limit = 20);

        // Priority-based Queries
        Task<IEnumerable<UsedVehicles>> GetTopPriorityUsedVehiclesAsync(int limit = 10);
        Task<IEnumerable<UsedVehicles>> GetUsedVehiclesByPriorityRangeAsync(int minPriority, int maxPriority);

        // Date-based Queries
        Task<IEnumerable<UsedVehicles>> GetActiveOnDateAsync(DateTime date);
        Task<IEnumerable<UsedVehicles>> GetRecentlyPostedAsync(int days = 7, int limit = 20);

        // Search
        Task<IEnumerable<UsedVehicles>> SearchAsync(string searchTerm, int limit = 20);
        Task<IEnumerable<UsedVehicles>> AdvancedSearchAsync(UsedVehiclesSearchCriteria criteria);

        // Counts
        Task<long> GetTotalCountAsync(bool? isActive = null);
        Task<long> GetActiveCountAsync();
        Task<long> GetExpiredCountAsync();
        Task<Dictionary<string, long>> GetCountByBrandAsync();
        Task<Dictionary<string, long>> GetCountByCityAsync();

        // Commands
        Task<UsedVehicles> CreateAsync(UsedVehicles vehicle);
        Task<UsedVehicles?> UpdateAsync(string id, UsedVehicles vehicle);
        Task<bool> DeleteAsync(string id);
        Task<bool> UpdatePriorityAsync(string id, int priority);
        Task<bool> ActivateAsync(string id, DateTime? endDate = null);
        Task<bool> DeactivateAsync(string id);
        Task<bool> MarkAsSoldAsync(string id);
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
        Task<bool> BulkMarkAsSoldAsync(List<string> ids);
    }
}
