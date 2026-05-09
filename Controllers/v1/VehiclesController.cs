using AutoNext.Platform.Listings.API.Models.Common;
using AutoNext.Platform.Listings.API.Models.DTOs;
using AutoNext.Platform.Listings.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AutoNext.Platform.Listings.API.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;
        private readonly ILogger<VehiclesController> _logger;

        public VehiclesController(IVehicleService vehicleService, ILogger<VehiclesController> logger)
        {
            _vehicleService = vehicleService;
            _logger = logger;
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("API v1 - Getting all vehicles - Page: {Page}, PageSize: {PageSize}", page, pageSize);

            pageSize = Math.Min(pageSize, 50);

            var result = await _vehicleService.GetAllAsync(page, pageSize);
            return Ok(ApiResponse<PagedResult<VehicleDto>>.Ok(result, "Vehicles retrieved successfully"));
        }

        [HttpGet("{id}")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetById(string id)
        {
            _logger.LogInformation("API v1 - Getting vehicle by id: {Id}", id);

            var vehicle = await _vehicleService.GetByIdAsync(id);
            if (vehicle == null)
                return NotFound(ApiResponse<VehicleDto>.NotFound("Vehicle not found"));

            return Ok(ApiResponse<VehicleDto>.Ok(vehicle, "Vehicle retrieved successfully"));
        }

        [HttpGet("search")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> Search([FromQuery] VehicleSearchRequest request)
        {
            _logger.LogInformation("API v1 - Searching vehicles with criteria: {@Request}", request);

            var vehicles = await _vehicleService.SearchAsync(request);

            return Ok(ApiResponse<PagedResult<VehicleDto>>.Ok(vehicles, "Search completed successfully"));
        }

        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> Create([FromBody] CreateVehicleRequest request)
        {
            _logger.LogInformation("API v1 - Creating new vehicle");

            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<VehicleDto>.Error("Invalid request", 400));

            var sellerId = "temp_seller_123";

            var vehicle = await _vehicleService.CreateAsync(request, sellerId);
            return CreatedAtAction(nameof(GetById), new { id = vehicle.Id, version = "1" },
                ApiResponse<VehicleDto>.Created(vehicle, "Vehicle created successfully"));
        }

        [HttpPut("{id}")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateVehicleRequest request)
        {
            _logger.LogInformation("API v1 - Updating vehicle: {Id}", id);

            var vehicle = await _vehicleService.UpdateAsync(id, request);
            if (vehicle == null)
                return NotFound(ApiResponse<VehicleDto>.NotFound("Vehicle not found"));

            return Ok(ApiResponse<VehicleDto>.Ok(vehicle, "Vehicle updated successfully"));
        }

        [HttpDelete("{id}")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> Delete(string id)
        {
            _logger.LogInformation("API v1 - Deleting vehicle: {Id}", id);

            var result = await _vehicleService.DeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse<object>.NotFound("Vehicle not found"));

            return Ok(ApiResponse<object>.Ok(null, "Vehicle deleted successfully"));
        }
    }
}