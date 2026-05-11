namespace AutoNext.Platform.Listings.API.Models.DTOs
{
    public class ImageDto
    {
        public string Url { get; set; } = string.Empty;
        public string WebpUrl { get; set; } = string.Empty;
        public string AltText { get; set; } = string.Empty;
        public string Type { get; set; } = "exterior"; 
        public int Order { get; set; }
        public bool IsPrimary { get; set; } = false;
    }
}
