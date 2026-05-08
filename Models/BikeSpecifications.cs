using MongoDB.Bson.Serialization.Attributes;

namespace AutoNext.Platform.Listings.API.Models
{
    public class BikeSpecifications
    {
        [BsonElement("bike_type")]
        public string BikeType { get; set; } = string.Empty;  // sports, cruiser, commuter, scooter

        [BsonElement("engine_cc")]
        public int EngineCC { get; set; }

        [BsonElement("top_speed")]
        public int TopSpeed { get; set; }  // km/h

        [BsonElement("kerb_weight")]
        public double KerbWeight { get; set; }  // in kg

        [BsonElement("seat_height")]
        public double SeatHeight { get; set; }  // in mm

        [BsonElement("abs")]
        public bool HasABS { get; set; }

        [BsonElement("disc_brakes")]
        public bool HasDiscBrakes { get; set; }
    }

}
