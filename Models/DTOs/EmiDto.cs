namespace AutoNext.Platform.Listings.API.Models.DTOs
{
    public class EmiDto
    {
        public int Emi { get; set; }
        public int Months { get; set; }
        public double InterestRate { get; set; }
        public string DisplayValue { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string LoanUrl { get; set; } = string.Empty;
        public string CarVariantId { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string DownPayment { get; set; } = string.Empty;
        public string VariantSlug { get; set; } = string.Empty;
        public bool ApplicableForloEMI { get; set; } = false;
        public string Title { get; set; } = string.Empty;
        public bool ShowEmiPopup { get; set; } = true;
        public bool DefaultCity { get; set; } = false;
        public string DisplayText { get; set; } = string.Empty;
        public string EmailIdRequired { get; set; } = "0";
        public string SubTitle { get; set; } = string.Empty;
        public string Img { get; set; } = string.Empty;
        public string EmiText { get; set; } = string.Empty;
        public int ModelId { get; set; }
    }
}
