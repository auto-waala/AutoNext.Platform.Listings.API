namespace AutoNext.Platform.Listings.API.Models.DTOs
{
    public class VariantDetailDto
    {
        public string Color { get; set; } = string.Empty;
        public string ColorCode { get; set; } = string.Empty;
        public string Engine { get; set; } = string.Empty;
        public string Transmission { get; set; } = string.Empty;
        public string FuelType { get; set; } = string.Empty;
        public string Mileage { get; set; } = string.Empty;
        public string YearOfManufacture { get; set; } = string.Empty;
        public decimal? Price { get; set; }
    }
}
