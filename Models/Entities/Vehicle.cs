using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoNext.Platform.Listings.API.Models.Entities
{
    public class Vehicle
    {
        // ── Identity ────────────────────────────────────────────────────────────

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        /// <summary>Vehicle Identification Number — unique per vehicle.</summary>
        [BsonElement("vin")]
        public string? VIN { get; set; }

        /// <summary>Chassis number — unique per vehicle.</summary>
        [BsonElement("chassis_number")]
        public string? ChassisNumber { get; set; }

        /// <summary>Engine number — unique per vehicle.</summary>
        [BsonElement("engine_number")]
        public string? EngineNumber { get; set; }

        // ── Basic listing info ──────────────────────────────────────────────────

        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        // ── Top-level shorthand fields (kept for backward compat + seed data) ──

        [BsonElement("make")]
        public string Make { get; set; } = string.Empty;

        [BsonElement("model")]
        public string Model { get; set; } = string.Empty;

        [BsonElement("year")]
        public int Year { get; set; }

        [BsonElement("kilometers")]
        public int Kilometers { get; set; }

        [BsonElement("fuel_type")]
        public string FuelType { get; set; } = string.Empty;

        [BsonElement("transmission")]
        public string Transmission { get; set; } = string.Empty;

        [BsonElement("color")]
        public string Color { get; set; } = string.Empty;

        [BsonElement("city")]
        public string City { get; set; } = string.Empty;

        [BsonElement("images")]
        public List<string> Images { get; set; } = new();

        // ── Structured price (Price.Raw used for index) ─────────────────────────

        [BsonElement("price")]
        public VehiclePrice Price { get; set; } = new();

        // ── Structured specifications (Specifications.Make/Model used for index) ─

        [BsonElement("specifications")]
        public VehicleSpecifications Specifications { get; set; } = new();

        // ── Seller info (Seller.UserId used for index) ───────────────────────────

        [BsonElement("seller")]
        public SellerInfo Seller { get; set; } = new();

        // ── Legacy flat seller fields (kept for seed data + backward compat) ────

        [BsonElement("seller_id")]
        public string SellerId { get; set; } = string.Empty;

        [BsonElement("seller_name")]
        public string SellerName { get; set; } = string.Empty;

        [BsonElement("seller_phone")]
        public string SellerPhone { get; set; } = string.Empty;

        // ── Inspection ──────────────────────────────────────────────────────────

        [BsonElement("inspection")]
        public InspectionReport? Inspection { get; set; }

        // ── Listing metadata ────────────────────────────────────────────────────

        [BsonElement("benefits")]
        public List<string> Benefits { get; set; } = new();

        [BsonElement("vehicle_type")]
        public string VehicleType { get; set; } = string.Empty; // Car, Bike, Truck, EV, Bicycle

        [BsonElement("body_type")]
        public string BodyType { get; set; } = string.Empty; // Hatchback, Sedan, SUV...

        [BsonElement("locality")]
        public string Locality { get; set; } = string.Empty;

        [BsonElement("views")]
        public int Views { get; set; } = 0;

        [BsonElement("status")]
        public string Status { get; set; } = "active"; // active, sold, expired, draft

        [BsonElement("is_active")]
        public bool IsActive { get; set; } = true;

        // ── Audit fields ────────────────────────────────────────────────────────

        [BsonElement("created_by")]
        public string CreatedBy { get; set; } = string.Empty;

        [BsonElement("created_on")]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [BsonElement("modified_by")]
        public string ModifiedBy { get; set; } = string.Empty;

        [BsonElement("modified_on")]
        public DateTime ModifiedOn { get; set; } = DateTime.UtcNow;
    }

    // ── Nested types ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Structured price object.
    /// <c>Raw</c> is the numeric value used for range queries and sorting.
    /// </summary>
    public class VehiclePrice
    {
        /// <summary>Numeric price — used for index and range filters.</summary>
        [BsonElement("raw")]
        public decimal Raw { get; set; }

        /// <summary>Display string e.g. "₹8.50 Lakh".</summary>
        [BsonElement("display")]
        public string Display { get; set; } = string.Empty;

        /// <summary>Currency code e.g. "INR".</summary>
        [BsonElement("currency")]
        public string Currency { get; set; } = "INR";

        /// <summary>Implicit conversion so you can write <c>vehicle.Price = 850000</c> in seed data.</summary>
        public static implicit operator VehiclePrice(decimal value) =>
            new()
            {
                Raw = value,
                Display = value >= 100000
                    ? $"₹{value / 100000:0.##} Lakh"
                    : $"₹{value:N0}",
                Currency = "INR"
            };
    }

    /// <summary>
    /// Detailed vehicle specifications.
    /// <c>Make</c> and <c>Model</c> are used for compound index.
    /// </summary>
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

        [BsonElement("engine_cc")]
        public int? EngineCC { get; set; }

        [BsonElement("mileage_kmpl")]
        public double? MileageKmpl { get; set; }

        [BsonElement("seating_capacity")]
        public int? SeatingCapacity { get; set; }

        [BsonElement("ownership_count")]
        public int OwnershipCount { get; set; } = 1;
    }


    /// <summary>
    /// Expert inspection report attached to a listing.
    /// </summary>
    public class InspectionReport
    {
        [BsonElement("report_url")]
        public string ReportUrl { get; set; } = string.Empty;

        [BsonElement("inspection_points")]
        public int InspectionPoints { get; set; }

        [BsonElement("inspected_on")]
        public DateTime InspectedOn { get; set; }

        [BsonElement("is_available")]
        public bool IsAvailable { get; set; } = false;

        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;
    }
}