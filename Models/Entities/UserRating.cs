using MongoDB.Bson.Serialization.Attributes;

namespace AutoNext.Platform.Listings.API.Models.Entities
{
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
}
