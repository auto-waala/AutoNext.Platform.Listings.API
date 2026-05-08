using MongoDB.Bson.Serialization.Attributes;

namespace AutoNext.Platform.Listings.API.Models
{
    public class VehicleSpecifications
    {
        [BsonElement("make")]
        public string Make { get; set; } = string.Empty;

        [BsonElement("model")]
        public string Model { get; set; } = string.Empty;

        [BsonElement("variant")]
        public string Variant { get; set; } = string.Empty;

        [BsonElement("year")]
        public int Year { get; set; }

        [BsonElement("kilometers_driven")]
        public int KilometersDriven { get; set; }

        [BsonElement("fuel_type")]
        public string FuelType { get; set; } = string.Empty;  // petrol, diesel, electric, cng

        [BsonElement("transmission")]
        public string Transmission { get; set; } = string.Empty;  // automatic, manual

        [BsonElement("engine_cc")]
        public int EngineCC { get; set; }

        [BsonElement("mileage")]
        public double Mileage { get; set; }  // km/l or km/charge

        [BsonElement("ownership")]
        public string Ownership { get; set; } = string.Empty;  // 1st, 2nd, 3rd

        [BsonElement("registered_in")]
        public string RegisteredIn { get; set; } = string.Empty;  // RTO code

        [BsonElement("insurance_valid_till")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? InsuranceValidTill { get; set; }

        [BsonElement("fitness_valid_till")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? FitnessValidTill { get; set; }

        [BsonElement("no_of_owners")]
        public int NoOfOwners { get; set; } = 1;

        [BsonElement("color")]
        public string Color { get; set; } = string.Empty;

        [BsonElement("seating_capacity")]
        public int SeatingCapacity { get; set; } = 5;

        [BsonElement("fuel_tank_capacity")]
        public double FuelTankCapacity { get; set; }  // in liters

        [BsonElement("ground_clearance")]
        public double GroundClearance { get; set; }  // in mm

        [BsonElement("tyre_size")]
        public string TyreSize { get; set; } = string.Empty;

        [BsonElement("emission_norms")]
        public string EmissionNorms { get; set; } = string.Empty;  // BS6, BS4, etc.
    }

}
