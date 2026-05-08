using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AutoNext.Platform.Listings.API.Models
{
    [BsonIgnoreExtraElements]
    public class Vehicle
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("vehicle_id")]
        public string VehicleId { get; set; } = string.Empty;

        // VIN and Identification Numbers
        [BsonElement("vin")]
        public string VIN { get; set; } = string.Empty;  // Vehicle Identification Number (17 characters)

        [BsonElement("chassis_number")]
        public string ChassisNumber { get; set; } = string.Empty;

        [BsonElement("engine_number")]
        public string EngineNumber { get; set; } = string.Empty;

        [BsonElement("registration_number")]
        public string RegistrationNumber { get; set; } = string.Empty;  // License plate number

        [BsonElement("motor_number")]
        public string MotorNumber { get; set; } = string.Empty;  // For electric vehicles

        // Common Fields
        [BsonElement("created_by")]
        public string CreatedBy { get; set; } = string.Empty;

        [BsonElement("created_on")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        [BsonElement("modified_by")]
        public string ModifiedBy { get; set; } = string.Empty;

        [BsonElement("modified_on")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime ModifiedOn { get; set; } = DateTime.Now;

        [BsonElement("is_active")]
        public bool IsActive { get; set; } = true;

        [BsonElement("revision")]
        public string Revision { get; set; } = "1";

        [BsonElement("version")]
        public int Version { get; set; } = 1;

        // Vehicle Type
        [BsonElement("vehicle_type")]
        public VehicleType VehicleType { get; set; } = new();

        // Listing Details
        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        // Pricing
        [BsonElement("price")]
        public Price Price { get; set; } = new();

        // Vehicle Specifications
        [BsonElement("specifications")]
        public VehicleSpecifications Specifications { get; set; } = new();

        // Truck Specifics
        [BsonElement("truck_specs")]
        public TruckSpecifications TruckSpecs { get; set; } = new();

        // Bike Specifics
        [BsonElement("bike_specs")]
        public BikeSpecifications BikeSpecs { get; set; } = new();

        // Location Details
        [BsonElement("locations")]
        public List<VehicleLocation> Locations { get; set; } = new();

        // Images
        [BsonElement("images")]
        public List<VehicleImage> Images { get; set; } = new();

        // Videos
        [BsonElement("videos")]
        public List<VehicleVideo> Videos { get; set; } = new();

        // Seller Information
        [BsonElement("seller")]
        public Seller Seller { get; set; } = new();

        // Status & Visibility
        [BsonElement("status")]
        public VehicleStatus Status { get; set; } = new();

        // Dates
        [BsonElement("dates")]
        public VehicleDates Dates { get; set; } = new();

        // Analytics
        [BsonElement("analytics")]
        public VehicleAnalytics Analytics { get; set; } = new();

        // Monetization & Promotion
        [BsonElement("monetization")]
        public Monetization Monetization { get; set; } = new();

        // RC & Documentation
        [BsonElement("documents")]
        public VehicleDocuments Documents { get; set; } = new();

        // Additional Metadata
        [BsonElement("metadata")]
        public VehicleMetadata Metadata { get; set; } = new();

        // Inspection
        [BsonElement("inspection")]
        public VehicleInspection Inspection { get; set; } = new();

        // Vehicle History (New)
        [BsonElement("vehicle_history")]
        public VehicleHistory VehicleHistory { get; set; } = new();

        // Service Records
        [BsonElement("service_records")]
        public List<ServiceRecord> ServiceRecords { get; set; } = new();
    }
}
