namespace AutoNext.Platform.Listings.API.Models.DTOs
{
    public class VariantDetailDto
    {
        public string VariantId { get; set; } = string.Empty;
        public string VariantName { get; set; } = string.Empty;
        public string VariantShortName { get; set; } = string.Empty;
        public string VariantSlug { get; set; } = string.Empty;
        public string ExShowRoomPrice { get; set; } = string.Empty;
        public string OnRoadPrice { get; set; } = string.Empty;
        public decimal OnRoadPriceValue { get; set; }
        public string FuelType { get; set; } = string.Empty;
        public string Transmission { get; set; } = string.Empty;
        public string Mileage { get; set; } = string.Empty;
        public string EngineCc { get; set; } = string.Empty;
        public string Emi { get; set; } = string.Empty;
        public string Tag { get; set; } = string.Empty; // "Base Model", "Top Model"
        public bool IsRecentLaunch { get; set; }
        public bool IsTopSelling { get; set; }
        public PriceBreakupDto? PriceBreakup { get; set; }
    }
}
