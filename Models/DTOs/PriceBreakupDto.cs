namespace AutoNext.Platform.Listings.API.Models.DTOs
{
    public class PriceBreakupDto
    {
        public string ExShowRoom { get; set; } = string.Empty;
        public string Rto { get; set; } = string.Empty;
        public string Insurance { get; set; } = string.Empty;
        public OthersChargesDto? Others { get; set; }
        public OptionalAccessoriesDto? OptionalAccessories { get; set; }
        public string OnRoadPrice { get; set; } = string.Empty;
        public decimal OnRoadPriceValue { get; set; }
    }
}
