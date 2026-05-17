using MongoDB.Bson.Serialization.Attributes;

namespace AutoNext.Platform.Listings.API.Models.Entities
{
    public class TagItem
    {
        [BsonElement("tag_name")]
        public string TagName { get; set; } = string.Empty;
    }
}
