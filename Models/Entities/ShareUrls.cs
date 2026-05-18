using MongoDB.Bson.Serialization.Attributes;

namespace AutoNext.Platform.Listings.API.Models.Entities
{
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
}
