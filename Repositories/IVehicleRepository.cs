using AutoNext.Platform.Listings.API.Models;

namespace AutoNext.Platform.Listings.API.Repositories
{
    public interface IVehicleRepository
    {
        // Basic CRUD
        Task<Vehicle> GetByIdAsync(string id);
        Task<Vehicle> GetByVINAsync(string vin);
        Task<Vehicle> GetByChassisNumberAsync(string chassisNumber);
        Task<Vehicle> GetByEngineNumberAsync(string engineNumber);
        Task<Vehicle> GetByRegistrationNumberAsync(string registrationNumber);
        Task<IEnumerable<Vehicle>> GetActiveVehiclesAsync(int page = 1, int pageSize = 20);
        Task<IEnumerable<Vehicle>> GetVehiclesBySellerAsync(string sellerId);
        Task<IEnumerable<Vehicle>> SearchVehiclesAsync(VehicleSearchCriteria criteria);
        Task<Vehicle> CreateAsync(Vehicle vehicle);
        Task<Vehicle> UpdateAsync(Vehicle vehicle);
        Task<bool> DeleteAsync(string id);
        Task<bool> SoftDeleteAsync(string id);
        Task<long> GetTotalCountAsync();

        // Additional methods
        Task<bool> UpdateVehicleStatusAsync(string id, string status, string modifiedBy);
        Task<bool> IncrementVehicleViewsAsync(string id);
        Task<bool> IncrementFavoritesAsync(string id);
        Task<IEnumerable<Vehicle>> GetFeaturedVehiclesAsync(int limit = 10);
        Task<IEnumerable<Vehicle>> GetSimilarVehiclesAsync(string vehicleId, int limit = 5);
        Task<IEnumerable<Vehicle>> GetRecentVehiclesAsync(int hours = 24);
        Task<bool> ExtendListingAsync(string id, int additionalDays, string modifiedBy);
        Task<bool> AddServiceRecordAsync(string id, ServiceRecord serviceRecord);
        Task<Dictionary<string, long>> GetVehicleCountByMakeAsync();
        Task<IEnumerable<Vehicle>> GetVehiclesByPriceRangeAsync(decimal minPrice, decimal maxPrice, int limit = 50);
        Task<IEnumerable<Vehicle>> GetExpiringListingsAsync(int daysThreshold = 7);
    }
}