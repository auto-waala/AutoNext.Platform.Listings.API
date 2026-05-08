namespace AutoNext.Platform.Listings.API.Models
{
    public class VehicleSearchCriteria
    {
        public string? Make { get; set; }
        public string? Model { get; set; }
        public int? MinYear { get; set; }
        public int? MaxYear { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? FuelType { get; set; }
        public string? Transmission { get; set; }
        public string? CityName { get; set; }
        public string? VehicleType { get; set; }
        public int? MaxKilometers { get; set; }
        public string? Color { get; set; }
    }
}
