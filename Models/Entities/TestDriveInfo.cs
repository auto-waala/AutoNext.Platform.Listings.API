using MongoDB.Bson.Serialization.Attributes;

namespace AutoNext.Platform.Listings.API.Models.Entities
{
    public class TestDriveInfo
    {
        [BsonElement("available")]
        public bool Available { get; set; } = true;

        [BsonElement("booking_amount")]
        public decimal BookingAmount { get; set; } = 0;
    }
}
