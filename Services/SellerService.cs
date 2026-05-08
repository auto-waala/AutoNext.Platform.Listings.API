using AutoNext.Platform.Listings.API.Models;
using AutoNext.Platform.Listings.API.Repositories;
using Microsoft.Extensions.Logging;

namespace AutoNext.Platform.Listings.API.Services
{
    public class SellerService : ISellerService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly ILogger<SellerService> _logger;

        public SellerService(IVehicleRepository vehicleRepository, ILogger<SellerService> logger)
        {
            _vehicleRepository = vehicleRepository;
            _logger = logger;
        }

        public async Task<Seller> GetSellerByIdAsync(string sellerId)
        {
            // Get seller from user service or separate seller collection
            // For now, get from first vehicle of seller
            var vehicles = await _vehicleRepository.GetVehiclesBySellerAsync(sellerId);
            var firstVehicle = vehicles.FirstOrDefault();

            return firstVehicle?.Seller ?? throw new KeyNotFoundException($"Seller with ID {sellerId} not found");
        }

        public async Task<Seller> GetSellerByUserIdAsync(string userId)
        {
            return await GetSellerByIdAsync(userId);
        }

        public async Task<Seller> GetSellerByEmailAsync(string email)
        {
            var vehicles = await _vehicleRepository.GetActiveVehiclesAsync(1, 1000);
            var seller = vehicles.Select(v => v.Seller).FirstOrDefault(s => s.Email == email);

            return seller ?? throw new KeyNotFoundException($"Seller with Email {email} not found");
        }

        public async Task<Seller> GetSellerByPhoneAsync(string phone)
        {
            var vehicles = await _vehicleRepository.GetActiveVehiclesAsync(1, 1000);
            var seller = vehicles.Select(v => v.Seller).FirstOrDefault(s => s.Phone == phone);

            return seller ?? throw new KeyNotFoundException($"Seller with Phone {phone} not found");
        }

        public async Task<Seller> CreateSellerAsync(Seller seller)
        {
            // Implementation would save to separate seller collection
            _logger.LogInformation("Creating seller: {@Seller}", seller);
            return await Task.FromResult(seller);
        }

        public async Task<Seller> UpdateSellerAsync(string sellerId, Seller seller)
        {
            _logger.LogInformation("Updating seller: {SellerId}", sellerId);
            return await Task.FromResult(seller);
        }

        public async Task<bool> DeleteSellerAsync(string sellerId)
        {
            _logger.LogInformation("Deleting seller: {SellerId}", sellerId);
            return await Task.FromResult(true);
        }

        public async Task<bool> VerifySellerAsync(string sellerId, string verifiedBy)
        {
            _logger.LogInformation("Verifying seller: {SellerId} by {VerifiedBy}", sellerId, verifiedBy);
            return await Task.FromResult(true);
        }

        public async Task<bool> SuspendSellerAsync(string sellerId, string reason, string suspendedBy)
        {
            _logger.LogInformation("Suspending seller: {SellerId} by {SuspendedBy}. Reason: {Reason}", sellerId, suspendedBy, reason);
            return await Task.FromResult(true);
        }

        public async Task<bool> ReactivateSellerAsync(string sellerId, string reactivatedBy)
        {
            _logger.LogInformation("Reactivating seller: {SellerId} by {ReactivatedBy}", sellerId, reactivatedBy);
            return await Task.FromResult(true);
        }

        public async Task<IEnumerable<Vehicle>> GetSellerListingsAsync(string sellerId, bool includeInactive = false)
        {
            var vehicles = await _vehicleRepository.GetVehiclesBySellerAsync(sellerId);

            if (!includeInactive)
                vehicles = vehicles.Where(v => v.IsActive && v.Status.CurrentStatus == "active");

            return vehicles;
        }

        public async Task<int> GetSellerActiveListingsCountAsync(string sellerId)
        {
            var vehicles = await GetSellerListingsAsync(sellerId, false);
            return vehicles.Count();
        }

        public async Task<decimal> GetSellerTotalListingValueAsync(string sellerId)
        {
            var vehicles = await GetSellerListingsAsync(sellerId, false);
            return vehicles.Sum(v => v.Price.Raw);
        }

        public async Task<SellerAnalytics> GetSellerAnalyticsAsync(string sellerId)
        {
            var vehicles = await GetSellerListingsAsync(sellerId, true);

            return new SellerAnalytics
            {
                SellerId = sellerId,
                TotalListings = vehicles.Count(),
                ActiveListings = vehicles.Count(v => v.Status.CurrentStatus == "active"),
                SoldListings = vehicles.Count(v => v.Status.CurrentStatus == "sold"),
                TotalViews = vehicles.Sum(v => v.Analytics.Views),
                TotalInquiries = vehicles.Sum(v => v.Analytics.Replies),
                AverageRating = 4.5m, // Would come from ratings service
                TotalValue = vehicles.Sum(v => v.Price.Raw)
            };
        }

