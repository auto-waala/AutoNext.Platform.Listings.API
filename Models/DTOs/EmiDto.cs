namespace AutoNext.Platform.Listings.API.Models.DTOs
{
    public class EmiDto
    {
        public decimal LoanAmount { get; set; }
        public int TenureMonths { get; set; }
        public double InterestRate { get; set; }
        public decimal MonthlyEmi { get; set; }
    }
}
