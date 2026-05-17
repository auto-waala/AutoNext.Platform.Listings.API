using AutoNext.Platform.Listings.API.Models.Common;
using AutoNext.Platform.Listings.API.Models.DTOs;
using AutoNext.Platform.Listings.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoNext.Platform.Listings.API.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class UsedVehiclesController : ControllerBase
    {
        private readonly IUsedVehiclesService _service;
        private readonly ILogger<UsedVehiclesController> _logger;

        public UsedVehiclesController(IUsedVehiclesService service, ILogger<UsedVehiclesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // ── GET Endpoints ───────────────────────────────────────────────────────────

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<UsedVehiclesResponseDto>>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null)
        {
            _logger.LogInformation("Getting all used vehicles - Page: {Page}, PageSize: {PageSize}", page, pageSize);

            var result = await _service.GetAllAsync(page, pageSize, sortBy, sortOrder);

            return Ok(ApiResponse<PagedResult<UsedVehiclesResponseDto>>.Success(result));
        }

        [HttpGet("active")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UsedVehiclesSummaryDto>>>> GetActiveUsedVehicles(
            [FromQuery] int limit = 50)
        {
            _logger.LogInformation("Getting active used vehicles with limit: {Limit}", limit);

            var vehicles = await _service.GetActiveUsedVehiclesAsync(limit);

            return Ok(ApiResponse<IEnumerable<UsedVehiclesResponseDto>>.Success(vehicles));
        }

        [HttpGet("top-priority")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UsedVehiclesResponseDto>>>> GetTopPriorityUsedVehicles(
            [FromQuery] int limit = 10)
        {
            _logger.LogInformation("Getting top priority used vehicles with limit: {Limit}", limit);

            var vehicles = await _service.GetTopPriorityUsedVehiclesAsync(limit);

            return Ok(ApiResponse<IEnumerable<UsedVehiclesResponseDto>>.Success(vehicles));
        }

        [HttpGet("recent")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UsedVehiclesResponseDto>>>> GetRecentlyPosted(
            [FromQuery] int days = 7,
            [FromQuery] int limit = 20)
        {
            _logger.LogInformation("Getting recently posted used vehicles - Days: {Days}, Limit: {Limit}", days, limit);

            var vehicles = await _service.GetRecentlyPostedAsync(days, limit);

            return Ok(ApiResponse<IEnumerable<UsedVehiclesResponseDto>>.Success(vehicles));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<UsedVehiclesResponseDto>>> GetById(string id)
        {
            _logger.LogInformation("Getting used vehicle by id: {Id}", id);

            var vehicle = await _service.GetByIdAsync(id);
            if (vehicle == null)
            {
                return NotFound(ApiResponse<UsedVehiclesResponseDto>.NotFound($"Used vehicle with id {id} not found"));
            }

            // Increment views asynchronously (fire and forget)
            _ = Task.Run(() => _service.IncrementViewsAsync(id));

            return Ok(ApiResponse<UsedVehiclesResponseDto>.Success(vehicle));
        }

        [HttpGet("slug/{slug}")]
        public async Task<ActionResult<ApiResponse<UsedVehiclesResponseDto>>> GetBySlug(string slug)
        {
            _logger.LogInformation("Getting used vehicle by slug: {Slug}", slug);

            var vehicle = await _service.GetBySlugAsync(slug);
            if (vehicle == null)
            {
                return NotFound(ApiResponse<UsedVehiclesResponseDto>.NotFound($"Used vehicle with slug {slug} not found"));
            }

            // Increment views asynchronously (fire and forget)
            _ = Task.Run(() => _service.IncrementViewsAsync(vehicle.Id));

            return Ok(ApiResponse<UsedVehiclesResponseDto>.Success(vehicle));
        }

        [HttpGet("model/{modelSlug}")]
        public async Task<ActionResult<ApiResponse<UsedVehiclesResponseDto>>> GetByModelSlug(string modelSlug)
        {
            _logger.LogInformation("Getting used vehicle by model slug: {ModelSlug}", modelSlug);

            var vehicle = await _service.GetByModelSlugAsync(modelSlug);
            if (vehicle == null)
            {
                return NotFound(ApiResponse<UsedVehiclesResponseDto>.NotFound($"Used vehicle with model slug {modelSlug} not found"));
            }

            return Ok(ApiResponse<UsedVehiclesResponseDto>.Success(vehicle));
        }

        // ── Filter Endpoints ────────────────────────────────────────────────────────

        [HttpGet("brand/{brandName}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UsedVehiclesSummaryDto>>>> GetByBrand(
            string brandName,
            [FromQuery] int limit = 20)
        {
            _logger.LogInformation("Getting used vehicles by brand: {BrandName}, Limit: {Limit}", brandName, limit);

            var vehicles = await _service.GetByBrandAsync(brandName, limit);

            return Ok(ApiResponse<IEnumerable<UsedVehiclesResponseDto>>.Success(vehicles));
        }

        [HttpGet("type/{vehicleType}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UsedVehiclesResponseDto>>>> GetByVehicleType(
            string vehicleType,
            [FromQuery] int limit = 20)
        {
            _logger.LogInformation("Getting used vehicles by type: {VehicleType}, Limit: {Limit}", vehicleType, limit);

            var vehicles = await _service.GetByVehicleTypeAsync(vehicleType, limit);

            return Ok(ApiResponse<IEnumerable<UsedVehiclesResponseDto>>.Success(vehicles));
        }

        [HttpGet("price-range")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UsedVehiclesResponseDto>>>> GetByPriceRange(
            [FromQuery] decimal minPrice,
            [FromQuery] decimal maxPrice,
            [FromQuery] int limit = 20)
        {
            _logger.LogInformation("Getting used vehicles by price range: {MinPrice} - {MaxPrice}, Limit: {Limit}", minPrice, maxPrice, limit);

            var vehicles = await _service.GetByPriceRangeAsync(minPrice, maxPrice, limit);

            return Ok(ApiResponse<IEnumerable<UsedVehiclesResponseDto>>.Success(vehicles));
        }

        [HttpGet("city/{city}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UsedVehiclesResponseDto>>>> GetByCity(
            string city,
            [FromQuery] int limit = 20)
        {
            _logger.LogInformation("Getting used vehicles by city: {City}, Limit: {Limit}", city, limit);

            var vehicles = await _service.GetByCityAsync(city, limit);

            return Ok(ApiResponse<IEnumerable<UsedVehiclesResponseDto>>.Success(vehicles));
        }

        [HttpGet("fuel-type/{fuelType}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UsedVehiclesResponseDto>>>> GetByFuelType(
            string fuelType,
            [FromQuery] int limit = 20)
        {
            _logger.LogInformation("Getting used vehicles by fuel type: {FuelType}, Limit: {Limit}", fuelType, limit);

            var vehicles = await _service.GetByFuelTypeAsync(fuelType, limit);

            return Ok(ApiResponse<IEnumerable<UsedVehiclesResponseDto>>.Success(vehicles));
        }

        [HttpGet("transmission/{transmission}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UsedVehiclesResponseDto>>>> GetByTransmission(
            string transmission,
            [FromQuery] int limit = 20)
        {
            _logger.LogInformation("Getting used vehicles by transmission: {Transmission}, Limit: {Limit}", transmission, limit);

            var vehicles = await _service.GetByTransmissionAsync(transmission, limit);

            return Ok(ApiResponse<IEnumerable<UsedVehiclesResponseDto>>.Success(vehicles));
        }

        [HttpGet("year-range")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UsedVehiclesResponseDto>>>> GetByYearRange(
            [FromQuery] int minYear,
            [FromQuery] int maxYear,
            [FromQuery] int limit = 20)
        {
            _logger.LogInformation("Getting used vehicles by year range: {MinYear} - {MaxYear}, Limit: {Limit}", minYear, maxYear, limit);

            var vehicles = await _service.GetByYearRangeAsync(minYear, maxYear, limit);

            return Ok(ApiResponse<IEnumerable<UsedVehiclesResponseDto>>.Success(vehicles));
        }

        [HttpGet("seller-type/{sellerType}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UsedVehiclesResponseDto>>>> GetBySellerType(
            string sellerType,
            [FromQuery] int limit = 20)
        {
            _logger.LogInformation("Getting used vehicles by seller type: {SellerType}, Limit: {Limit}", sellerType, limit);

            var vehicles = await _service.GetBySellerTypeAsync(sellerType, limit);

            return Ok(ApiResponse<IEnumerable<UsedVehiclesResponseDto>>.Success(vehicles));
        }

        [HttpGet("search")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UsedVehiclesResponseDto>>>> Search(
            [FromQuery] string q,
            [FromQuery] int limit = 20)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return BadRequest(ApiResponse<IEnumerable<UsedVehiclesResponseDto>>.Error("Search term cannot be empty"));
            }

            _logger.LogInformation("Searching used vehicles with term: {SearchTerm}, Limit: {Limit}", q, limit);

            var vehicles = await _service.SearchAsync(q, limit);

            return Ok(ApiResponse<IEnumerable<UsedVehiclesResponseDto>>.Success(vehicles));
        }

        [HttpPost("advanced-search")]
        public async Task<ActionResult<ApiResponse<PagedResult<UsedVehiclesResponseDto>>>> AdvancedSearch(
            [FromBody] UsedVehiclesSearchCriteria criteria)
        {
            if (criteria == null)
            {
                return BadRequest(ApiResponse<PagedResult<UsedVehiclesResponseDto>>.Error("Search criteria cannot be null"));
            }

            _logger.LogInformation("Performing advanced search for used vehicles");

            var result = await _service.AdvancedSearchAsync(criteria);

            return Ok(ApiResponse<PagedResult<UsedVehiclesResponseDto>>.Success(result));
        }

        // ── CRUD Endpoints ─────────────────────────────────────────────────────────

        [HttpPost]
        public async Task<ActionResult<ApiResponse<UsedVehiclesResponseDto>>> Create([FromBody] UsedVehiclesRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<UsedVehiclesResponseDto>.Error("Invalid request data"));
            }

            _logger.LogInformation("Creating used vehicle for brand: {BrandName}, model: {ModelName}",
                request.BrandName, request.ModelName);

            try
            {
                var vehicle = await _service.CreateAsync(request);
                return Ok(ApiResponse<UsedVehiclesResponseDto>.Success(vehicle, "Used vehicle created successfully"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation error while creating used vehicle");
                return Conflict(ApiResponse<UsedVehiclesResponseDto>.Error(ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<UsedVehiclesResponseDto>>> Update(string id, [FromBody] UsedVehiclesRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<UsedVehiclesResponseDto>.Error("Invalid request data"));
            }

            _logger.LogInformation("Updating used vehicle with id: {Id}", id);

            try
            {
                var vehicle = await _service.UpdateAsync(id, request);
                if (vehicle == null)
                {
                    return NotFound(ApiResponse<UsedVehiclesResponseDto>.NotFound($"Used vehicle with id {id} not found"));
                }

                return Ok(ApiResponse<UsedVehiclesResponseDto>.Success(vehicle, "Used vehicle updated successfully"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation error while updating used vehicle");
                return Conflict(ApiResponse<UsedVehiclesResponseDto>.Error(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(string id)
        {
            _logger.LogInformation("Deleting used vehicle with id: {Id}", id);

            var result = await _service.DeleteAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.NotFound($"Used vehicle with id {id} not found"));
            }

            return Ok(ApiResponse<bool>.Success(true, "Used vehicle deleted successfully"));
        }

        // ── Used Vehicles Specific Operations ───────────────────────────────────────

        [HttpPatch("{id}/priority")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdatePriority(
            string id,
            [FromBody] UpdatePriorityRequest request)
        {
            _logger.LogInformation("Updating priority for used vehicle {Id} to {Priority}", id, request.Priority);

            try
            {
                var result = await _service.UpdatePriorityAsync(id, request.Priority);
                if (!result)
                {
                    return NotFound(ApiResponse<bool>.NotFound($"Used vehicle with id {id} not found"));
                }

                return Ok(ApiResponse<bool>.Success(true, "Priority updated successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<bool>.Error(ex.Message));
            }
        }

        [HttpPost("{id}/activate")]
        public async Task<ActionResult<ApiResponse<bool>>> Activate(
            string id,
            [FromBody] ActivateUsedVehicleRequest? request = null)
        {
            _logger.LogInformation("Activating used vehicle with id: {Id}", id);

            var result = await _service.ActivateAsync(id, request?.EndDate);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.NotFound($"Used vehicle with id {id} not found"));
            }

            return Ok(ApiResponse<bool>.Success(true, "Used vehicle activated successfully"));
        }

        [HttpPost("{id}/deactivate")]
        public async Task<ActionResult<ApiResponse<bool>>> Deactivate(string id)
        {
            _logger.LogInformation("Deactivating used vehicle with id: {Id}", id);

            var result = await _service.DeactivateAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.NotFound($"Used vehicle with id {id} not found"));
            }

            return Ok(ApiResponse<bool>.Success(true, "Used vehicle deactivated successfully"));
        }

        [HttpPost("{id}/mark-as-sold")]
        public async Task<ActionResult<ApiResponse<bool>>> MarkAsSold(string id)
        {
            _logger.LogInformation("Marking used vehicle as sold with id: {Id}", id);

            var result = await _service.MarkAsSoldAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.NotFound($"Used vehicle with id {id} not found"));
            }

            return Ok(ApiResponse<bool>.Success(true, "Used vehicle marked as sold successfully"));
        }

        [HttpPost("bulk/priority")]
        public async Task<ActionResult<ApiResponse<bool>>> BulkUpdatePriority(
            [FromBody] Dictionary<string, int> priorityUpdates)
        {
            _logger.LogInformation("Bulk updating priorities for {Count} used vehicles", priorityUpdates.Count);

            var result = await _service.BulkUpdatePriorityAsync(priorityUpdates);
            if (!result)
            {
                return BadRequest(ApiResponse<bool>.Error("Failed to update priorities"));
            }

            return Ok(ApiResponse<bool>.Success(true, "Priorities updated successfully"));
        }

        // ── Engagement Endpoints ────────────────────────────────────────────────────

        [HttpPost("{id}/like")]
        public async Task<ActionResult<ApiResponse<bool>>> Like(string id)
        {
            _logger.LogInformation("User liked used vehicle: {Id}", id);

            var result = await _service.IncrementLikesAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.NotFound($"Used vehicle with id {id} not found"));
            }

            return Ok(ApiResponse<bool>.Success(true));
        }

        [HttpPost("{id}/share")]
        public async Task<ActionResult<ApiResponse<bool>>> Share(string id)
        {
            _logger.LogInformation("User shared used vehicle: {Id}", id);

            var result = await _service.IncrementSharesAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.NotFound($"Used vehicle with id {id} not found"));
            }

            return Ok(ApiResponse<bool>.Success(true));
        }

        [HttpPost("{id}/enquire")]
        public async Task<ActionResult<ApiResponse<bool>>> Enquire(string id)
        {
            _logger.LogInformation("User made enquiry for used vehicle: {Id}", id);

            var result = await _service.IncrementEnquiriesAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.NotFound($"Used vehicle with id {id} not found"));
            }

            return Ok(ApiResponse<bool>.Success(true));
        }

        // ── Rating Endpoints ────────────────────────────────────────────────────────

        [HttpPost("{id}/rating")]
        public async Task<ActionResult<ApiResponse<bool>>> AddRating(
            string id,
            [FromBody] UserRatingDto rating)
        {
            if (rating.Rating < 1 || rating.Rating > 5)
            {
                return BadRequest(ApiResponse<bool>.Error("Rating must be between 1 and 5"));
            }

            _logger.LogInformation("Adding rating {Rating} for used vehicle: {Id}", rating.Rating, id);

            try
            {
                var result = await _service.AddRatingAsync(id, rating);
                if (!result)
                {
                    return NotFound(ApiResponse<bool>.NotFound($"Used vehicle with id {id} not found"));
                }

                return Ok(ApiResponse<bool>.Success(true, "Rating added successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<bool>.Error(ex.Message));
            }
        }

        // ── Statistics Endpoints ────────────────────────────────────────────────────

        [HttpGet("statistics/counts")]
        public async Task<ActionResult<ApiResponse<object>>> GetStatistics()
        {
            _logger.LogInformation("Getting used vehicles statistics");

            var totalCount = await _service.GetTotalCountAsync();
            var activeCount = await _service.GetActiveCountAsync();
            var soldCount = await _service.GetSoldCountAsync();

            return Ok(ApiResponse<object>.Success(new
            {
                Total = totalCount,
                Active = activeCount,
                Sold = soldCount
            }));
        }
    }
}