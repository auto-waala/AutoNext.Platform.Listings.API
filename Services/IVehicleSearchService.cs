using AutoNext.Platform.Listings.API.Models;
using MongoDB.Driver.Search;

namespace AutoNext.Platform.Listings.API.Services
{
    public interface IVehicleSearchService
    {
        // Basic Search
        Task<IEnumerable<Vehicle>> SearchByKeywordAsync(string keyword, SearchOptions? options = null);
        Task<IEnumerable<Vehicle>> SearchByFiltersAsync(VehicleSearchCriteria criteria, SearchOptions? options = null);

        // Advanced Search
        Task<IEnumerable<Vehicle>> AdvancedSearchAsync(VehicleAdvancedSearchCriteria criteria);
        Task<SearchResult<Vehicle>> PaginatedSearchAsync(VehicleSearchCriteria criteria, int page = 1, int pageSize = 20);

        // Location-Based Search
        Task<IEnumerable<Vehicle>> SearchNearbyAsync(double lat, double lng, double radiusInKm, SearchOptions? options = null);
        Task<IEnumerable<Vehicle>> SearchByCityAsync(string cityName, SearchOptions? options = null);
        Task<IEnumerable<Vehicle>> SearchByStateAsync(string stateName, SearchOptions? options = null);

        // Specialized Searches
        Task<IEnumerable<Vehicle>> SearchByMakeAndModelAsync(string make, string model, int? year = null);
        Task<IEnumerable<Vehicle>> SearchByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<IEnumerable<Vehicle>> SearchByYearRangeAsync(int minYear, int maxYear);
        Task<IEnumerable<Vehicle>> SearchByFuelTypeAsync(string fuelType);
        Task<IEnumerable<Vehicle>> SearchByTransmissionTypeAsync(string transmission);
        Task<IEnumerable<Vehicle>> SearchByColorAsync(string color);

        // Faceted Search
        Task<FacetedSearchResult> GetFacetedSearchResultsAsync(VehicleSearchCriteria criteria);
        Task<Dictionary<string, List<FacetCount>>> GetSearchFacetsAsync(VehicleSearchCriteria criteria);

        // Personalized Search
        Task<IEnumerable<Vehicle>> GetPersonalizedRecommendationsAsync(string userId, int limit = 20);
        Task<IEnumerable<Vehicle>> GetSimilarVehiclesAsync(string vehicleId, int limit = 5);
        Task<IEnumerable<Vehicle>> GetTrendingVehiclesAsync(int limit = 10);

        // Search History
        Task SaveSearchHistoryAsync(string userId, VehicleSearchCriteria criteria);
        Task<IEnumerable<SavedSearch>> GetUserSavedSearchesAsync(string userId);
        Task<bool> DeleteSavedSearchAsync(string userId, string searchId);

        // Search Suggestions
        Task<IEnumerable<string>> GetSearchSuggestionsAsync(string prefix, int limit = 10);
        Task<IEnumerable<Vehicle>> GetRecentlyViewedVehiclesAsync(string userId, int limit = 10);

        // Comparison
        Task<VehicleComparison> CompareVehiclesAsync(List<string> vehicleIds);

        // Export
        Task<byte[]> ExportSearchResultsAsync(VehicleSearchCriteria criteria, ExportFormat format = ExportFormat.CSV);
    }
}
