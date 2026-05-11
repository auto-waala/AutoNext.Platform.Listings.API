namespace AutoNext.Platform.Listings.API.Models.DTOs
{
    public class ChargeDto
    {
        public string Price { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string Key { get; set; } = string.Empty;
    }
}
