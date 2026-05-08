using AutoNext.Platform.Listings.API.Models;

namespace AutoNext.Platform.Listings.API.Repositories
{
    public interface IVehicleSearchRepository
    {
        Task<IEnumerable<Vehicle>> AdvancedSearchAsync(VehicleAdvancedSearchCriteria criteria);
        Task<IEnumerable<Vehicle>> SearchByLocationAsync(double lat, double lng, double radiusInKm, int limit = 50);
        Task<IEnumerable<Vehicle>> SearchByKeywordAsync(string keyword, int limit = 50);
        Task<IEnumerable<Vehicle>> GetRecommendationsAsync(string userId, int limit = 20);
        Task<Dictionary<string, long>> GetFacetedSearchCountsAsync(VehicleSearchCriteria criteria);
        Task<IEnumerable<Vehicle>> GetRecentlyViewedAsync(string userId, int limit = 10);
        Task<IEnumerable<Vehicle>> GetSavedSearchesAsync(string userId, string searchId);
        Task<IEnumerable<Vehicle>> SearchByMakeAndModelAsync(string make, string model, int year = 0);
        Task<IEnumerable<Vehicle>> GetVehiclesForComparisonAsync(List<string> vehicleIds);
    }
}