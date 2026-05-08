using MongoDB.Bson.Serialization.Attributes;

namespace AutoNext.Platform.Listings.API.Models
{
    public class TruckSpecifications
    {
        [BsonElement("load_capacity")]
        public string LoadCapacity { get; set; } = string.Empty;  // in tons

        [BsonElement("container_length")]
        public string ContainerLength { get; set; } = string.Empty;  // in feet

        [BsonElement("axle_type")]
        public string AxleType { get; set; } = string.Empty;  // Single, Tandem, Multi

        [BsonElement("body_type")]
        public string BodyType { get; set; } = string.Empty;  // container, tipper, trailer, tanker

        [BsonElement("number_of_tyres")]
        public int NumberOfTyres { get; set; }

        [BsonElement("hydraulic_lift")]
        public bool HasHydraulicLift { get; set; }

        [BsonElement("gps_tracking")]
        public bool HasGpsTracking { get; set; }
    }

}
