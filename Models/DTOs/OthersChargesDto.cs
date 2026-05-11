namespace AutoNext.Platform.Listings.API.Models.DTOs
{
    public class OthersChargesDto
    {
        public string TotalOtherChargesInRsFormat { get; set; } = string.Empty;
        public decimal TotalOtherCharges { get; set; }
        public List<ChargeDto> List { get; set; } = new();
    }
}
