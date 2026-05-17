using MongoDB.Bson.Serialization.Attributes;

namespace AutoNext.Platform.Listings.API.Models.Entities
{
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

}
