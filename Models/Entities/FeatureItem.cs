using MongoDB.Bson.Serialization.Attributes;

namespace AutoNext.Platform.Listings.API.Models.Entities
{
    public class FeatureItem
    {
        [BsonElement("feature")]
        public string Feature { get; set; } = string.Empty;
    }
}
