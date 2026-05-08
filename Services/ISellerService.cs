using AutoNext.Platform.Listings.API.Models;

namespace AutoNext.Platform.Listings.API.Services
{
    public interface ISellerService
    {
        // Seller Information
        Task<Seller> GetSellerByIdAsync(string sellerId);
        Task<Seller> GetSellerByUserIdAsync(string userId);
        Task<Seller> GetSellerByEmailAsync(string email);
        Task<Seller> GetSellerByPhoneAsync(string phone);

        // Seller Management
        Task<Seller> CreateSellerAsync(Seller seller);
        Task<Seller> UpdateSellerAsync(string sellerId, Seller seller);
        Task<bool> DeleteSellerAsync(string sellerId);
        Task<bool> VerifySellerAsync(string sellerId, string verifiedBy);
        Task<bool> SuspendSellerAsync(string sellerId, string reason, string suspendedBy);
        Task<bool> ReactivateSellerAsync(string sellerId, string reactivatedBy);

        // Seller Listings
        Task<IEnumerable<Vehicle>> GetSellerListingsAsync(string sellerId, bool includeInactive = false);
        Task<int> GetSellerActiveListingsCountAsync(string sellerId);
        Task<decimal> GetSellerTotalListingValueAsync(string sellerId);

        // Seller Analytics
        Task<SellerAnalytics> GetSellerAnalyticsAsync(string sellerId);
        Task<IEnumerable<SellerPerformance>> GetTopSellersAsync(int limit = 10);
        Task<Dictionary<string, int>> GetSellerListingStatsByCategoryAsync(string sellerId);

        // Seller Verification
        Task<bool> RequestSellerVerificationAsync(string sellerId);
        Task<bool> UpdateSellerVerificationStatusAsync(string sellerId, string status, string? remarks = null);
        Task<IEnumerable<Seller>> GetPendingVerificationSellersAsync(int page = 1, int pageSize = 20);

        // Ratings and Reviews
        Task<bool> AddSellerRatingAsync(string sellerId, decimal rating, string reviewerId, string? reviewText = null);
        Task<decimal> GetSellerAverageRatingAsync(string sellerId);
        Task<IEnumerable<SellerReview>> GetSellerReviewsAsync(string sellerId, int page = 1, int pageSize = 20);

        // Seller Dashboard
        Task<SellerDashboard> GetSellerDashboardAsync(string sellerId);
        Task<IEnumerable<Vehicle>> GetSellerRecentActivityAsync(string sellerId, int days = 30);

        // Seller Preferences
        Task<SellerPreferences> GetSellerPreferencesAsync(string sellerId);
        Task<SellerPreferences> UpdateSellerPreferencesAsync(string sellerId, SellerPreferences preferences);

        // Validation
        Task<bool> IsSellerEmailUniqueAsync(string email, string? excludeSellerId = null);
        Task<bool> IsSellerPhoneUniqueAsync(string phone, string? excludeSellerId = null);
        Task<bool> CanSellerCreateListingAsync(string sellerId);
    }
}