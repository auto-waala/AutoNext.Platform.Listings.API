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
    public class NewlyArrivedController : ControllerBase
    {
        private readonly INewlyArrivedService _service;
        private readonly ILogger<NewlyArrivedController> _logger;

        public NewlyArrivedController(INewlyArrivedService service, ILogger<NewlyArrivedController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<NewlyArrivedResponseDto>>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Getting all newly arrived vehicles - Page: {Page}, PageSize: {PageSize}", page, pageSize);

            var result = await _service.GetAllAsync(page, pageSize);

            _logger.LogInformation("Retrieved {Count} newly arrived vehicles", result.TotalCount);
            return Ok(ApiResponse<PagedResult<NewlyArrivedResponseDto>>.Success(result));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<NewlyArrivedResponseDto>>> GetById(string id)
        {
            _logger.LogInformation("Getting newly arrived vehicle by id: {Id}", id);

            var vehicle = await _service.GetByIdAsync(id);
            if (vehicle == null)
            {
                _logger.LogWarning("Newly arrived vehicle not found with id: {Id}", id);
                return NotFound(ApiResponse<NewlyArrivedResponseDto>.NotFound());
            }

            _logger.LogInformation("Successfully retrieved newly arrived vehicle with id: {Id}", id);
            return Ok(ApiResponse<NewlyArrivedResponseDto>.Success(vehicle));
        }

        [HttpGet("slug/{modelSlug}")]
        public async Task<ActionResult<ApiResponse<NewlyArrivedResponseDto>>> GetByModelSlug(string modelSlug)
        {
            _logger.LogInformation("Getting newly arrived vehicle by slug: {ModelSlug}", modelSlug);

            var vehicle = await _service.GetByModelSlugAsync(modelSlug);
            if (vehicle == null)
            {
                _logger.LogWarning("Newly arrived vehicle not found with slug: {ModelSlug}", modelSlug);
                return NotFound(ApiResponse<NewlyArrivedResponseDto>.NotFound());
            }

            _logger.LogInformation("Successfully retrieved newly arrived vehicle with slug: {ModelSlug}", modelSlug);
            return Ok(ApiResponse<NewlyArrivedResponseDto>.Success(vehicle));
        }

        [HttpGet("weekly")]
        public async Task<ActionResult<ApiResponse<IEnumerable<NewlyArrivedResponseDto>>>> GetWeeklyArrivals(
            [FromQuery] int limit = 20)
        {
            _logger.LogInformation("Getting weekly arrivals with limit: {Limit}", limit);

            var vehicles = await _service.GetWeeklyArrivalsAsync(limit);

            _logger.LogInformation("Successfully retrieved weekly arrivals");
            return Ok(ApiResponse<IEnumerable<NewlyArrivedResponseDto>>.Success(vehicles));
        }

        [HttpGet("monthly")]
        public async Task<ActionResult<ApiResponse<IEnumerable<NewlyArrivedResponseDto>>>> GetMonthlyArrivals(
            [FromQuery] int month = 0,
            [FromQuery] int year = 0,
            [FromQuery] int limit = 20)
        {
            var targetMonth = month > 0 ? month : DateTime.UtcNow.Month;
            var targetYear = year > 0 ? year : DateTime.UtcNow.Year;

            _logger.LogInformation("Getting monthly arrivals for Month: {Month}, Year: {Year}, Limit: {Limit}",
                targetMonth, targetYear, limit);

            var vehicles = await _service.GetMonthlyArrivalsAsync(targetMonth, targetYear, limit);

            _logger.LogInformation("Successfully retrieved monthly arrivals for {Month}/{Year}", targetMonth, targetYear);
            return Ok(ApiResponse<IEnumerable<NewlyArrivedResponseDto>>.Success(vehicles));
        }

        [HttpGet("yearly/{year}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<NewlyArrivedResponseDto>>>> GetYearlyArrivals(int year)
        {
            _logger.LogInformation("Getting yearly arrivals for Year: {Year}", year);

            var vehicles = await _service.GetYearlyArrivalsAsync(year);

            _logger.LogInformation("Successfully retrieved yearly arrivals for {Year}", year);
            return Ok(ApiResponse<IEnumerable<NewlyArrivedResponseDto>>.Success(vehicles));
        }

        [HttpGet("yearly/current")]
        public async Task<ActionResult<ApiResponse<IEnumerable<NewlyArrivedResponseDto>>>> GetCurrentYearArrivals()
        {
            var currentYear = DateTime.UtcNow.Year;

            _logger.LogInformation("Getting current year arrivals for Year: {Year}", currentYear);

            var vehicles = await _service.GetYearlyArrivalsAsync(currentYear);

            _logger.LogInformation("Successfully retrieved current year arrivals for {Year}", currentYear);
            return Ok(ApiResponse<IEnumerable<NewlyArrivedResponseDto>>.Success(vehicles));
        }

        [HttpGet("featured")]
        public async Task<ActionResult<ApiResponse<IEnumerable<NewlyArrivedResponseDto>>>> GetFeaturedArrivals(
            [FromQuery] int limit = 10)
        {
            _logger.LogInformation("Getting featured arrivals with limit: {Limit}", limit);

            var vehicles = await _service.GetFeaturedArrivalsAsync(limit);

            _logger.LogInformation("Successfully retrieved featured arrivals");
            return Ok(ApiResponse<IEnumerable<NewlyArrivedResponseDto>>.Success(vehicles));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<NewlyArrivedResponseDto>>> Create(
            [FromBody] NewlyArrivedRequestDto request)
        {
            var publishedBy = User.Identity?.Name ?? "admin";

            _logger.LogInformation("Creating newly arrived vehicle, PublishedBy: {PublishedBy}", publishedBy);

            var vehicle = await _service.CreateAsync(request, publishedBy);

            _logger.LogInformation("Successfully created newly arrived vehicle with id: {Id}", vehicle.Id);
            return Ok(ApiResponse<NewlyArrivedResponseDto>.Success(vehicle, "Vehicle created successfully"));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<NewlyArrivedResponseDto>>> Update(
            string id,
            [FromBody] NewlyArrivedRequestDto request)
        {
            _logger.LogInformation("Updating newly arrived vehicle with id: {Id}", id);

            var vehicle = await _service.UpdateAsync(id, request);
            if (vehicle == null)
            {
                _logger.LogWarning("Newly arrived vehicle not found for update with id: {Id}", id);
                return NotFound(ApiResponse<NewlyArrivedResponseDto>.NotFound());
            }

            _logger.LogInformation("Successfully updated newly arrived vehicle with id: {Id}", id);
            return Ok(ApiResponse<NewlyArrivedResponseDto>.Success(vehicle, "Vehicle updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(string id)
        {
            _logger.LogInformation("Deleting newly arrived vehicle with id: {Id}", id);

            var result = await _service.DeleteAsync(id);
            if (!result)
            {
                _logger.LogWarning("Newly arrived vehicle not found for deletion with id: {Id}", id);
                return NotFound(ApiResponse<bool>.NotFound());
            }

            _logger.LogInformation("Successfully deleted newly arrived vehicle with id: {Id}", id);
            return Ok(ApiResponse<bool>.Success(true, "Vehicle deleted successfully"));
        }

        [HttpPost("{id}/publish")]
        public async Task<ActionResult<ApiResponse<bool>>> Publish(string id)
        {
            var publishedBy = User.Identity?.Name ?? "admin";

            _logger.LogInformation("Publishing newly arrived vehicle with id: {Id}, PublishedBy: {PublishedBy}", id, publishedBy);

            var result = await _service.PublishAsync(id, publishedBy);
            if (!result)
            {
                _logger.LogWarning("Newly arrived vehicle not found for publishing with id: {Id}", id);
                return NotFound(ApiResponse<bool>.NotFound());
            }

            _logger.LogInformation("Successfully published newly arrived vehicle with id: {Id}", id);
            return Ok(ApiResponse<bool>.Success(true, "Vehicle published successfully"));
        }

        [HttpPost("{id}/unpublish")]
        public async Task<ActionResult<ApiResponse<bool>>> Unpublish(string id)
        {
            _logger.LogInformation("Unpublishing newly arrived vehicle with id: {Id}", id);

            var result = await _service.UnpublishAsync(id);
            if (!result)
            {
                _logger.LogWarning("Newly arrived vehicle not found for unpublishing with id: {Id}", id);
                return NotFound(ApiResponse<bool>.NotFound());
            }

            _logger.LogInformation("Successfully unpublished newly arrived vehicle with id: {Id}", id);
            return Ok(ApiResponse<bool>.Success(true, "Vehicle unpublished successfully"));
        }
    }
}