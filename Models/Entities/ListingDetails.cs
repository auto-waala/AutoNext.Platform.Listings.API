using MongoDB.Bson.Serialization.Attributes;

namespace AutoNext.Platform.Listings.API.Models.Entities
{
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
}