        public async Task<IEnumerable<SellerPerformance>> GetTopSellersAsync(int limit = 10)
        {
            // Get all unique seller IDs
            var allVehicles = await _vehicleRepository.GetActiveVehiclesAsync(1, 1000);
            var sellerGroups = allVehicles.GroupBy(v => v.Seller.UserId);

            var performances = new List<SellerPerformance>();

            foreach (var group in sellerGroups)
            {
                var analytics = await GetSellerAnalyticsAsync(group.Key);
                performances.Add(new SellerPerformance
                {
                    SellerId = group.Key,
                    SellerName = group.First().Seller.Name,
                    TotalListings = analytics.TotalListings,
                    ActiveListings = analytics.ActiveListings,
                    TotalViews = analytics.TotalViews,
                    AverageRating = analytics.AverageRating
                });
            }

            return performances.OrderByDescending(p => p.TotalListings).Take(limit);
        }

        public async Task<Dictionary<string, int>> GetSellerListingStatsByCategoryAsync(string sellerId)
        {
            var vehicles = await GetSellerListingsAsync(sellerId, true);

            return vehicles
                .GroupBy(v => v.VehicleType.Type)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public async Task<bool> RequestSellerVerificationAsync(string sellerId)
        {
            _logger.LogInformation("Verification requested for seller: {SellerId}", sellerId);
            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateSellerVerificationStatusAsync(string sellerId, string status, string? remarks = null)
        {
            _logger.LogInformation("Updated verification status for seller {SellerId} to {Status}. Remarks: {Remarks}", sellerId, status, remarks);
            return await Task.FromResult(true);
        }

        public async Task<IEnumerable<Seller>> GetPendingVerificationSellersAsync(int page = 1, int pageSize = 20)
        {
            // Implementation would get from separate seller collection
            return await Task.FromResult(new List<Seller>());
        }

        public async Task<bool> AddSellerRatingAsync(string sellerId, decimal rating, string reviewerId, string? reviewText = null)
        {
            _logger.LogInformation("Rating added for seller {SellerId}: {Rating} stars by {ReviewerId}", sellerId, rating, reviewerId);
            return await Task.FromResult(true);
        }

        public async Task<decimal> GetSellerAverageRatingAsync(string sellerId)
        {
            // Would come from ratings service
            return await Task.FromResult(4.5m);
        }

        public async Task<IEnumerable<SellerReview>> GetSellerReviewsAsync(string sellerId, int page = 1, int pageSize = 20)
        {
            // Would come from reviews service
            return await Task.FromResult(new List<SellerReview>());
        }

        public async Task<SellerDashboard> GetSellerDashboardAsync(string sellerId)
        {
            var analytics = await GetSellerAnalyticsAsync(sellerId);
            var recentListings = await GetSellerListingsAsync(sellerId, false);

            return new SellerDashboard
            {
                SellerId = sellerId,
                Analytics = analytics,
                RecentListings = recentListings.Take(5),
                TotalEarnings = analytics.TotalValue * 0.95m, // Assuming 5% platform fee
                PendingPayout = analytics.TotalValue * 0.05m
            };
        }

        public async Task<IEnumerable<Vehicle>> GetSellerRecentActivityAsync(string sellerId, int days = 30)
        {
            var vehicles = await GetSellerListingsAsync(sellerId, true);
            var cutoffDate = DateTime.UtcNow.AddDays(-days);

            return vehicles.Where(v => v.CreatedOn >= cutoffDate || v.ModifiedOn >= cutoffDate);
        }

        public async Task<SellerPreferences> GetSellerPreferencesAsync(string sellerId)
        {
            // Would come from preferences service
            return await Task.FromResult(new SellerPreferences());
        }

        public async Task<SellerPreferences> UpdateSellerPreferencesAsync(string sellerId, SellerPreferences preferences)
        {
            _logger.LogInformation("Updated preferences for seller: {SellerId}", sellerId);
            return await Task.FromResult(preferences);
        }

        public async Task<bool> IsSellerEmailUniqueAsync(string email, string? excludeSellerId = null)
        {
            try
            {
                await GetSellerByEmailAsync(email);
                return false; // Email exists
            }
            catch
            {
                return true; // Email is unique
            }
        }

        public async Task<bool> IsSellerPhoneUniqueAsync(string phone, string? excludeSellerId = null)
        {
            try
            {
                await GetSellerByPhoneAsync(phone);
                return false; // Phone exists
            }
            catch
            {
                return true; // Phone is unique
            }
        }

        public async Task<bool> CanSellerCreateListingAsync(string sellerId)
        {
            var activeListings = await GetSellerActiveListingsCountAsync(sellerId);
            return activeListings < 50; // Max 50 active listings per seller
        }
    }
}