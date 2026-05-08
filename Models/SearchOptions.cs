// Models/SearchOptions.cs
namespace AutoNext.Platform.Listings.API.Models
{
    public class SearchOptions
    {
        public int Limit { get; set; } = 50;
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
        public bool IncludeInactive { get; set; } = false;
    }

    public class SearchResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public long TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class FacetedSearchResult
    {
        public IEnumerable<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public List<FacetCount> Facets { get; set; } = new List<FacetCount>();
    }

    public class FacetCount
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class SavedSearch
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public VehicleSearchCriteria Criteria { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }

    public class VehicleComparison
    {
        public List<Vehicle> Vehicles { get; set; } = new();
        public DateTime ComparisonDate { get; set; }
    }

    public enum ExportFormat
    {
        CSV,
        JSON,
        PDF,
        Excel
    }

    public enum InquiryType
    {
        Phone,
        Email,
        Chat,
        Visit
    }

    public enum ConversionType
    {
        TestDrive,
        Purchase,
        Booking,
        Contact
    }

    public enum ReportType
    {
        Daily,
        Weekly,
        Monthly,
        Quarterly,
        Yearly
    }

    public class DailyAnalytics
    {
        public DateTime Date { get; set; }
        public long TotalViews { get; set; }
        public long TotalInquiries { get; set; }
        public int TotalListings { get; set; }
        public int ActiveListings { get; set; }
    }

    public class MonthlyAnalytics
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public long TotalViews { get; set; }
        public long TotalInquiries { get; set; }
        public int TotalListings { get; set; }
        public int NewListings { get; set; }
        public int SoldVehicles { get; set; }
    }

    public class PerformanceMetrics
    {
        public long TotalListings { get; set; }
        public int ActiveListings { get; set; }
        public decimal AveragePrice { get; set; }
        public long TotalViews { get; set; }
        public long TotalInquiries { get; set; }
        public decimal ConversionRate { get; set; }
    }

    public class SellerPerformanceMetrics
    {
        public string SellerId { get; set; } = string.Empty;
        public int TotalListings { get; set; }
        public int ActiveListings { get; set; }
        public long TotalViews { get; set; }
        public long TotalInquiries { get; set; }
        public int AverageResponseTime { get; set; }
        public decimal Rating { get; set; }
    }

    public class CategoryPerformanceMetrics
    {
        public string CategoryId { get; set; } = string.Empty;
        public int TotalListings { get; set; }
        public decimal AveragePrice { get; set; }
        public long TotalViews { get; set; }
        public int AverageDaysToList { get; set; }
    }

    public class TrendData
    {
        public DateTime Date { get; set; }
        public long Value { get; set; }
    }

    public class ConversionMetrics
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long TotalViews { get; set; }
        public long TotalInquiries { get; set; }
        public long TotalConversions { get; set; }
        public decimal ConversionRate { get; set; }
        public decimal AverageTimeToConversion { get; set; }
    }

    public class GeographicAnalytics
    {
        public string City { get; set; } = string.Empty;
        public long Views { get; set; }
        public long Inquiries { get; set; }
    }

    public class RealTimeAnalytics
    {
        public DateTime Timestamp { get; set; }
        public int ActiveUsers { get; set; }
        public long ViewsLastHour { get; set; }
        public long InquiriesLastHour { get; set; }
        public int NewListingsLastHour { get; set; }
    }

    public class ActiveUser
    {
        public string UserId { get; set; } = string.Empty;
        public DateTime LastActive { get; set; }
        public string CurrentPage { get; set; } = string.Empty;
    }

    public class ReportSchedule
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public ReportType ReportType { get; set; }
        public string Frequency { get; set; } = string.Empty; // Daily, Weekly, Monthly
        public string RecipientEmail { get; set; } = string.Empty;
        public ExportFormat Format { get; set; }
        public bool IsActive { get; set; }
    }

    public class AdminDashboardData
    {
        public PerformanceMetrics PerformanceMetrics { get; set; } = new();
        public IEnumerable<TrendData> ViewTrends { get; set; } = new List<TrendData>();
        public Dictionary<string, int> PopularMakes { get; set; } = new();
        public int TotalSellers { get; set; }
        public int NewSellersThisWeek { get; set; }
        public decimal PlatformRevenue { get; set; }
    }

    public class SellerDashboardData
    {
        public string SellerId { get; set; } = string.Empty;
        public SellerPerformanceMetrics PerformanceMetrics { get; set; } = new();
        public IEnumerable<Vehicle> RecentActivity { get; set; } = new List<Vehicle>();
        public IEnumerable<Vehicle> TopPerformingVehicles { get; set; } = new List<Vehicle>();
        public IEnumerable<string> Recommendations { get; set; } = new List<string>();
    }

    public class AnalyticsAlert
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsResolved { get; set; }
    }

    public class SellerAnalytics
    {
        public string SellerId { get; set; } = string.Empty;
        public int TotalListings { get; set; }
        public int ActiveListings { get; set; }
        public int SoldListings { get; set; }
        public long TotalViews { get; set; }
        public long TotalInquiries { get; set; }
        public decimal AverageRating { get; set; }
        public decimal TotalValue { get; set; }
    }

    public class SellerPerformance
    {
        public string SellerId { get; set; } = string.Empty;
        public string SellerName { get; set; } = string.Empty;
        public int TotalListings { get; set; }
        public int ActiveListings { get; set; }
        public long TotalViews { get; set; }
        public decimal AverageRating { get; set; }
    }

    public class SellerReview
    {
        public string Id { get; set; } = string.Empty;
        public string SellerId { get; set; } = string.Empty;
        public string ReviewerId { get; set; } = string.Empty;
        public string ReviewerName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class SellerDashboard
    {
        public string SellerId { get; set; } = string.Empty;
        public SellerAnalytics Analytics { get; set; } = new();
        public IEnumerable<Vehicle> RecentListings { get; set; } = new List<Vehicle>();
        public decimal TotalEarnings { get; set; }
        public decimal PendingPayout { get; set; }
    }

    public class SellerPreferences
    {
        public bool EmailNotifications { get; set; } = true;
        public bool SmsNotifications { get; set; } = false;
        public bool AutoRenewListings { get; set; } = false;
        public int DefaultListingDuration { get; set; } = 30;
        public string PreferredLanguage { get; set; } = "en";
        public string TimeZone { get; set; } = "UTC";
    }
}
