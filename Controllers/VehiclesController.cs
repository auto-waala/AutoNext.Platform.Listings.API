using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using AutoNext.Platform.Listings.API.Models;
using AutoNext.Platform.Listings.API.Services;
using global::AutoNext.Platform.Listings.API.Models;
using global::AutoNext.Platform.Listings.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AutoNext.Platform.Listings.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;
        private readonly IVehicleSearchService _vehicleSearchService;
        private readonly ILogger<VehiclesController> _logger;

        public VehiclesController(
            IVehicleService vehicleService,
            IVehicleSearchService vehicleSearchService,
            ILogger<VehiclesController> logger)
        {
            _vehicleService = vehicleService;
            _vehicleSearchService = vehicleSearchService;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetVehicles([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            _logger.LogInformation("Getting vehicles - Page: {Page}, PageSize: {PageSize}", page, pageSize);

            var vehicles = await _vehicleService.GetAllActiveVehiclesAsync(page, pageSize);
            var totalCount = await _vehicleService.GetTotalActiveVehiclesCountAsync();

            Response.Headers.Append("X-Total-Count", totalCount.ToString());
            Response.Headers.Append("X-Page", page.ToString());
            Response.Headers.Append("X-Page-Size", pageSize.ToString());
            Response.Headers.Append("X-Total-Pages", Math.Ceiling(totalCount / (double)pageSize).ToString());

            return Ok(ApiResponse<IEnumerable<Vehicle>>.Ok(vehicles, "Vehicles retrieved successfully"));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetVehicleById(string id)
        {
            _logger.LogInformation("Getting vehicle by ID: {Id}", id);

            try
            {
                var vehicle = await _vehicleService.GetVehicleByIdAsync(id);

                // Track view
                await _vehicleService.RecordVehicleViewAsync(id);

                return Ok(ApiResponse<Vehicle>.Ok(vehicle, "Vehicle retrieved successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Vehicle not found: {Id}", id);
                return NotFound(ApiResponse<object>.Error("Vehicle not found", 404));
            }
        }

        [HttpGet("vin/{vin}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetVehicleByVIN(string vin)
        {
            _logger.LogInformation("Getting vehicle by VIN: {Vin}", vin);

            try
            {
                var vehicle = await _vehicleService.GetVehicleByVINAsync(vin);
                return Ok(ApiResponse<Vehicle>.Ok(vehicle, "Vehicle retrieved successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Vehicle not found with VIN: {Vin}", vin);
                return NotFound(ApiResponse<object>.Error("Vehicle not found", 404));
            }
        }

        [HttpGet("chassis/{chassisNumber}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetVehicleByChassisNumber(string chassisNumber)
        {
            _logger.LogInformation("Getting vehicle by Chassis Number: {ChassisNumber}", chassisNumber);

            try
            {
                var vehicle = await _vehicleService.GetVehicleByChassisNumberAsync(chassisNumber);
                return Ok(ApiResponse<Vehicle>.Ok(vehicle, "Vehicle retrieved successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Vehicle not found with Chassis Number: {ChassisNumber}", chassisNumber);
                return NotFound(ApiResponse<object>.Error("Vehicle not found", 404));
            }
        }

        [HttpGet("engine/{engineNumber}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetVehicleByEngineNumber(string engineNumber)
        {
            _logger.LogInformation("Getting vehicle by Engine Number: {EngineNumber}", engineNumber);

            try
            {
                var vehicle = await _vehicleService.GetVehicleByEngineNumberAsync(engineNumber);
                return Ok(ApiResponse<Vehicle>.Ok(vehicle, "Vehicle retrieved successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Vehicle not found with Engine Number: {EngineNumber}", engineNumber);
                return NotFound(ApiResponse<object>.Error("Vehicle not found", 404));
            }
        }

        [HttpGet("seller/{sellerId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetVehiclesBySeller(string sellerId)
        {
            _logger.LogInformation("Getting vehicles by seller: {SellerId}", sellerId);

            var vehicles = await _vehicleService.GetVehiclesBySellerAsync(sellerId);
            return Ok(ApiResponse<IEnumerable<Vehicle>>.Ok(vehicles, "Vehicles retrieved successfully"));
        }

        [HttpPost]
        public async Task<IActionResult> CreateVehicle([FromBody] CreateVehicleRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _logger.LogInformation("Create vehicle attempt by user: {UserId}", userId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Error("Invalid request", 400,
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            try
            {
                var vehicle = MapToVehicle(request);
                var result = await _vehicleService.CreateVehicleAsync(vehicle, userId ?? "system");

                _logger.LogInformation("Vehicle created successfully: {VehicleId} by {UserId}", result.VehicleId, userId);
                return Ok(ApiResponse<Vehicle>.Ok(result, "Vehicle created successfully"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Vehicle creation failed");
                return BadRequest(ApiResponse<object>.Error(ex.Message, 400));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVehicle(string id, [FromBody] UpdateVehicleRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _logger.LogInformation("Update vehicle attempt for {Id} by {UserId}", id, userId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Error("Invalid request", 400,
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            try
            {
                var vehicle = MapToVehicle(request);
                vehicle.Id = id;

                var result = await _vehicleService.UpdateVehicleAsync(id, vehicle, userId ?? "system");

                _logger.LogInformation("Vehicle updated successfully: {Id} by {UserId}", id, userId);
                return Ok(ApiResponse<Vehicle>.Ok(result, "Vehicle updated successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Vehicle not found for update: {Id}", id);
                return NotFound(ApiResponse<object>.Error("Vehicle not found", 404));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating vehicle: {Id}", id);
                return StatusCode(500, ApiResponse<object>.Error("Internal server error", 500));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _logger.LogInformation("Delete vehicle attempt for {Id} by {UserId}", id, userId);

            var result = await _vehicleService.SoftDeleteVehicleAsync(id, userId ?? "system");

            if (!result)
            {
                _logger.LogWarning("Vehicle not found for deletion: {Id}", id);
                return NotFound(ApiResponse<object>.Error("Vehicle not found", 404));
            }

            _logger.LogInformation("Vehicle deleted successfully: {Id} by {UserId}", id, userId);
            return Ok(ApiResponse<object>.Ok(null, "Vehicle deleted successfully"));
        }

        [HttpPost("{id}/activate")]
        public async Task<IActionResult> ActivateVehicle(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _logger.LogInformation("Activate vehicle attempt for {Id} by {UserId}", id, userId);

            var result = await _vehicleService.ActivateVehicleAsync(id, userId ?? "system");

            if (!result)
            {
                return NotFound(ApiResponse<object>.Error("Vehicle not found", 404));
            }

            return Ok(ApiResponse<object>.Ok(null, "Vehicle activated successfully"));
        }

        [HttpPost("{id}/deactivate")]
        public async Task<IActionResult> DeactivateVehicle(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _logger.LogInformation("Deactivate vehicle attempt for {Id} by {UserId}", id, userId);

            var result = await _vehicleService.DeactivateVehicleAsync(id, userId ?? "system");

            if (!result)
            {
                return NotFound(ApiResponse<object>.Error("Vehicle not found", 404));
            }

            return Ok(ApiResponse<object>.Ok(null, "Vehicle deactivated successfully"));
        }

        [HttpPost("{id}/mark-sold")]
        public async Task<IActionResult> MarkVehicleAsSold(string id, [FromBody] MarkAsSoldRequest? request = null)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _logger.LogInformation("Mark vehicle as sold attempt for {Id} by {UserId}", id, userId);

            var result = await _vehicleService.MarkVehicleAsSoldAsync(id, userId ?? "system", request?.SoldDate);

            if (!result)
            {
                return NotFound(ApiResponse<object>.Error("Vehicle not found", 404));
            }

            return Ok(ApiResponse<object>.Ok(null, "Vehicle marked as sold successfully"));
        }

        [HttpPost("{id}/extend")]
        public async Task<IActionResult> ExtendListing(string id, [FromBody] ExtendListingRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _logger.LogInformation("Extend listing for vehicle {Id} by {UserId}", id, userId);

            var result = await _vehicleService.ExtendVehicleListingAsync(id, request.AdditionalDays, userId ?? "system");

            if (!result)
            {
                return NotFound(ApiResponse<object>.Error("Vehicle not found", 404));
            }

            return Ok(ApiResponse<object>.Ok(null, $"Listing extended by {request.AdditionalDays} days"));
        }

        [HttpPost("{id}/favorite")]
        public async Task<IActionResult> AddToFavorites(string id)
        {
            _logger.LogInformation("Add to favorites: {Id}", id);

            var result = await _vehicleService.RecordVehicleFavoriteAsync(id);

            if (!result)
            {
                return NotFound(ApiResponse<object>.Error("Vehicle not found", 404));
            }

            return Ok(ApiResponse<object>.Ok(null, "Added to favorites"));
        }
    }

}
