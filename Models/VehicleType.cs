using MongoDB.Bson.Serialization.Attributes;

namespace AutoNext.Platform.Listings.API.Models
{
    public class VehicleType
    {
        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;  // "car", "truck", "bike", "suv", "sedan"

        [BsonElement("category_id")]
        public string CategoryId { get; set; } = string.Empty;

        [BsonElement("sub_category_id")]
        public string SubCategoryId { get; set; } = string.Empty;
    }
}
