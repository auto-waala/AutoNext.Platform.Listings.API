using MongoDB.Bson.Serialization.Attributes;

namespace AutoNext.Platform.Listings.API.Models.Entities
{
    public class VehicleVariant
    {
        [BsonElement("color")]
        public string Color { get; set; } = string.Empty;

        [BsonElement("engine")]
        public string Engine { get; set; } = string.Empty;

        [BsonElement("transmission")]
        public string Transmission { get; set; } = string.Empty;

        [BsonElement("fuel_type")]
        public string FuelType { get; set; } = string.Empty;

        [BsonElement("mileage")]
        public string Mileage { get; set; } = string.Empty;

        [BsonElement("year_of_manufacture")]
        public string YearOfManufacture { get; set; } = string.Empty;
    }
}
