using AutoNext.Platform.Listings.API.Models;
using AutoNext.Platform.Listings.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AutoNext.Platform.Listings.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class AnalyticsController : ControllerBase
    {
        private readonly IListingAnalyticsService _analyticsService;
        private readonly ILogger<AnalyticsController> _logger;

        public AnalyticsController(IListingAnalyticsService analyticsService, ILogger<AnalyticsController> logger)
        {
            _analyticsService = analyticsService;
            _logger = logger;
        }

        [HttpGet("vehicle/{vehicleId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetVehicleAnalytics(string vehicleId)
        {
            _logger.LogInformation("Getting analytics for vehicle: {VehicleId}", vehicleId);

            var analytics = await _analyticsService.GetVehicleAnalyticsAsync(vehicleId);
            return Ok(ApiResponse<VehicleAnalytics>.Ok(analytics, "Analytics retrieved successfully"));
        }

        [HttpGet("dashboard/admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminDashboard()
        {
            _logger.LogInformation("Getting admin dashboard data");

            var dashboardData = await _analyticsService.GetAdminDashboardDataAsync();
            return Ok(ApiResponse<AdminDashboardData>.Ok(dashboardData, "Dashboard data retrieved successfully"));
        }

        [HttpGet("dashboard/seller")]
        public async Task<IActionResult> GetSellerDashboard()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _logger.LogInformation("Getting seller dashboard for: {UserId}", userId);

            var dashboardData = await _analyticsService.GetSellerDashboardDataAsync(userId ?? "unknown");
            return Ok(ApiResponse<SellerDashboardData>.Ok(dashboardData, "Dashboard data retrieved successfully"));
        }

        [HttpGet("performance")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPerformanceMetrics()
        {
            _logger.LogInformation("Getting overall performance metrics");

            var metrics = await _analyticsService.GetOverallPerformanceMetricsAsync();
            return Ok(ApiResponse<PerformanceMetrics>.Ok(metrics, "Performance metrics retrieved successfully"));
        }

        [HttpGet("trends/views")]
        [AllowAnonymous]
        public async Task<IActionResult> GetViewTrends([FromQuery] int days = 30)
        {
            _logger.LogInformation("Getting view trends for last {Days} days", days);

            var trends = await _analyticsService.GetViewTrendsAsync(days);
            return Ok(ApiResponse<IEnumerable<TrendData>>.Ok(trends, "View trends retrieved successfully"));
        }

        [HttpGet("trends/inquiries")]
        [AllowAnonymous]
        public async Task<IActionResult> GetInquiryTrends([FromQuery] int days = 30)
        {
            _logger.LogInformation("Getting inquiry trends for last {Days} days", days);

            var trends = await _analyticsService.GetInquiryTrendsAsync(days);
            return Ok(ApiResponse<IEnumerable<TrendData>>.Ok(trends, "Inquiry trends retrieved successfully"));
        }

        [HttpGet("popular-makes")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPopularMakes([FromQuery] int limit = 10)
        {
            _logger.LogInformation("Getting popular makes, limit: {Limit}", limit);

            var popularMakes = await _analyticsService.GetPopularMakesAsync(limit);
            return Ok(ApiResponse<Dictionary<string, int>>.Ok(popularMakes, "Popular makes retrieved successfully"));
        }

        [HttpGet("popular-models")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPopularModels([FromQuery] string? make = null, [FromQuery] int limit = 10)
        {
            _logger.LogInformation("Getting popular models for make: {Make}, limit: {Limit}", make, limit);

            var popularModels = await _analyticsService.GetPopularModelsAsync(make, limit);
            return Ok(ApiResponse<Dictionary<string, int>>.Ok(popularModels, "Popular models retrieved successfully"));
        }

        [HttpGet("top-cities")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTopCities([FromQuery] int limit = 10)
        {
            _logger.LogInformation("Getting top cities, limit: {Limit}", limit);

            var topCities = await _analyticsService.GetTopCitiesByListingsAsync(limit);
            return Ok(ApiResponse<Dictionary<string, int>>.Ok(topCities, "Top cities retrieved successfully"));
        }

        [HttpGet("location/{city}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCityAnalytics(string city)
        {
            _logger.LogInformation("Getting analytics for city: {City}", city);

            // This would be implemented based on your requirements
            return Ok(ApiResponse<object>.Ok(null, "City analytics retrieved successfully"));
        }

        [HttpPost("track/view/{vehicleId}")]
        [AllowAnonymous]
        public async Task<IActionResult> TrackView(string vehicleId, [FromBody] TrackViewRequest? request = null)
        {
            _logger.LogInformation("Tracking view for vehicle: {VehicleId}", vehicleId);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            var result = await _analyticsService.TrackVehicleViewAsync(vehicleId, userId, ipAddress);

            if (!result)
            {
                return NotFound(ApiResponse<object>.Error("Vehicle not found", 404));
            }

            return Ok(ApiResponse<object>.Ok(null, "View tracked successfully"));
        }

        [HttpPost("track/inquiry/{vehicleId}")]
        public async Task<IActionResult> TrackInquiry(string vehicleId, [FromBody] TrackInquiryRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _logger.LogInformation("Tracking inquiry for vehicle: {VehicleId} by user: {UserId}", vehicleId, userId);

            var result = await _analyticsService.TrackVehicleInquiryAsync(vehicleId, userId ?? "anonymous", request.InquiryType);

            if (!result)
            {
                return NotFound(ApiResponse<object>.Error("Vehicle not found", 404));
            }

            return Ok(ApiResponse<object>.Ok(null, "Inquiry tracked successfully"));
        }

        [HttpGet("report")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GenerateReport(
            [FromQuery] ReportType reportType,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] ExportFormat format = ExportFormat.PDF)
        {
            _logger.LogInformation("Generating {ReportType} report from {StartDate} to {EndDate}", reportType, startDate, endDate);

            var report = await _analyticsService.GenerateAnalyticsReportAsync(reportType, startDate, endDate, format);

            var contentType = format switch
            {
                ExportFormat.CSV => "text/csv",
                ExportFormat.JSON => "application/json",
                ExportFormat.PDF => "application/pdf",
                ExportFormat.Excel => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "application/octet-stream"
            };

            var fileName = $"analytics_report_{reportType}_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.{format.ToString().ToLower()}";

            return File(report, contentType, fileName);
        }
    }
}
