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
        public string SellerType { get; set; } = string.Empty;

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

        [BsonElement("user_id")]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("phone")]
        public string Phone { get; set; } = string.Empty;

        [BsonElement("dealer_id")]
        public string DealerId { get; set; } = string.Empty;

        [BsonElement("store_id")]
        public string StoreId { get; set; } = string.Empty;

        [BsonElement("location")]
        public string Location { get; set; } = string.Empty;
        [BsonElement("chat_enabled")]
        public bool ChatEnabled { get; set; } = false;

        [BsonElement("call_enabled")]
        public bool CallEnabled { get; set; } = false;

        [BsonElement("is_verified")]
        public bool IsVerified { get; set; } = false;
    }
}
