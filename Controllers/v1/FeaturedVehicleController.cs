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
    public class FeaturedVehicleController : ControllerBase
    {
        private readonly IFeaturedVehicleService _service;
        private readonly ILogger<FeaturedVehicleController> _logger;

        public FeaturedVehicleController(IFeaturedVehicleService service, ILogger<FeaturedVehicleController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // ── GET Endpoints ───────────────────────────────────────────────────────────

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<FeaturedVehicleResponseDto>>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null)
        {
            _logger.LogInformation("Getting all featured vehicles - Page: {Page}, PageSize: {PageSize}", page, pageSize);

            var result = await _service.GetAllAsync(page, pageSize, sortBy, sortOrder);

            return Ok(ApiResponse<PagedResult<FeaturedVehicleResponseDto>>.Success(result));
        }

        [HttpGet("active")]
        public async Task<ActionResult<ApiResponse<IEnumerable<FeaturedVehicleSummaryDto>>>> GetActiveFeaturedVehicles(
            [FromQuery] int limit = 50)
        {
            _logger.LogInformation("Getting active featured vehicles with limit: {Limit}", limit);

            var vehicles = await _service.GetActiveFeaturedVehiclesAsync(limit);

            return Ok(ApiResponse<IEnumerable<FeaturedVehicleSummaryDto>>.Success(vehicles));
        }

        [HttpGet("top-priority")]
        public async Task<ActionResult<ApiResponse<IEnumerable<FeaturedVehicleSummaryDto>>>> GetTopPriorityFeaturedVehicles(
            [FromQuery] int limit = 10)
        {
            _logger.LogInformation("Getting top priority featured vehicles with limit: {Limit}", limit);

            var vehicles = await _service.GetTopPriorityFeaturedVehiclesAsync(limit);

            return Ok(ApiResponse<IEnumerable<FeaturedVehicleSummaryDto>>.Success(vehicles));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<FeaturedVehicleResponseDto>>> GetById(string id)
        {
            _logger.LogInformation("Getting featured vehicle by id: {Id}", id);

            var vehicle = await _service.GetByIdAsync(id);
            if (vehicle == null)
            {
                return NotFound(ApiResponse<FeaturedVehicleResponseDto>.NotFound($"Featured vehicle with id {id} not found"));
            }

            // Increment views asynchronously (fire and forget)
            _ = Task.Run(() => _service.IncrementViewsAsync(id));

            return Ok(ApiResponse<FeaturedVehicleResponseDto>.Success(vehicle));
        }

        [HttpGet("slug/{slug}")]
        public async Task<ActionResult<ApiResponse<FeaturedVehicleResponseDto>>> GetBySlug(string slug)
        {
            _logger.LogInformation("Getting featured vehicle by slug: {Slug}", slug);

            var vehicle = await _service.GetBySlugAsync(slug);
            if (vehicle == null)
            {
                return NotFound(ApiResponse<FeaturedVehicleResponseDto>.NotFound($"Featured vehicle with slug {slug} not found"));
            }

            // Increment views asynchronously (fire and forget)
            _ = Task.Run(() => _service.IncrementViewsAsync(vehicle.Id));

            return Ok(ApiResponse<FeaturedVehicleResponseDto>.Success(vehicle));
        }

        [HttpGet("model/{modelSlug}")]
        public async Task<ActionResult<ApiResponse<FeaturedVehicleResponseDto>>> GetByModelSlug(string modelSlug)
        {
            _logger.LogInformation("Getting featured vehicle by model slug: {ModelSlug}", modelSlug);

            var vehicle = await _service.GetByModelSlugAsync(modelSlug);
            if (vehicle == null)
            {
                return NotFound(ApiResponse<FeaturedVehicleResponseDto>.NotFound($"Featured vehicle with model slug {modelSlug} not found"));
            }

            return Ok(ApiResponse<FeaturedVehicleResponseDto>.Success(vehicle));
        }

        // ── Filter Endpoints ────────────────────────────────────────────────────────

        [HttpGet("brand/{brandName}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<FeaturedVehicleSummaryDto>>>> GetByBrand(
            string brandName,
            [FromQuery] int limit = 20)
        {
            _logger.LogInformation("Getting featured vehicles by brand: {BrandName}, Limit: {Limit}", brandName, limit);

            var vehicles = await _service.GetByBrandAsync(brandName, limit);

            return Ok(ApiResponse<IEnumerable<FeaturedVehicleSummaryDto>>.Success(vehicles));
        }

        [HttpGet("type/{vehicleType}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<FeaturedVehicleSummaryDto>>>> GetByVehicleType(
            string vehicleType,
            [FromQuery] int limit = 20)
        {
            _logger.LogInformation("Getting featured vehicles by type: {VehicleType}, Limit: {Limit}", vehicleType, limit);

            var vehicles = await _service.GetByVehicleTypeAsync(vehicleType, limit);

            return Ok(ApiResponse<IEnumerable<FeaturedVehicleSummaryDto>>.Success(vehicles));
        }

        [HttpGet("price-range")]
        public async Task<ActionResult<ApiResponse<IEnumerable<FeaturedVehicleSummaryDto>>>> GetByPriceRange(
            [FromQuery] decimal minPrice,
            [FromQuery] decimal maxPrice,
            [FromQuery] int limit = 20)
        {
            _logger.LogInformation("Getting featured vehicles by price range: {MinPrice} - {MaxPrice}, Limit: {Limit}", minPrice, maxPrice, limit);

            var vehicles = await _service.GetByPriceRangeAsync(minPrice, maxPrice, limit);

            return Ok(ApiResponse<IEnumerable<FeaturedVehicleSummaryDto>>.Success(vehicles));
        }

        [HttpGet("city/{city}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<FeaturedVehicleSummaryDto>>>> GetByCity(
            string city,
            [FromQuery] int limit = 20)
        {
            _logger.LogInformation("Getting featured vehicles by city: {City}, Limit: {Limit}", city, limit);

            var vehicles = await _service.GetByCityAsync(city, limit);

            return Ok(ApiResponse<IEnumerable<FeaturedVehicleSummaryDto>>.Success(vehicles));
        }

        [HttpGet("search")]
        public async Task<ActionResult<ApiResponse<IEnumerable<FeaturedVehicleSummaryDto>>>> Search(
            [FromQuery] string q,
            [FromQuery] int limit = 20)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return BadRequest(ApiResponse<IEnumerable<FeaturedVehicleSummaryDto>>.Error("Search term cannot be empty"));
            }

            _logger.LogInformation("Searching featured vehicles with term: {SearchTerm}, Limit: {Limit}", q, limit);

            var vehicles = await _service.SearchAsync(q, limit);

            return Ok(ApiResponse<IEnumerable<FeaturedVehicleSummaryDto>>.Success(vehicles));
        }

        // ── CRUD Endpoints ─────────────────────────────────────────────────────────

        [HttpPost]
        public async Task<ActionResult<ApiResponse<FeaturedVehicleResponseDto>>> Create([FromBody] FeaturedVehicleRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<FeaturedVehicleResponseDto>.Error("Invalid request data"));
            }

            _logger.LogInformation("Creating featured vehicle for brand: {BrandName}, model: {ModelName}",
                request.BrandName, request.ModelName);

            try
            {
                var vehicle = await _service.CreateAsync(request);
                return Ok(ApiResponse<FeaturedVehicleResponseDto>.Success(vehicle, "Featured vehicle created successfully"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation error while creating featured vehicle");
                return Conflict(ApiResponse<FeaturedVehicleResponseDto>.Error(ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<FeaturedVehicleResponseDto>>> Update(string id, [FromBody] FeaturedVehicleRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<FeaturedVehicleResponseDto>.Error("Invalid request data"));
            }

            _logger.LogInformation("Updating featured vehicle with id: {Id}", id);

            try
            {
                var vehicle = await _service.UpdateAsync(id, request);
                if (vehicle == null)
                {
                    return NotFound(ApiResponse<FeaturedVehicleResponseDto>.NotFound($"Featured vehicle with id {id} not found"));
                }

                return Ok(ApiResponse<FeaturedVehicleResponseDto>.Success(vehicle, "Featured vehicle updated successfully"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation error while updating featured vehicle");
                return Conflict(ApiResponse<FeaturedVehicleResponseDto>.Error(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(string id)
        {
            _logger.LogInformation("Deleting featured vehicle with id: {Id}", id);

            var result = await _service.DeleteAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.NotFound($"Featured vehicle with id {id} not found"));
            }

            return Ok(ApiResponse<bool>.Success(true, "Featured vehicle deleted successfully"));
        }

        // ── Featured-specific Operations ────────────────────────────────────────────

        [HttpPatch("{id}/priority")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdatePriority(
            string id,
            [FromBody] UpdatePriorityRequest request)
        {
            _logger.LogInformation("Updating priority for featured vehicle {Id} to {Priority}", id, request.Priority);

            try
            {
                var result = await _service.UpdatePriorityAsync(id, request.Priority);
                if (!result)
                {
                    return NotFound(ApiResponse<bool>.NotFound($"Featured vehicle with id {id} not found"));
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
            [FromBody] ActivateFeaturedVehicleRequest? request = null)
        {
            _logger.LogInformation("Activating featured vehicle with id: {Id}", id);

            var result = await _service.ActivateAsync(id, request?.EndDate);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.NotFound($"Featured vehicle with id {id} not found"));
            }

            return Ok(ApiResponse<bool>.Success(true, "Featured vehicle activated successfully"));
        }

        [HttpPost("{id}/deactivate")]
        public async Task<ActionResult<ApiResponse<bool>>> Deactivate(string id)
        {
            _logger.LogInformation("Deactivating featured vehicle with id: {Id}", id);

            var result = await _service.DeactivateAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.NotFound($"Featured vehicle with id {id} not found"));
            }

            return Ok(ApiResponse<bool>.Success(true, "Featured vehicle deactivated successfully"));
        }

        [HttpPost("bulk/priority")]
        public async Task<ActionResult<ApiResponse<bool>>> BulkUpdatePriority(
            [FromBody] Dictionary<string, int> priorityUpdates)
        {
            _logger.LogInformation("Bulk updating priorities for {Count} featured vehicles", priorityUpdates.Count);

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
            _logger.LogInformation("User liked featured vehicle: {Id}", id);

            var result = await _service.IncrementLikesAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.NotFound($"Featured vehicle with id {id} not found"));
            }

            return Ok(ApiResponse<bool>.Success(true));
        }

        [HttpPost("{id}/share")]
        public async Task<ActionResult<ApiResponse<bool>>> Share(string id)
        {
            _logger.LogInformation("User shared featured vehicle: {Id}", id);

            var result = await _service.IncrementSharesAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.NotFound($"Featured vehicle with id {id} not found"));
            }

            return Ok(ApiResponse<bool>.Success(true));
        }

        [HttpPost("{id}/enquire")]
        public async Task<ActionResult<ApiResponse<bool>>> Enquire(string id)
        {
            _logger.LogInformation("User made enquiry for featured vehicle: {Id}", id);

            var result = await _service.IncrementEnquiriesAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.NotFound($"Featured vehicle with id {id} not found"));
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

            _logger.LogInformation("Adding rating {Rating} for featured vehicle: {Id}", rating.Rating, id);

            try
            {
                var result = await _service.AddRatingAsync(id, rating);
                if (!result)
                {
                    return NotFound(ApiResponse<bool>.NotFound($"Featured vehicle with id {id} not found"));
                }

                return Ok(ApiResponse<bool>.Success(true, "Rating added successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<bool>.Error(ex.Message));
            }
        }

    }
}