using MongoDB.Bson.Serialization.Attributes;

namespace AutoNext.Platform.Listings.API.Models
{
    public class VehicleLocation
    {
        [BsonElement("lat")]
        public double Lat { get; set; }

        [BsonElement("lon")]
        public double Lon { get; set; }

        [BsonElement("region_id")]
        public string RegionId { get; set; } = string.Empty;

        [BsonElement("region_name")]
        public string RegionName { get; set; } = string.Empty;

        [BsonElement("city_id")]
        public string CityId { get; set; } = string.Empty;

        [BsonElement("city_name")]
        public string CityName { get; set; } = string.Empty;

        [BsonElement("subregion_id")]
        public string SubregionId { get; set; } = string.Empty;

        [BsonElement("district_id")]
        public string DistrictId { get; set; } = string.Empty;

        [BsonElement("neighbourhood")]
        public string Neighbourhood { get; set; } = string.Empty;

        [BsonElement("full_address")]
        public string FullAddress { get; set; } = string.Empty;

        [BsonElement("pincode")]
        public string Pincode { get; set; } = string.Empty;

        [BsonElement("address_components")]
        public List<AddressComponent> AddressComponents { get; set; } = new();
    }

    public class AddressComponent
    {
        [BsonElement("order")]
        public int Order { get; set; }

        [BsonElement("id")]
        public string Id { get; set; } = string.Empty;

        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;  // COUNTRY, STATE, CITY, NEIGHBOURHOOD

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("latitude")]
        public double Latitude { get; set; }

