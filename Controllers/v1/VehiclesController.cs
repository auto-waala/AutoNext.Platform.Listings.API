using AutoNext.Platform.Listings.Models.Common;
using AutoNext.Platform.Listings.Models.DTOs;
using AutoNext.Platform.Listings.Services;
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

        /// <summary>
        /// Get all vehicles with pagination
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page (default: 10, max: 50)</param>
        /// <returns>List of vehicles</returns>
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("API v1 - Getting all vehicles - Page: {Page}, PageSize: {PageSize}", page, pageSize);

            // Limit page size to 50
            pageSize = Math.Min(pageSize, 50);

            var result = await _vehicleService.GetAllAsync(page, pageSize);
            return Ok(ApiResponse<PagedResult<VehicleDto>>.Ok(result, "Vehicles retrieved successfully"));
        }

        /// <summary>
        /// Get vehicle by ID
        /// </summary>
        /// <param name="id">Vehicle ID</param>
        /// <returns>Vehicle details</returns>
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

        /// <summary>
        /// Search vehicles
        /// </summary>
        /// <param name="request">Search criteria</param>
        /// <returns>Matching vehicles</returns>
        [HttpGet("search")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> Search([FromQuery] VehicleSearchRequest request)
        {
            _logger.LogInformation("API v1 - Searching vehicles with criteria: {@Request}", request);

            var vehicles = await _vehicleService.SearchAsync(request);

            return Ok(ApiResponse<PagedResult<VehicleDto>>.Ok(vehicles, "Search completed successfully"));
        }

        /// <summary>
        /// Create a new vehicle listing
        /// </summary>
        /// <param name="request">Vehicle details</param>
        /// <returns>Created vehicle</returns>
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> Create([FromBody] CreateVehicleRequest request)
        {
            _logger.LogInformation("API v1 - Creating new vehicle");

            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<VehicleDto>.Error("Invalid request", 400));

            // In real app, get seller ID from JWT token
            var sellerId = "temp_seller_123";

            var vehicle = await _vehicleService.CreateAsync(request, sellerId);
            return CreatedAtAction(nameof(GetById), new { id = vehicle.Id, version = "1" },
                ApiResponse<VehicleDto>.Created(vehicle, "Vehicle created successfully"));
        }

        /// <summary>
        /// Update an existing vehicle
        /// </summary>
        /// <param name="id">Vehicle ID</param>
        /// <param name="request">Updated vehicle details</param>
        /// <returns>Updated vehicle</returns>
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

        /// <summary>
        /// Delete a vehicle
        /// </summary>
        /// <param name="id">Vehicle ID</param>
        /// <returns>Success message</returns>
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
