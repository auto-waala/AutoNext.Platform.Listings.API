using MongoDB.Bson.Serialization.Attributes;

namespace AutoNext.Platform.Listings.API.Models.Entities
{
    public class SellerInfo
    {
        [BsonElement("seller_id")]
        public string SellerId { get; set; } = string.Empty;

        [BsonElement("seller_name")]
        public string SellerName { get; set; } = string.Empty;

        [BsonElement("seller_type")]
        public string SellerType { get; set; } = string.Empty; // Dealer, Individual, etc.

        [BsonElement("contact_number")]
        public string ContactNumber { get; set; } = string.Empty;

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("is_verified_seller")]
        public bool IsVerifiedSeller { get; set; } = false;

        [BsonElement("dealer_name")]
        public string DealerName { get; set; } = string.Empty;

        [BsonElement("dealer_address")]
        public string DealerAddress { get; set; } = string.Empty;
    }
}
