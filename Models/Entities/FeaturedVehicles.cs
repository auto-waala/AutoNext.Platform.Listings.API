using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AutoNext.Platform.Listings.API.Models.Entities
{
    public class FeaturedVehicle
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        // Basic Information
        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("descriptions")]
        public string Descriptions { get; set; } = string.Empty;

        [BsonElement("slug")]
        public string Slug { get; set; } = string.Empty;

        [BsonElement("meta_title")]
        public string MetaTitle { get; set; } = string.Empty;

        [BsonElement("meta_description")]
        public string MetaDescription { get; set; } = string.Empty;

        [BsonElement("brand_name")]
        public string BrandName { get; set; } = string.Empty;

        [BsonElement("model_name")]
        public string ModelName { get; set; } = string.Empty;

        [BsonElement("model_slug")]
        public string ModelSlug { get; set; } = string.Empty;

        [BsonElement("vehicle_type")]
        public string VehicleType { get; set; } = string.Empty;

        [BsonElement("body_type")]
        public string BodyType { get; set; } = string.Empty;

        // Price Information
        [BsonElement("price")]
        public PriceInfo Price { get; set; } = new();

        [BsonElement("price_range_from")]
        public string PriceRangeFrom { get; set; } = string.Empty;

        [BsonElement("price_range_to")]
        public string PriceRangeTo { get; set; } = string.Empty;

        // Media
        [BsonElement("images")]
        public List<VehicleImage> Images { get; set; } = new();

        [BsonElement("videos")]
        public List<VehicleMedia> Videos { get; set; } = new();

        [BsonElement("shorts")]
        public List<VehicleMedia> Shorts { get; set; } = new();

        // Thumbnail (cached for performance - derived from Images where IsPrimary=true)
        [BsonElement("thumbnail")]
        public string Thumbnail { get; set; } = string.Empty;

        [BsonElement("thumbnail_webp")]
        public string? ThumbnailWebp { get; set; }

        // Specifications
        [BsonElement("variants")]
        public List<VehicleVariant> Variants { get; set; } = new();

        [BsonElement("key_specifications")]
        public KeySpecifications KeySpecifications { get; set; } = new();

        [BsonElement("top_features")]
        public List<FeatureItem> TopFeatures { get; set; } = new();

        [BsonElement("stand_out_features")]
        public List<FeatureItem> StandOutFeatures { get; set; } = new();

        [BsonElement("pros")]
        public List<ProConItem> Pros { get; set; } = new();

        [BsonElement("cons")]
        public List<ProConItem> Cons { get; set; } = new();

        [BsonElement("tags")]
        public List<TagItem> Tags { get; set; } = new();

        // Ratings & Engagement
        [BsonElement("user_ratings")]
        public List<UserRating> UserRatings { get; set; } = new();

        [BsonElement("rating")]
        public double Rating { get; set; }

        // Seller Information
        [BsonElement("seller")]
        public SellerInfo Seller { get; set; } = new();

        // Location
        [BsonElement("location")]
        public LocationInfo Location { get; set; } = new();

        // Vehicle Condition
        [BsonElement("condition")]
        public VehicleCondition Condition { get; set; } = new();

        // Listing Details
        [BsonElement("listing_details")]
        public ListingDetails ListingDetails { get; set; } = new();

        // Engagement Metrics
        [BsonElement("engagement")]
        public EngagementMetrics Engagement { get; set; } = new();

        // Share URLs
        [BsonElement("share_urls")]
        public ShareUrls ShareUrls { get; set; } = new();

        // Display Features (for featured vehicle)
        [BsonElement("badges")]
        public List<string> Badges { get; set; } = new();

        [BsonElement("highlight")]
        public string? Highlight { get; set; }

        // Test Drive
        [BsonElement("test_drive")]
        public TestDriveInfo TestDrive { get; set; } = new();

        // Sorting & Visibility (Featured specific)
        [BsonElement("priority")]
        public int Priority { get; set; } = 1;

        [BsonElement("is_active")]
        public bool IsActive { get; set; } = true;

        [BsonElement("start_date")]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        [BsonElement("end_date")]
        public DateTime? EndDate { get; set; }

        // Audit
        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Helper method to update thumbnail from primary image
        public void UpdateThumbnailFromPrimaryImage()
        {
            var primaryImage = Images?.FirstOrDefault(x => x.IsPrimary);
            if (primaryImage != null && !string.IsNullOrEmpty(primaryImage.FileUrl))
            {
                Thumbnail = primaryImage.FileUrl;
            }
        }
    }

    // Nested Classes

    public class PriceInfo
    {
        [BsonElement("amount")]
        public decimal Amount { get; set; }

        [BsonElement("currency")]
        public string Currency { get; set; } = "INR";

        [BsonElement("negotiable")]
        public bool Negotiable { get; set; } = true;

        [BsonElement("on_road_price")]
        public decimal OnRoadPrice { get; set; }
    }

    public class VehicleImage
    {
        [BsonElement("file_id")]
        public string FileId { get; set; } = string.Empty;

        [BsonElement("file_url")]
        public string FileUrl { get; set; } = string.Empty;

        [BsonElement("is_primary")]
        public bool IsPrimary { get; set; } = false;
    }

    public class VehicleMedia
    {
        [BsonElement("file_url")]
        public string FileUrl { get; set; } = string.Empty;
    }

    public class VehicleVariant
    {
        [BsonElement("color")]
        public string Color { get; set; } = string.Empty;

        [BsonElement("engine")]
        public string Engine { get; set; } = string.Empty;

        [BsonElement("transmission")]
        public string Transmission { get; set; } = string.Empty;

        [BsonElement("fuel_type")]
        public string FuelType { get; set; } = string.Empty;

        [BsonElement("mileage")]
        public string Mileage { get; set; } = string.Empty;

        [BsonElement("year_of_manufacture")]
        public string YearOfManufacture { get; set; } = string.Empty;
    }

    public class KeySpecifications
    {
        [BsonElement("engine")]
        public string Engine { get; set; } = string.Empty;

        [BsonElement("transmission")]
        public string Transmission { get; set; } = string.Empty;

        [BsonElement("fuel_type")]
        public string FuelType { get; set; } = string.Empty;

        [BsonElement("mileage")]
        public string Mileage { get; set; } = string.Empty;

        [BsonElement("year_of_manufacture")]
        public string YearOfManufacture { get; set; } = string.Empty;
    }

    public class FeatureItem
    {
        [BsonElement("feature")]
        public string Feature { get; set; } = string.Empty;
    }

    public class ProConItem
    {
        [BsonElement("pro")]
        public string Pro { get; set; } = string.Empty;

        [BsonElement("con")]
        public string Con { get; set; } = string.Empty;
    }

    public class TagItem
    {
        [BsonElement("tag_name")]
        public string TagName { get; set; } = string.Empty;
    }

    public class UserRating
    {
        [BsonElement("user_id")]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("rating")]
        public int Rating { get; set; }

        [BsonElement("comment")]
        public string Comment { get; set; } = string.Empty;

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class LocationInfo
    {
        [BsonElement("city")]
        public string City { get; set; } = string.Empty;

        [BsonElement("state")]
        public string State { get; set; } = string.Empty;

        [BsonElement("pincode")]
        public string Pincode { get; set; } = string.Empty;

        [BsonElement("latitude")]
        public string Latitude { get; set; } = string.Empty;

        [BsonElement("longitude")]
        public string Longitude { get; set; } = string.Empty;
    }

    public class VehicleCondition
    {
        [BsonElement("is_new")]
        public bool IsNew { get; set; } = true;

        [BsonElement("owner_count")]
        public int OwnerCount { get; set; } = 0;

        [BsonElement("km_driven")]
        public int KMDriven { get; set; } = 0;

        [BsonElement("accidental")]
        public bool Accidental { get; set; } = false;

        [BsonElement("service_history_available")]
        public bool ServiceHistoryAvailable { get; set; } = true;
    }

    public class ListingDetails
    {
        [BsonElement("is_available")]
        public bool IsAvailable { get; set; } = true;

        [BsonElement("is_featured")]
        public bool IsFeatured { get; set; } = false;

        [BsonElement("is_sold")]
        public bool IsSold { get; set; } = false;

        [BsonElement("posted_date")]
        public DateTime PostedDate { get; set; } = DateTime.UtcNow;

        [BsonElement("expiry_date")]
        public DateTime? ExpiryDate { get; set; }

        [BsonElement("is_verified")]
        public bool IsVerified { get; set; } = false;

        [BsonElement("verified_by")]
        public string VerifiedBy { get; set; } = string.Empty;

        [BsonElement("verification_date")]
        public DateTime? VerificationDate { get; set; }
    }

    public class EngagementMetrics
    {
        [BsonElement("views")]
        public long Views { get; set; } = 0;

        [BsonElement("likes")]
        public long Likes { get; set; } = 0;

        [BsonElement("shares")]
        public long Shares { get; set; } = 0;

        [BsonElement("enquiries")]
        public long Enquiries { get; set; } = 0;
    }

    public class ShareUrls
    {
        [BsonElement("facebook")]
        public string Facebook { get; set; } = string.Empty;

        [BsonElement("twitter")]
        public string Twitter { get; set; } = string.Empty;

        [BsonElement("whats_app")]
        public string WhatsApp { get; set; } = string.Empty;

        [BsonElement("linked_in")]
        public string LinkedIn { get; set; } = string.Empty;
    }

    public class TestDriveInfo
    {
        [BsonElement("available")]
        public bool Available { get; set; } = true;

        [BsonElement("booking_amount")]
        public decimal BookingAmount { get; set; } = 0;
    }
}