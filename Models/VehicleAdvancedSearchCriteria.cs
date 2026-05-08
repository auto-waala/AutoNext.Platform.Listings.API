namespace AutoNext.Platform.Listings.API.Models
{
    public class VehicleAdvancedSearchCriteria : VehicleSearchCriteria
    {
        // Additional advanced search filters
        public int? MinKilometers { get; set; }
        public int? MaxKilometers { get; set; }
        public int? MinEngineCC { get; set; }
        public int? MaxEngineCC { get; set; }
        public bool IsFeaturedOnly { get; set; } = false;
        public int? MinOwnerCount { get; set; }
        public int? MaxOwnerCount { get; set; }

        // Sorting and pagination
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; } = "desc";
        public int? Page { get; set; }
        public int? PageSize { get; set; }

        // Additional vehicle specifications
        public string? BodyType { get; set; }  // SUV, Sedan, Hatchback, etc.
        public string? DriveType { get; set; }  // FWD, RWD, AWD, 4WD
        public int? MinSeatingCapacity { get; set; }
        public int? MaxSeatingCapacity { get; set; }
        public bool? HasSunroof { get; set; }
        public bool? HasNavigation { get; set; }
        public bool? HasLeatherSeats { get; set; }
        public bool? HasBackupCamera { get; set; }
        public bool? HasBluetooth { get; set; }
        public string? ExteriorColor { get; set; }
        public string? InteriorColor { get; set; }

        // Condition and history
        public string? Condition { get; set; }  // Excellent, Good, Fair
        public bool? HasAccidentHistory { get; set; }
        public bool? HasServiceHistory { get; set; }
        public bool? IsCertifiedPreOwned { get; set; }

        // Warranty and insurance
        public bool? HasWarranty { get; set; }
        public int? MinWarrantyMonths { get; set; }
        public bool? HasInsurance { get; set; }

        // Seller filters
        public string? SellerType { get; set; }  // Individual, Dealer, Business
        public bool? IsVerifiedSeller { get; set; }
        public decimal? MinSellerRating { get; set; }

        // Date filters
        public DateTime? ListedAfter { get; set; }
        public DateTime? ListedBefore { get; set; }
        public DateTime? SoldAfter { get; set; }
        public DateTime? SoldBefore { get; set; }

        // Geographic filters
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? RadiusInKm { get; set; }

        // Additional filters
        public List<string>? IncludedFeatures { get; set; }
        public List<string>? ExcludedFeatures { get; set; }
        public string? SearchText { get; set; }  // Search across title, description, features

        // Advanced options
        public bool IncludeInactive { get; set; } = false;
        public bool IncludeSold { get; set; } = false;
        public bool IncludeExpired { get; set; } = false;
        public bool UseWildcardSearch { get; set; } = true;
        public bool CaseInsensitiveSearch { get; set; } = true;
    }

    // Supporting classes for advanced search
    public class SearchCriteriaBase
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; } = "desc";
    }


    public class VehicleSearchResult
    {
        public IEnumerable<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public long TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public Dictionary<string, List<FacetCount>> Facets { get; set; } = new();
        public long SearchExecutionTimeMs { get; set; }
    }




    public enum QueryOperator
    {
        And,
        Or
    }

    public class SearchQueryBuilder
    {
        public QueryOperator DefaultOperator { get; set; } = QueryOperator.And;
        public List<SearchCondition> Conditions { get; set; } = new();
        public List<string> SortFields { get; set; } = new();
    }

    public class SearchCondition
    {
        public string Field { get; set; } = string.Empty;
        public string Operator { get; set; } = string.Empty; // eq, ne, gt, lt, gte, lte, contains, startswith, endswith, in
        public object Value { get; set; } = string.Empty;
        public List<object> Values { get; set; } = new(); // For 'in' operator
        public QueryOperator ConditionOperator { get; set; } = QueryOperator.And;
    }
}