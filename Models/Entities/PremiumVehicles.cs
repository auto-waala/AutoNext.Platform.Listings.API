using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AutoNext.Platform.Listings.API.Models.Entities
{
    public class PremiumVehicle
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

        // Thumbnail
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

        // Premium Display Features
        [BsonElement("badges")]
        public List<string> Badges { get; set; } = new();

        [BsonElement("highlight")]
        public string? Highlight { get; set; }

        // Test Drive
        [BsonElement("test_drive")]
        public TestDriveInfo TestDrive { get; set; } = new();

        // Premium-specific fields
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

        // Helper method
        public void UpdateThumbnailFromPrimaryImage()
        {
            var primaryImage = Images?.FirstOrDefault(x => x.IsPrimary);

            if (primaryImage != null && !string.IsNullOrEmpty(primaryImage.FileUrl))
            {
                Thumbnail = primaryImage.FileUrl;
            }
        }
    }
}