namespace AutoNext.Platform.Listings.API.Models.DTOs
{
    public class VideoDto
    {
        public string Image { get; set; } = string.Empty;
        public string WebpImage { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;  // YouTube URL
        public string Type { get; set; } = "youtube";   // youtube, vimeo
        public string Duration { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string VideoId { get; set; } = string.Empty; // YouTube video ID
        public string AuthorName { get; set; } = string.Empty;
        public int NoOfViewer { get; set; }
        public string ViewCountText { get; set; } = string.Empty;
        public string DaysText { get; set; } = string.Empty;
    }
}
