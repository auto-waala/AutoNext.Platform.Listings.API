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
    public class PremiumVehicleController : ControllerBase
    {
        private readonly IPremiumVehicleService _service;
        private readonly ILogger<PremiumVehicleController> _logger;

        public PremiumVehicleController(IPremiumVehicleService service, ILogger<PremiumVehicleController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // GET Endpoints

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<PremiumVehicleResponseDto>>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null)
        {
            _logger.LogInformation("Getting all premium vehicles - Page: {Page}, PageSize: {PageSize}", page, pageSize);

            var result = await _service.GetAllAsync(page, pageSize, sortBy, sortOrder);
            return Ok(ApiResponse<PagedResult<PremiumVehicleResponseDto>>.Success(result));
        }

        [HttpGet("active")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PremiumVehicleSummaryDto>>>> GetActivePremiumVehicles(
            [FromQuery] int limit = 50)
        {
            _logger.LogInformation("Getting active premium vehicles with limit: {Limit}", limit);

            var vehicles = await _service.GetActivePremiumVehiclesAsync(limit);
            return Ok(ApiResponse<IEnumerable<PremiumVehicleSummaryDto>>.Success(vehicles));
        }

        [HttpGet("top-priority")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PremiumVehicleSummaryDto>>>> GetTopPriorityPremiumVehicles(
            [FromQuery] int limit = 10)
        {
            _logger.LogInformation("Getting top priority premium vehicles with limit: {Limit}", limit);

            var vehicles = await _service.GetTopPriorityPremiumVehiclesAsync(limit);
            return Ok(ApiResponse<IEnumerable<PremiumVehicleSummaryDto>>.Success(vehicles));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<PremiumVehicleResponseDto>>> GetById(string id)
        {
            _logger.LogInformation("Getting premium vehicle by id: {Id}", id);

            var vehicle = await _service.GetByIdAsync(id);
            if (vehicle == null)
            {
                return NotFound(ApiResponse<PremiumVehicleResponseDto>.NotFound($"Premium vehicle with id {id} not found"));
            }

            _ = Task.Run(() => _service.IncrementViewsAsync(id));

            return Ok(ApiResponse<PremiumVehicleResponseDto>.Success(vehicle));
        }

        [HttpGet("slug/{slug}")]
        public async Task<ActionResult<ApiResponse<PremiumVehicleResponseDto>>> GetBySlug(string slug)
        {
            _logger.LogInformation("Getting premium vehicle by slug: {Slug}", slug);

            var vehicle = await _service.GetBySlugAsync(slug);
            if (vehicle == null)
            {
                return NotFound(ApiResponse<PremiumVehicleResponseDto>.NotFound($"Premium vehicle with slug {slug} not found"));
            }

            _ = Task.Run(() => _service.IncrementViewsAsync(vehicle.Id));

            return Ok(ApiResponse<PremiumVehicleResponseDto>.Success(vehicle));
        }

        [HttpGet("model/{modelSlug}")]
        public async Task<ActionResult<ApiResponse<PremiumVehicleResponseDto>>> GetByModelSlug(string modelSlug)
        {
            _logger.LogInformation("Getting premium vehicle by model slug: {ModelSlug}", modelSlug);

            var vehicle = await _service.GetByModelSlugAsync(modelSlug);
            if (vehicle == null)
            {
                return NotFound(ApiResponse<PremiumVehicleResponseDto>.NotFound($"Premium vehicle with model slug {modelSlug} not found"));
            }

            return Ok(ApiResponse<PremiumVehicleResponseDto>.Success(vehicle));
        }

        // Filter Endpoints

        [HttpGet("brand/{brandName}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PremiumVehicleSummaryDto>>>> GetByBrand(
            string brandName,
            [FromQuery] int limit = 20)
        {
            _logger.LogInformation("Getting premium vehicles by brand: {BrandName}, Limit: {Limit}", brandName, limit);

            var vehicles = await _service.GetByBrandAsync(brandName, limit);
            return Ok(ApiResponse<IEnumerable<PremiumVehicleSummaryDto>>.Success(vehicles));
        }

        [HttpGet("type/{vehicleType}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PremiumVehicleSummaryDto>>>> GetByVehicleType(
            string vehicleType,
            [FromQuery] int limit = 20)
        {
            _logger.LogInformation("Getting premium vehicles by type: {VehicleType}, Limit: {Limit}", vehicleType, limit);

            var vehicles = await _service.GetByVehicleTypeAsync(vehicleType, limit);
            return Ok(ApiResponse<IEnumerable<PremiumVehicleSummaryDto>>.Success(vehicles));
        }

        [HttpGet("price-range")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PremiumVehicleSummaryDto>>>> GetByPriceRange(
            [FromQuery] decimal minPrice,
            [FromQuery] decimal maxPrice,
            [FromQuery] int limit = 20)
        {
            _logger.LogInformation("Getting premium vehicles by price range: {MinPrice} - {MaxPrice}, Limit: {Limit}", minPrice, maxPrice, limit);

            var vehicles = await _service.GetByPriceRangeAsync(minPrice, maxPrice, limit);
            return Ok(ApiResponse<IEnumerable<PremiumVehicleSummaryDto>>.Success(vehicles));
        }

        [HttpGet("city/{city}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PremiumVehicleSummaryDto>>>> GetByCity(
            string city,
            [FromQuery] int limit = 20)
        {
            _logger.LogInformation("Getting premium vehicles by city: {City}, Limit: {Limit}", city, limit);

            var vehicles = await _service.GetByCityAsync(city, limit);
            return Ok(ApiResponse<IEnumerable<PremiumVehicleSummaryDto>>.Success(vehicles));
        }

        [HttpGet("search")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PremiumVehicleSummaryDto>>>> Search(
            [FromQuery] string q,
            [FromQuery] int limit = 20)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return BadRequest(ApiResponse<IEnumerable<PremiumVehicleSummaryDto>>.Error("Search term cannot be empty"));
            }

            _logger.LogInformation("Searching premium vehicles with term: {SearchTerm}, Limit: {Limit}", q, limit);

            var vehicles = await _service.SearchAsync(q, limit);
            return Ok(ApiResponse<IEnumerable<PremiumVehicleSummaryDto>>.Success(vehicles));
        }

        // CRUD Endpoints

        [HttpPost]
        public async Task<ActionResult<ApiResponse<PremiumVehicleResponseDto>>> Create([FromBody] PremiumVehicleRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<PremiumVehicleResponseDto>.Error("Invalid request data"));
            }

            _logger.LogInformation("Creating premium vehicle for brand: {BrandName}, model: {ModelName}",
                request.BrandName, request.ModelName);

            try
            {
                var vehicle = await _service.CreateAsync(request);
                return Ok(ApiResponse<PremiumVehicleResponseDto>.Success(vehicle, "Premium vehicle created successfully"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation error while creating premium vehicle");
                return Conflict(ApiResponse<PremiumVehicleResponseDto>.Error(ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<PremiumVehicleResponseDto>>> Update(string id, [FromBody] PremiumVehicleRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<PremiumVehicleResponseDto>.Error("Invalid request data"));
            }

            _logger.LogInformation("Updating premium vehicle with id: {Id}", id);

            try
            {
                var vehicle = await _service.UpdateAsync(id, request);
                if (vehicle == null)
                {
                    return NotFound(ApiResponse<PremiumVehicleResponseDto>.NotFound($"Premium vehicle with id {id} not found"));
                }

                return Ok(ApiResponse<PremiumVehicleResponseDto>.Success(vehicle, "Premium vehicle updated successfully"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation error while updating premium vehicle");
                return Conflict(ApiResponse<PremiumVehicleResponseDto>.Error(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(string id)
        {
            _logger.LogInformation("Deleting premium vehicle with id: {Id}", id);

            var result = await _service.DeleteAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.NotFound($"Premium vehicle with id {id} not found"));
            }

            return Ok(ApiResponse<bool>.Success(true, "Premium vehicle deleted successfully"));
        }

        // Premium-specific Operations

        [HttpPatch("{id}/priority")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdatePriority(
            string id,
            [FromBody] UpdatePremiumVehiclePriorityDto request)
        {
            _logger.LogInformation("Updating priority for premium vehicle {Id} to {Priority}", id, request.Priority);

            try
            {
                var result = await _service.UpdatePriorityAsync(id, request.Priority);
                if (!result)
                {
                    return NotFound(ApiResponse<bool>.NotFound($"Premium vehicle with id {id} not found"));
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
            [FromBody] ActivatePremiumVehicleRequest? request = null)
        {
            _logger.LogInformation("Activating premium vehicle with id: {Id}", id);

            var result = await _service.ActivateAsync(id, request?.EndDate);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.NotFound($"Premium vehicle with id {id} not found"));
            }

            return Ok(ApiResponse<bool>.Success(true, "Premium vehicle activated successfully"));
        }

        [HttpPost("{id}/deactivate")]
        public async Task<ActionResult<ApiResponse<bool>>> Deactivate(string id)
        {
            _logger.LogInformation("Deactivating premium vehicle with id: {Id}", id);

            var result = await _service.DeactivateAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.NotFound($"Premium vehicle with id {id} not found"));
            }

            return Ok(ApiResponse<bool>.Success(true, "Premium vehicle deactivated successfully"));
        }

        [HttpPost("bulk/priority")]
        public async Task<ActionResult<ApiResponse<bool>>> BulkUpdatePriority(
            [FromBody] Dictionary<string, int> priorityUpdates)
        {
            _logger.LogInformation("Bulk updating priorities for {Count} premium vehicles", priorityUpdates.Count);

            var result = await _service.BulkUpdatePriorityAsync(priorityUpdates);
            if (!result)
            {
                return BadRequest(ApiResponse<bool>.Error("Failed to update priorities"));
            }

            return Ok(ApiResponse<bool>.Success(true, "Priorities updated successfully"));
        }

        // Engagement Endpoints

        [HttpPost("{id}/like")]
        public async Task<ActionResult<ApiResponse<bool>>> Like(string id)
        {
            _logger.LogInformation("User liked premium vehicle: {Id}", id);

            var result = await _service.IncrementLikesAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.NotFound($"Premium vehicle with id {id} not found"));
            }

            return Ok(ApiResponse<bool>.Success(true));
        }

        [HttpPost("{id}/share")]
        public async Task<ActionResult<ApiResponse<bool>>> Share(string id)
        {
            _logger.LogInformation("User shared premium vehicle: {Id}", id);

            var result = await _service.IncrementSharesAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.NotFound($"Premium vehicle with id {id} not found"));
            }

            return Ok(ApiResponse<bool>.Success(true));
        }

        [HttpPost("{id}/enquire")]
        public async Task<ActionResult<ApiResponse<bool>>> Enquire(string id)
        {
            _logger.LogInformation("User made enquiry for premium vehicle: {Id}", id);

            var result = await _service.IncrementEnquiriesAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.NotFound($"Premium vehicle with id {id} not found"));
            }

            return Ok(ApiResponse<bool>.Success(true));
        }

        // Rating Endpoints

        [HttpPost("{id}/rating")]
        public async Task<ActionResult<ApiResponse<bool>>> AddRating(
     string id,
     [FromBody] PremiumVehicleUserRatingDto rating)
        {
            if (rating.Rating < 1 || rating.Rating > 5)
            {
                return BadRequest(ApiResponse<bool>.Error("Rating must be between 1 and 5"));
            }

            _logger.LogInformation("Adding rating {Rating} for premium vehicle: {Id}", rating.Rating, id);

            try
            {
                var result = await _service.AddRatingAsync(id, rating);

                if (!result)
                {
                    return NotFound(ApiResponse<bool>.NotFound($"Premium vehicle with id {id} not found"));
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
