using MongoDB.Bson.Serialization.Attributes;

namespace AutoNext.Platform.Listings.API.Models
{
    public class Price
    {
        [BsonElement("raw")]
        public decimal Raw { get; set; }

        [BsonElement("display")]
        public string Display { get; set; } = string.Empty;

        [BsonElement("currency")]
        public Currency Currency { get; set; } = new();

        [BsonElement("negotiable")]
        public bool Negotiable { get; set; } = true;

        [BsonElement("original_price")]
        public decimal? OriginalPrice { get; set; }

        [BsonElement("price_updated_on")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? PriceUpdatedOn { get; set; }
    }
}
