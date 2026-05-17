namespace AutoNext.Platform.Listings.API.Models.DTOs
{
    public class UsedVehiclesSummaryDto
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public string ModelName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Thumbnail { get; set; }
        public int RegistrationYear { get; set; }
        public int KMDriven { get; set; }
        public string City { get; set; } = string.Empty;
        public double Rating { get; set; }
        public bool IsVerified { get; set; }
        public bool IsSold { get; set; }
        public DateTime PostedDate { get; set; }
    }
}
