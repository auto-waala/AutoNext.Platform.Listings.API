namespace AutoNext.Platform.Listings.API.Models.DTOs
{
    public class NewlyArrivedResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public string ModelName { get; set; } = string.Empty;
        public string ModelSlug { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public string BodyType { get; set; } = string.Empty;
        public string PriceRange { get; set; } = string.Empty;
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public DateTime ArrivalDate { get; set; }
        public string ArrivalPeriod { get; set; } = string.Empty;
        public bool Featured { get; set; }
        public EmiDto? Emi { get; set; }
        public List<ImageDto>? Images { get; set; }
        public string? ThumbnailImage { get; set; }
        public List<VideoDto>? Videos { get; set; }
        public List<VariantDetailDto>? Variants { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public string PageTitle { get; set; } = string.Empty;
        public string DescriptionText { get; set; } = string.Empty;
    }
}
