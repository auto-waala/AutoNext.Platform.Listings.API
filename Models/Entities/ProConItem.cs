using MongoDB.Bson.Serialization.Attributes;

namespace AutoNext.Platform.Listings.API.Models.Entities
{
    public class ProConItem
    {
        [BsonElement("pro")]
        public string Pro { get; set; } = string.Empty;

        [BsonElement("con")]
        public string Con { get; set; } = string.Empty;
    }
}