        [BsonElement("longitude")]
        public double Longitude { get; set; }
    }

    public class VehicleImage
    {
        [BsonElement("id")]
        public string Id { get; set; } = string.Empty;

        [BsonElement("url")]
        public string Url { get; set; } = string.Empty;

        [BsonElement("full_url")]
        public string FullUrl { get; set; } = string.Empty;

        [BsonElement("thumbnail_url")]
        public string ThumbnailUrl { get; set; } = string.Empty;

        [BsonElement("width")]
        public int Width { get; set; }

        [BsonElement("height")]
        public int Height { get; set; }

        [BsonElement("size_kb")]
        public long SizeKb { get; set; }

        [BsonElement("is_primary")]
        public bool IsPrimary { get; set; }

        [BsonElement("uploaded_on")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UploadedOn { get; set; } = DateTime.Now;

        [BsonElement("order")]
        public int Order { get; set; }
    }

    public class VehicleVideo
    {
        [BsonElement("id")]
        public string Id { get; set; } = string.Empty;

        [BsonElement("url")]
        public string Url { get; set; } = string.Empty;

        [BsonElement("thumbnail_url")]
        public string ThumbnailUrl { get; set; } = string.Empty;

        [BsonElement("duration")]
        public int Duration { get; set; }  // in seconds
    }

    public class Seller
    {
        [BsonElement("user_id")]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("phone")]
        public string Phone { get; set; } = string.Empty;

        [BsonElement("phone_verified")]
        public bool PhoneVerified { get; set; }

        [BsonElement("email_verified")]
        public bool EmailVerified { get; set; }

        [BsonElement("seller_type")]
        public string SellerType { get; set; } = "individual";  // individual, dealer, business

        [BsonElement("dealership_name")]
        public string DealershipName { get; set; } = string.Empty;

        [BsonElement("rating")]
        public double Rating { get; set; }

        [BsonElement("total_listings")]
        public int TotalListings { get; set; }

        [BsonElement("member_since")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime MemberSince { get; set; }

        [BsonElement("is_verified_seller")]
        public bool IsVerifiedSeller { get; set; }

        [BsonElement("badges")]
        public List<string> Badges { get; set; } = new();

        [BsonElement("location")]
        public SellerLocation Location { get; set; } = new();
    }

    public class SellerLocation
    {
        [BsonElement("lat")]
        public double Lat { get; set; }

        [BsonElement("lon")]
        public double Lon { get; set; }
    }

    public class VehicleStatus
    {
        [BsonElement("current_status")]
        public string CurrentStatus { get; set; } = "active";  // active, sold, expired, draft, paused, reported

        [BsonElement("display_status")]
        public string DisplayStatus { get; set; } = "active";

        [BsonElement("allow_edit")]
        public bool AllowEdit { get; set; } = true;

        [BsonElement("allow_deactivate")]
        public bool AllowDeactivate { get; set; } = true;

        [BsonElement("flags")]
        public StatusFlags Flags { get; set; } = new();

        [BsonElement("ban_reason")]
        public string BanReason { get; set; } = string.Empty;

        [BsonElement("reported_count")]
        public int ReportedCount { get; set; }
    }

    public class StatusFlags
    {
        [BsonElement("is_new")]
        public bool IsNew { get; set; }

        [BsonElement("is_hot")]
        public bool IsHot { get; set; }

        [BsonElement("is_featured")]
        public bool IsFeatured { get; set; }

        [BsonElement("is_urgent")]
        public bool IsUrgent { get; set; }
    }

    public class VehicleDates
    {
        [BsonElement("created_at")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [BsonElement("created_at_first")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedAtFirst { get; set; } = DateTime.Now;

        [BsonElement("republish_date")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? RepublishDate { get; set; }

        [BsonElement("valid_to")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime ValidTo { get; set; }

        [BsonElement("last_updated")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        [BsonElement("expires_on")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime ExpiresOn { get; set; }

        [BsonElement("sold_date")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? SoldDate { get; set; }
    }

    public class VehicleAnalytics
    {
        [BsonElement("views")]
        public long Views { get; set; }

        [BsonElement("unique_views")]
        public long UniqueViews { get; set; }

        [BsonElement("replies")]
        public long Replies { get; set; }

        [BsonElement("calls")]
        public long Calls { get; set; }

        [BsonElement("favorites")]
        public long Favorites { get; set; }

        [BsonElement("shares")]
        public long Shares { get; set; }

        [BsonElement("impressions")]
        public long Impressions { get; set; }

        [BsonElement("click_through_rate")]
        public double ClickThroughRate { get; set; }
    }

    public class Monetization
    {
        [BsonElement("is_featured")]
        public bool IsFeatured { get; set; }

        [BsonElement("featured_until")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? FeaturedUntil { get; set; }

        [BsonElement("package_id")]
        public int PackageId { get; set; }

        [BsonElement("package_name")]
        public string PackageName { get; set; } = string.Empty;

        [BsonElement("auto_renew")]
        public bool AutoRenew { get; set; }

        [BsonElement("promotion_level")]
        public string PromotionLevel { get; set; } = "standard";  // standard, premium, vip

        [BsonElement("boosted_count")]
        public int BoostedCount { get; set; }

        [BsonElement("last_boosted_at")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? LastBoostedAt { get; set; }
    }

    public class VehicleDocuments
    {
        [BsonElement("rc_available")]
        public bool RCAvailable { get; set; }

        [BsonElement("insurance_available")]
        public bool InsuranceAvailable { get; set; }

        [BsonElement("service_records_available")]
        public bool ServiceRecordsAvailable { get; set; }

        [BsonElement("no_claim_bonus")]
        public bool NoClaimBonus { get; set; }

        [BsonElement("hypothecation")]
        public bool Hypothecation { get; set; }

        [BsonElement("loan_pending")]
        public bool LoanPending { get; set; }

        [BsonElement("pollution_certificate")]
        public bool PollutionCertificate { get; set; }

        [BsonElement("tax_paid_till")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? TaxPaidTill { get; set; }

        [BsonElement("document_urls")]
        public List<DocumentUrl> DocumentUrls { get; set; } = new();
    }

    public class DocumentUrl
    {
        [BsonElement("document_type")]
        public string DocumentType { get; set; } = string.Empty;  // RC, Insurance, Service, etc.

        [BsonElement("url")]
        public string Url { get; set; } = string.Empty;

        [BsonElement("verified")]
        public bool Verified { get; set; }
    }

    public class VehicleMetadata
    {
        [BsonElement("source")]
        public string Source { get; set; } = "web";  // web, mobile_app, api

        [BsonElement("platform")]
        public string Platform { get; set; } = "vehicle_portal";

        [BsonElement("ip_address")]
        public string IpAddress { get; set; } = string.Empty;

        [BsonElement("user_agent")]
        public string UserAgent { get; set; } = string.Empty;

        [BsonElement("tags")]
        public List<string> Tags { get; set; } = new();

        [BsonElement("search_keywords")]
        public List<string> SearchKeywords { get; set; } = new();
    }

    public class VehicleInspection
    {
        [BsonElement("is_inspected")]
        public bool IsInspected { get; set; }

        [BsonElement("inspection_agency")]
        public string InspectionAgency { get; set; } = string.Empty;

        [BsonElement("inspection_report_url")]
        public string InspectionReportUrl { get; set; } = string.Empty;

        [BsonElement("inspection_date")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? InspectionDate { get; set; }

        [BsonElement("inspection_score")]
        public int? InspectionScore { get; set; }  // Out of 100

        [BsonElement("verified_km")]
        public int? VerifiedKm { get; set; }

        [BsonElement("inspection_notes")]
        public string InspectionNotes { get; set; } = string.Empty;
    }

    public class VehicleHistory
    {
        [BsonElement("accident_history")]
        public bool HasAccidentHistory { get; set; }

        [BsonElement("accident_details")]
        public string AccidentDetails { get; set; } = string.Empty;

        [BsonElement("ownership_history")]
        public List<OwnershipHistory> OwnershipHistory { get; set; } = new();

        [BsonElement("insurance_claims")]
        public List<InsuranceClaim> InsuranceClaims { get; set; } = new();

        [BsonElement("rc_transfers")]
        public List<RcTransfer> RcTransfers { get; set; } = new();
    }

    public class OwnershipHistory
    {
        [BsonElement("owner_name")]
        public string OwnerName { get; set; } = string.Empty;

        [BsonElement("from_date")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime FromDate { get; set; }

        [BsonElement("to_date")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime ToDate { get; set; }

        [BsonElement("registered_in")]
        public string RegisteredIn { get; set; } = string.Empty;
    }

    public class InsuranceClaim
    {
        [BsonElement("claim_id")]
        public string ClaimId { get; set; } = string.Empty;

        [BsonElement("claim_date")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime ClaimDate { get; set; }

        [BsonElement("claim_amount")]
        public decimal ClaimAmount { get; set; }

        [BsonElement("claim_type")]
        public string ClaimType { get; set; } = string.Empty;
    }

    public class RcTransfer
    {
        [BsonElement("transfer_date")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime TransferDate { get; set; }

        [BsonElement("from_name")]
        public string FromName { get; set; } = string.Empty;

        [BsonElement("to_name")]
        public string ToName { get; set; } = string.Empty;

        [BsonElement("rto_office")]
        public string RtoOffice { get; set; } = string.Empty;
    }

    public class ServiceRecord
    {
        [BsonElement("service_date")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime ServiceDate { get; set; }

        [BsonElement("service_type")]
        public string ServiceType { get; set; } = string.Empty;  // Regular, Major, Repair

        [BsonElement("kilometers_at_service")]
        public int KilometersAtService { get; set; }

        [BsonElement("service_center")]
        public string ServiceCenter { get; set; } = string.Empty;

        [BsonElement("cost")]
        public decimal Cost { get; set; }

        [BsonElement("invoice_url")]
        public string InvoiceUrl { get; set; } = string.Empty;

        [BsonElement("work_done")]
        public List<string> WorkDone { get; set; } = new();
    }
}
