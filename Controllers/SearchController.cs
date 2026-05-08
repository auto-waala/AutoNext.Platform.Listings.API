using AutoNext.Platform.Listings.API.Models;
using AutoNext.Platform.Listings.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AutoNext.Platform.Listings.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [AllowAnonymous]
    public class SearchController : ControllerBase
    {
        private readonly IVehicleSearchService _vehicleSearchService;
        private readonly ILogger<SearchController> _logger;

        public SearchController(IVehicleSearchService vehicleSearchService, ILogger<SearchController> logger)
        {
            _vehicleSearchService = vehicleSearchService;
            _logger = logger;
        }

        [HttpGet("keyword")]
        public async Task<IActionResult> SearchByKeyword([FromQuery] string q, [FromQuery] int limit = 50)
        {
            _logger.LogInformation("Search by keyword: {Keyword}", q);

            if (string.IsNullOrWhiteSpace(q))
            {
                return BadRequest(ApiResponse<object>.Error("Search keyword is required", 400));
            }

            var results = await _vehicleSearchService.SearchByKeywordAsync(q, new SearchOptions { Limit = limit });
            return Ok(ApiResponse<IEnumerable<Vehicle>>.Ok(results, "Search completed successfully"));
        }

        [HttpPost("advanced")]
        public async Task<IActionResult> AdvancedSearch([FromBody] VehicleAdvancedSearchCriteria criteria)
        {
            _logger.LogInformation("Advanced search with criteria: {@Criteria}", criteria);

            var results = await _vehicleSearchService.AdvancedSearchAsync(criteria);
            return Ok(ApiResponse<IEnumerable<Vehicle>>.Ok(results, "Search completed successfully"));
        }

        [HttpGet("filter")]
        public async Task<IActionResult> FilterVehicles(
            [FromQuery] string? make = null,
            [FromQuery] string? model = null,
            [FromQuery] int? minYear = null,
            [FromQuery] int? maxYear = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] string? fuelType = null,
            [FromQuery] string? transmission = null,
            [FromQuery] string? city = null,
            [FromQuery] string? vehicleType = null,
            [FromQuery] int? maxKilometers = null,
            [FromQuery] string? color = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            _logger.LogInformation("Filter vehicles with parameters: Make={Make}, Model={Model}, MinPrice={MinPrice}, MaxPrice={MaxPrice}",
                make, model, minPrice, maxPrice);

            var criteria = new VehicleSearchCriteria
            {
                Make = make,
                Model = model,
                MinYear = minYear,
                MaxYear = maxYear,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                FuelType = fuelType,
                Transmission = transmission,
                CityName = city,
                VehicleType = vehicleType,
                MaxKilometers = maxKilometers,
                Color = color
            };

            var result = await _vehicleSearchService.PaginatedSearchAsync(criteria, page, pageSize);

            Response.Headers.Append("X-Total-Count", result.TotalCount.ToString());
            Response.Headers.Append("X-Page", result.Page.ToString());
            Response.Headers.Append("X-Page-Size", result.PageSize.ToString());
            Response.Headers.Append("X-Total-Pages", result.TotalPages.ToString());

            return Ok(ApiResponse<SearchResult<Vehicle>>.Ok(result, "Filter completed successfully"));
        }

        [HttpGet("nearby")]
        public async Task<IActionResult> SearchNearby(
            [FromQuery] double lat,
            [FromQuery] double lng,
            [FromQuery] double radiusInKm = 10,
            [FromQuery] int limit = 50)
        {
            _logger.LogInformation("Search nearby: Lat={Lat}, Lng={Lng}, Radius={Radius}", lat, lng, radiusInKm);

            var results = await _vehicleSearchService.SearchNearbyAsync(lat, lng, radiusInKm, new SearchOptions { Limit = limit });
            return Ok(ApiResponse<IEnumerable<Vehicle>>.Ok(results, "Nearby search completed successfully"));
        }

        [HttpGet("by-make-model")]
        public async Task<IActionResult> SearchByMakeAndModel(
            [FromQuery] string make,
            [FromQuery] string model,
            [FromQuery] int? year = null)
        {
            _logger.LogInformation("Search by make and model: {Make} {Model} {Year}", make, model, year);

            var results = await _vehicleSearchService.SearchByMakeAndModelAsync(make, model, year);
            return Ok(ApiResponse<IEnumerable<Vehicle>>.Ok(results, "Search completed successfully"));
        }

        [HttpGet("price-range")]
        public async Task<IActionResult> SearchByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
        {
            _logger.LogInformation("Search by price range: {MinPrice} - {MaxPrice}", minPrice, maxPrice);

            var results = await _vehicleSearchService.SearchByPriceRangeAsync(minPrice, maxPrice);
            return Ok(ApiResponse<IEnumerable<Vehicle>>.Ok(results, "Search completed successfully"));
        }

        [HttpGet("suggestions")]
        public async Task<IActionResult> GetSuggestions([FromQuery] string prefix, [FromQuery] int limit = 10)
        {
            _logger.LogInformation("Get suggestions for prefix: {Prefix}", prefix);

            var suggestions = await _vehicleSearchService.GetSearchSuggestionsAsync(prefix, limit);
            return Ok(ApiResponse<IEnumerable<string>>.Ok(suggestions, "Suggestions retrieved successfully"));
        }

        [HttpGet("trending")]
        public async Task<IActionResult> GetTrendingVehicles([FromQuery] int limit = 10)
        {
            _logger.LogInformation("Get trending vehicles, limit: {Limit}", limit);

            var vehicles = await _vehicleSearchService.GetTrendingVehiclesAsync(limit);
            return Ok(ApiResponse<IEnumerable<Vehicle>>.Ok(vehicles, "Trending vehicles retrieved successfully"));
        }

        [HttpGet("similar/{vehicleId}")]
        public async Task<IActionResult> GetSimilarVehicles(string vehicleId, [FromQuery] int limit = 5)
        {
            _logger.LogInformation("Get similar vehicles for: {VehicleId}", vehicleId);

            var vehicles = await _vehicleSearchService.GetSimilarVehiclesAsync(vehicleId, limit);
            return Ok(ApiResponse<IEnumerable<Vehicle>>.Ok(vehicles, "Similar vehicles retrieved successfully"));
        }

        [HttpPost("compare")]
        public async Task<IActionResult> CompareVehicles([FromBody] List<string> vehicleIds)
        {
            _logger.LogInformation("Compare vehicles: {@VehicleIds}", vehicleIds);

            if (vehicleIds == null || vehicleIds.Count < 2 || vehicleIds.Count > 4)
            {
                return BadRequest(ApiResponse<object>.Error("Please provide between 2 and 4 vehicle IDs for comparison", 400));
            }

            var comparison = await _vehicleSearchService.CompareVehiclesAsync(vehicleIds);
            return Ok(ApiResponse<VehicleComparison>.Ok(comparison, "Comparison completed successfully"));
        }

        [HttpGet("facets")]
        public async Task<IActionResult> GetSearchFacets([FromQuery] string? make = null, [FromQuery] string? model = null)
        {
            _logger.LogInformation("Get search facets");

            var criteria = new VehicleSearchCriteria { Make = make, Model = model };
            var facets = await _vehicleSearchService.GetSearchFacetsAsync(criteria);

            return Ok(ApiResponse<Dictionary<string, List<FacetCount>>>.Ok(facets, "Facets retrieved successfully"));
        }
    }
}
