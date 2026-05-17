using MongoDB.Bson.Serialization.Attributes;

namespace AutoNext.Platform.Listings.API.Models.Entities
{
    public class VehicleCondition
    {
        [BsonElement("is_new")]
        public bool IsNew { get; set; } = false;

        [BsonElement("owner_count")]
        public int OwnerCount { get; set; } = 0;

        [BsonElement("km_driven")]
        public int KMDriven { get; set; } = 0;

        [BsonElement("accidental")]
        public bool Accidental { get; set; } = false;

        [BsonElement("service_history_available")]
        public bool ServiceHistoryAvailable { get; set; } = true;

        [BsonElement("registration_year")]
        public int RegistrationYear { get; set; }

        [BsonElement("registration_month")]
        public int RegistrationMonth { get; set; }
    }
}
