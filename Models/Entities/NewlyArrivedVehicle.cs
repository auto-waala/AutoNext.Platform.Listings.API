using AutoNext.Platform.Listings.API.Models.DTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AutoNext.Platform.Listings.API.Models.Entities
{
    public class NewlyArrivedVehicle
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        // Basic Info
        [BsonElement("brand_name")]
        public string BrandName { get; set; } = string.Empty;

        [BsonElement("model_name")]
        public string ModelName { get; set; } = string.Empty;

        [BsonElement("model_slug")]
        public string ModelSlug { get; set; } = string.Empty;

        [BsonElement("vehicle_type")]
        public string VehicleType { get; set; } = string.Empty; // Car, Bike, Truck, EV

        [BsonElement("body_type")]
        public string BodyType { get; set; } = string.Empty; // SUV, Sedan, Hatchback, etc.

        [BsonElement("price_range")]
        public string PriceRange { get; set; } = string.Empty;

        [BsonElement("min_price")]
        public decimal MinPrice { get; set; }

        [BsonElement("max_price")]
        public decimal MaxPrice { get; set; }

        // Arrival Info
        [BsonElement("arrival_date")]
        public DateTime ArrivalDate { get; set; } = DateTime.UtcNow;

        [BsonElement("arrival_period")]
        public string ArrivalPeriod { get; set; } = "weekly"; // weekly, monthly, yearly

        [BsonElement("is_newly_arrived")]
        public bool IsNewlyArrived { get; set; } = true;

        [BsonElement("expiry_date")]
        public DateTime? ExpiryDate { get; set; }

        [BsonElement("display_order")]
        public int DisplayOrder { get; set; }

        [BsonElement("featured")]
        public bool Featured { get; set; } = false;

        // EMI
        [BsonElement("emi")]
        public EmiDto? Emi { get; set; }

        // Images
        [BsonElement("images")]
        public List<ImageDto>? Images { get; set; }

        [BsonElement("thumbnail_image")]
        public string? ThumbnailImage { get; set; }

        [BsonElement("thumbnail_webp")]
        public string? ThumbnailWebp { get; set; }

        [BsonElement("og_image")]
        public string? OgImage { get; set; }

        // Videos (YouTube URLs)
        [BsonElement("videos")]
        public List<VideoDto>? Videos { get; set; }

        // Variants
        [BsonElement("variants")]
        public List<VariantDetailDto>? Variants { get; set; }


        // Ratings & Reviews
        [BsonElement("rating")]
        public double Rating { get; set; }

        [BsonElement("review_count")]
        public int ReviewCount { get; set; }

        [BsonElement("review_url")]
        public string? ReviewUrl { get; set; }

        // Page Title
        [BsonElement("page_title")]
        public string PageTitle { get; set; } = string.Empty;

        // Description Text
        [BsonElement("description_text")]
        public string DescriptionText { get; set; } = string.Empty;

        // Status
        [BsonElement("status")]
        public bool Status { get; set; } = true;

        [BsonElement("is_active")]
        public bool IsActive { get; set; } = true;

        // Audit
        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("published_by")]
        public string? PublishedBy { get; set; }
    }
}
