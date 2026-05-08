using AutoNext.Platform.Listings.API.Models;

namespace AutoNext.Platform.Listings.API.Services
{
    public interface IListingAnalyticsService
    {
        // Vehicle Analytics
        Task<VehicleAnalytics> GetVehicleAnalyticsAsync(string vehicleId);
        Task<bool> TrackVehicleViewAsync(string vehicleId, string? userId = null, string? ipAddress = null);
        Task<bool> TrackVehicleClickAsync(string vehicleId, string? userId = null, string? source = null);
        Task<bool> TrackVehicleInquiryAsync(string vehicleId, string userId, InquiryType inquiryType);

        // Daily/Monthly Analytics
        Task<DailyAnalytics> GetDailyAnalyticsAsync(DateTime date);
        Task<MonthlyAnalytics> GetMonthlyAnalyticsAsync(int year, int month);
        Task<IEnumerable<DailyAnalytics>> GetAnalyticsRangeAsync(DateTime startDate, DateTime endDate);

        // Performance Metrics
        Task<PerformanceMetrics> GetOverallPerformanceMetricsAsync();
        Task<SellerPerformanceMetrics> GetSellerPerformanceMetricsAsync(string sellerId);
        Task<CategoryPerformanceMetrics> GetCategoryPerformanceMetricsAsync(string categoryId);

        // Trends and Insights
        Task<IEnumerable<TrendData>> GetViewTrendsAsync(int days = 30);
        Task<IEnumerable<TrendData>> GetInquiryTrendsAsync(int days = 30);
        Task<Dictionary<string, int>> GetPopularSearchesAsync(int limit = 20);
        Task<Dictionary<string, int>> GetPopularMakesAsync(int limit = 10);
        Task<Dictionary<string, int>> GetPopularModelsAsync(string? make = null, int limit = 10);

        // Conversion Tracking
        Task<bool> TrackConversionAsync(string vehicleId, string userId, ConversionType conversionType);
        Task<ConversionMetrics> GetConversionMetricsAsync(DateTime startDate, DateTime endDate);

        // Geographic Analytics
        Task<IEnumerable<GeographicAnalytics>> GetViewsByLocationAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<string, int>> GetTopCitiesByListingsAsync(int limit = 10);

        // Real-time Analytics
        Task<RealTimeAnalytics> GetRealTimeAnalyticsAsync();
        Task<IEnumerable<ActiveUser>> GetActiveUsersAsync();

        // Reporting
        Task<byte[]> GenerateAnalyticsReportAsync(ReportType reportType, DateTime startDate, DateTime endDate, ExportFormat format = ExportFormat.PDF);
        Task ScheduleAnalyticsReportAsync(string userId, ReportSchedule schedule);

        // Dashboard Data
        Task<AdminDashboardData> GetAdminDashboardDataAsync();
        Task<SellerDashboardData> GetSellerDashboardDataAsync(string sellerId);

        // Alerts and Notifications
        Task CheckAndSendAnalyticsAlertsAsync();
        Task<IEnumerable<AnalyticsAlert>> GetActiveAlertsAsync();
    }
}