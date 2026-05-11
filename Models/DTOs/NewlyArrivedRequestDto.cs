namespace AutoNext.Platform.Listings.API.Models.DTOs
{
    public class NewlyArrivedRequestDto
    {
        public string BrandName { get; set; } = string.Empty;
        public string ModelName { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public string BodyType { get; set; } = string.Empty;
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public string ArrivalPeriod { get; set; } = "weekly";
        public EmiDto? Emi { get; set; }
        public List<ImageDto>? Images { get; set; }
        public List<VideoDto>? Videos { get; set; }
        public List<VariantDetailDto>? Variants { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public string PageTitle { get; set; } = string.Empty;
        public string DescriptionText { get; set; } = string.Empty;
    }
}
