using MongoDB.Bson.Serialization.Attributes;

namespace AutoNext.Platform.Listings.API.Models.Entities
{
    public class LocationInfo
    {
        [BsonElement("city")]
        public string City { get; set; } = string.Empty;

        [BsonElement("state")]
        public string State { get; set; } = string.Empty;

        [BsonElement("pincode")]
        public string Pincode { get; set; } = string.Empty;

        [BsonElement("latitude")]
        public string Latitude { get; set; } = string.Empty;

        [BsonElement("longitude")]
        public string Longitude { get; set; } = string.Empty;
    }
}
