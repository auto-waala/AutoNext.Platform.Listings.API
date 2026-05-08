using AutoNext.Platform.Listings.API.Models;

namespace AutoNext.Platform.Listings.API.Services
{
    public interface IVehicleService
    {
        // Basic CRUD Operations
        Task<Vehicle> GetVehicleByIdAsync(string id);
        Task<Vehicle> GetVehicleByVINAsync(string vin);
        Task<Vehicle> GetVehicleByChassisNumberAsync(string chassisNumber);
        Task<Vehicle> GetVehicleByEngineNumberAsync(string engineNumber);
        Task<Vehicle> GetVehicleByRegistrationNumberAsync(string registrationNumber);

        // Listing Management
        Task<IEnumerable<Vehicle>> GetAllActiveVehiclesAsync(int page = 1, int pageSize = 20);
        Task<IEnumerable<Vehicle>> GetVehiclesBySellerAsync(string sellerId);
        Task<Vehicle> CreateVehicleAsync(Vehicle vehicle, string createdBy);
        Task<Vehicle> UpdateVehicleAsync(string id, Vehicle vehicle, string modifiedBy);
        Task<bool> DeleteVehicleAsync(string id);
        Task<bool> SoftDeleteVehicleAsync(string id, string modifiedBy);

        // Search and Filter
        Task<IEnumerable<Vehicle>> SearchVehiclesAsync(VehicleSearchCriteria criteria);
        Task<IEnumerable<Vehicle>> GetFeaturedVehiclesAsync(int limit = 10);
        Task<IEnumerable<Vehicle>> GetSimilarVehiclesAsync(string vehicleId, int limit = 5);
        Task<IEnumerable<Vehicle>> GetRecentVehiclesAsync(int hours = 24);
        Task<IEnumerable<Vehicle>> GetVehiclesByPriceRangeAsync(decimal minPrice, decimal maxPrice, int limit = 50);

        // Vehicle Status Management
        Task<bool> ActivateVehicleAsync(string id, string modifiedBy);
        Task<bool> DeactivateVehicleAsync(string id, string modifiedBy);
        Task<bool> MarkVehicleAsSoldAsync(string id, string modifiedBy, DateTime? soldDate = null);
        Task<bool> ExtendVehicleListingAsync(string id, int additionalDays, string modifiedBy);

        // Analytics and Metrics
        Task<bool> RecordVehicleViewAsync(string id);
        Task<bool> RecordVehicleFavoriteAsync(string id);
        Task<VehicleAnalytics> GetVehicleAnalyticsAsync(string id);
        Task<Dictionary<string, long>> GetVehicleStatisticsByMakeAsync();
        Task<long> GetTotalActiveVehiclesCountAsync();

        // Bulk Operations
        Task<IEnumerable<Vehicle>> BulkCreateVehiclesAsync(IEnumerable<Vehicle> vehicles, string createdBy);
        Task<bool> BulkUpdateVehicleStatusAsync(IEnumerable<string> ids, string status, string modifiedBy);
        Task<bool> BulkSoftDeleteVehiclesAsync(IEnumerable<string> ids, string modifiedBy);

        // Validation
        Task<bool> IsVINUniqueAsync(string vin, string? excludeVehicleId = null);
        Task<bool> IsChassisNumberUniqueAsync(string chassisNumber, string? excludeVehicleId = null);
        Task<bool> IsEngineNumberUniqueAsync(string engineNumber, string? excludeVehicleId = null);
        Task<bool> IsRegistrationNumberUniqueAsync(string registrationNumber, string? excludeVehicleId = null);

        // Expiry Management
        Task<IEnumerable<Vehicle>> GetExpiringListingsAsync(int daysThreshold = 7);
        Task<int> ProcessExpiredListingsAsync();
    }
}