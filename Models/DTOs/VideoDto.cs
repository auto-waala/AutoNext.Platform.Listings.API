namespace AutoNext.Platform.Listings.API.Models.DTOs
{
    public class VideoDto
    {
        public string FileUrl { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public string? Duration { get; set; }
    }
}
