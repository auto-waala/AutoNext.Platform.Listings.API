using MongoDB.Bson.Serialization.Attributes;

namespace AutoNext.Platform.Listings.API.Models
{
    public class Currency
    {
        [BsonElement("pre")]
        public string Pre { get; set; } = "₹";

        [BsonElement("post")]
        public string Post { get; set; } = string.Empty;

        [BsonElement("iso_4217")]
        public string Iso4217 { get; set; } = "INR";

        [BsonElement("locale")]
        public string Locale { get; set; } = "hi_IN";
    }
}
