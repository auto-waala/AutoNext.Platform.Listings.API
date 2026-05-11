namespace AutoNext.Platform.Listings.API.Models.DTOs
{
    public class OptionalAccessoriesDto
    {
        public string TotalAccessoriesInRs { get; set; } = string.Empty;
        public decimal TotalAccessories { get; set; }
        public List<ChargeDto> List { get; set; } = new();
    }
}
