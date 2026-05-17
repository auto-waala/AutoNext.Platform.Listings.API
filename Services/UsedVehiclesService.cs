using AutoNext.Platform.Listings.API.Models.Common;
using AutoNext.Platform.Listings.API.Models.DTOs;
using AutoNext.Platform.Listings.API.Models.Entities;
using AutoNext.Platform.Listings.API.Models.Mappers;
using AutoNext.Platform.Listings.API.Repositories;
using MongoDB.Driver.Core.Servers;
using System.Text.RegularExpressions;

namespace AutoNext.Platform.Listings.API.Services
{
    public class UsedVehiclesService : IUsedVehiclesService
    {
        private readonly IUsedVehiclesRepository _repository;
        private readonly ILogger<UsedVehiclesService> _logger;

        public UsedVehiclesService(
            IUsedVehiclesRepository repository,
            ILogger<UsedVehiclesService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // ─────────────────────────────────────────────────────────────
        // Get Single
        // ─────────────────────────────────────────────────────────────

        public async Task<UsedVehiclesResponseDto?> GetByIdAsync(string id)
        {
            var vehicle = await _repository.GetByIdAsync(id);
            return vehicle?.ToResponseDto();
        }

        public async Task<UsedVehiclesResponseDto?> GetBySlugAsync(string slug)
        {
            var vehicle = await _repository.GetBySlugAsync(slug);
            return vehicle?.ToResponseDto();
        }

        public async Task<UsedVehiclesResponseDto?> GetByModelSlugAsync(string modelSlug)
        {
            var vehicle = await _repository.GetByModelSlugAsync(modelSlug);
            return vehicle?.ToResponseDto();
        }

        // ─────────────────────────────────────────────────────────────
        // Get Collections
        // ─────────────────────────────────────────────────────────────

        public async Task<PagedResult<UsedVehiclesResponseDto>> GetAllAsync(
            int page,
            int pageSize,
            string? sortBy = null,
            string? sortOrder = null)
        {
            var vehicles = await _repository.GetAllAsync(
                page,
                pageSize,
                sortBy,
                sortOrder);

            var totalCount = await _repository.GetTotalCountAsync(true);

            return new PagedResult<UsedVehiclesResponseDto>
            {
                Items = vehicles.ToResponseDtoList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<IEnumerable<UsedVehiclesResponseDto>> GetActiveUsedVehiclesAsync(int limit = 50)
        {
            var vehicles = await _repository.GetActiveUsedVehiclesAsync(limit);
            return vehicles.ToResponseDtoList();
        }

        public async Task<IEnumerable<UsedVehiclesResponseDto>> GetTopPriorityUsedVehiclesAsync(int limit = 10)
        {
            var vehicles = await _repository.GetTopPriorityUsedVehiclesAsync(limit);
            return vehicles.ToResponseDtoList();
        }

        public async Task<IEnumerable<UsedVehiclesResponseDto>> GetRecentlyPostedAsync(int days = 7, int limit = 20)
        {
            var vehicles = await _repository.GetRecentlyPostedAsync(days, limit);
            return vehicles.ToResponseDtoList();
        }

        // ─────────────────────────────────────────────────────────────
        // Filtered Collections
        // ─────────────────────────────────────────────────────────────

        public async Task<IEnumerable<UsedVehiclesResponseDto>> GetByBrandAsync(
            string brandName,
            int limit = 20)
        {
            var vehicles = await _repository.GetByBrandAsync(brandName, limit);
            return vehicles.ToResponseDtoList();
        }

        public async Task<IEnumerable<UsedVehiclesResponseDto>> GetByVehicleTypeAsync(
            string vehicleType,
            int limit = 20)
        {
            var vehicles = await _repository.GetByVehicleTypeAsync(vehicleType, limit);
            return vehicles.ToResponseDtoList();
        }

        public async Task<IEnumerable<UsedVehiclesResponseDto>> GetByPriceRangeAsync(
            decimal minPrice,
            decimal maxPrice,
            int limit = 20)
        {
            var vehicles = await _repository.GetByPriceRangeAsync(
                minPrice,
                maxPrice,
                limit);

            return vehicles.ToResponseDtoList();
        }

        public async Task<IEnumerable<UsedVehiclesResponseDto>> GetByCityAsync(
            string city,
            int limit = 20)
        {
            var vehicles = await _repository.GetByCityAsync(city, limit);
            return vehicles.ToResponseDtoList();
        }

        public async Task<IEnumerable<UsedVehiclesResponseDto>> GetByFuelTypeAsync(
            string fuelType,
            int limit = 20)
        {
            var vehicles = await _repository.GetByFuelTypeAsync(fuelType, limit);
            return vehicles.ToResponseDtoList();
        }

        public async Task<IEnumerable<UsedVehiclesResponseDto>> GetByTransmissionAsync(
            string transmission,
            int limit = 20)
        {
            var vehicles = await _repository.GetByTransmissionAsync(transmission, limit);
            return vehicles.ToResponseDtoList();
        }

        public async Task<IEnumerable<UsedVehiclesResponseDto>> GetByYearRangeAsync(
            int minYear,
            int maxYear,
            int limit = 20)
        {
            var vehicles = await _repository.GetByYearRangeAsync(
                minYear,
                maxYear,
                limit);

            return vehicles.ToResponseDtoList();
        }

        public async Task<IEnumerable<UsedVehiclesResponseDto>> GetBySellerTypeAsync(
            string sellerType,
            int limit = 20)
        {
            var vehicles = await _repository.GetBySellerTypeAsync(sellerType, limit);
            return vehicles.ToResponseDtoList();
        }

        // ─────────────────────────────────────────────────────────────
        // Search
        // ─────────────────────────────────────────────────────────────

        public async Task<IEnumerable<UsedVehiclesResponseDto>> SearchAsync(
            string searchTerm,
            int limit = 20)
        {
            var vehicles = await _repository.SearchAsync(searchTerm, limit);
            return vehicles.ToResponseDtoList();
        }

        public async Task<PagedResult<UsedVehiclesResponseDto>> AdvancedSearchAsync(
            UsedVehiclesSearchCriteria criteria)
        {
            var vehicles = await _repository.AdvancedSearchAsync(criteria);

            return new PagedResult<UsedVehiclesResponseDto>
            {
                Items = vehicles.ToResponseDtoList(),
                TotalCount = vehicles.Count(),
                Page = criteria.Page,
                PageSize = criteria.PageSize
            };
        }

        // ─────────────────────────────────────────────────────────────
        // CRUD Operations
        // ─────────────────────────────────────────────────────────────

        public async Task<UsedVehiclesResponseDto> CreateAsync(
            UsedVehiclesRequestDto request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (!string.IsNullOrWhiteSpace(request.Slug))
            {
                var isSlugUnique = await IsSlugUniqueAsync(request.Slug);

                if (!isSlugUnique)
                {
                    throw new InvalidOperationException(
                        $"Slug '{request.Slug}' already exists");
                }
            }

            var modelSlug = GenerateSlug(
                request.BrandName,
                request.ModelName);

            var isModelSlugUnique = await IsModelSlugUniqueAsync(modelSlug);

            if (!isModelSlugUnique)
            {
                throw new InvalidOperationException(
                    $"Model slug '{modelSlug}' already exists");
            }

            var entity = request.ToEntity();

            var created = await _repository.CreateAsync(entity);

            _logger.LogInformation(
                "Created used vehicle with Id: {Id}, Slug: {Slug}",
                created.Id,
                created.Slug);

            return created.ToResponseDto();
        }

        public async Task<UsedVehiclesResponseDto?> UpdateAsync(
            string id,
            UsedVehiclesRequestDto request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var existing = await _repository.GetByIdAsync(id);

            if (existing == null)
            {
                _logger.LogWarning(
                    "Used vehicle not found for update. Id: {Id}",
                    id);

                return null;
            }

            if (!string.IsNullOrWhiteSpace(request.Slug) &&
                request.Slug != existing.Slug)
            {
                var isSlugUnique = await IsSlugUniqueAsync(
                    request.Slug,
                    id);

                if (!isSlugUnique)
                {
                    throw new InvalidOperationException(
                        $"Slug '{request.Slug}' already exists");
                }
            }

            existing.UpdateFromRequest(request);

            var updated = await _repository.UpdateAsync(id, existing);

            _logger.LogInformation(
                "Updated used vehicle with Id: {Id}",
                id);

            return updated?.ToResponseDto();
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _repository.DeleteAsync(id);

            if (result)
            {
                _logger.LogInformation(
                    "Deleted used vehicle with Id: {Id}",
                    id);
            }

            return result;
        }

        // ─────────────────────────────────────────────────────────────
        // Vehicle Operations
        // ─────────────────────────────────────────────────────────────

        public async Task<bool> UpdatePriorityAsync(
            string id,
            int priority)
        {
            if (priority <= 0)
            {
                throw new ArgumentException(
                    "Priority must be greater than zero");
            }

            var result = await _repository.UpdatePriorityAsync(
                id,
                priority);

            if (result)
            {
                _logger.LogInformation(
                    "Updated vehicle priority. Id: {Id}, Priority: {Priority}",
                    id,
                    priority);
            }

            return result;
        }

        public async Task<bool> ActivateAsync(
            string id,
            DateTime? endDate = null)
        {
            var result = await _repository.ActivateAsync(
                id,
                endDate);

            if (result)
            {
                _logger.LogInformation(
                    "Activated vehicle. Id: {Id}",
                    id);
            }

            return result;
        }

        public async Task<bool> DeactivateAsync(string id)
        {
            var result = await _repository.DeactivateAsync(id);

            if (result)
            {
                _logger.LogInformation(
                    "Deactivated vehicle. Id: {Id}",
                    id);
            }

            return result;
        }

        public async Task<bool> MarkAsSoldAsync(string id)
        {
            var result = await _repository.MarkAsSoldAsync(id);

            if (result)
            {
                _logger.LogInformation(
                    "Marked vehicle as sold. Id: {Id}",
                    id);
            }

            return result;
        }

        public async Task<bool> BulkUpdatePriorityAsync(
            Dictionary<string, int> priorityUpdates)
        {
            if (priorityUpdates == null || !priorityUpdates.Any())
            {
                return false;
            }

            var result = await _repository.BulkUpdatePriorityAsync(
                priorityUpdates);

            if (result)
            {
                _logger.LogInformation(
                    "Bulk updated priorities for {Count} vehicles",
                    priorityUpdates.Count);
            }

            return result;
        }

        // ─────────────────────────────────────────────────────────────
        // Engagement
        // ─────────────────────────────────────────────────────────────

        public async Task<bool> IncrementViewsAsync(string id)
        {
            return await _repository.IncrementViewsAsync(id);
        }

        public async Task<bool> IncrementLikesAsync(string id)
        {
            return await _repository.IncrementLikesAsync(id);
        }

        public async Task<bool> IncrementSharesAsync(string id)
        {
            return await _repository.IncrementSharesAsync(id);
        }

        public async Task<bool> IncrementEnquiriesAsync(string id)
        {
            return await _repository.IncrementEnquiriesAsync(id);
        }

        // ─────────────────────────────────────────────────────────────
        // Rating
        // ─────────────────────────────────────────────────────────────

        public async Task<bool> AddRatingAsync(
            string id,
            UserRatingDto rating)
        {
            if (rating == null)
            {
                throw new ArgumentNullException(nameof(rating));
            }

            if (rating.Rating < 1 || rating.Rating > 5)
            {
                throw new ArgumentException(
                    "Rating must be between 1 and 5");
            }

            var userRating = new UserRating
            {
                UserId = rating.UserId,
                Rating = rating.Rating,
                Comment = rating.Comment,
                CreatedAt = DateTime.UtcNow
            };

            return await _repository.AddRatingAsync(id, userRating);
        }

        // ─────────────────────────────────────────────────────────────
        // Validation
        // ─────────────────────────────────────────────────────────────

        public async Task<bool> IsSlugUniqueAsync(
            string slug,
            string? excludeId = null)
        {
            var vehicle = await _repository.GetBySlugAsync(slug);

            if (vehicle == null)
            {
                return true;
            }

            return excludeId != null &&
                   vehicle.Id == excludeId;
        }

        public async Task<bool> IsModelSlugUniqueAsync(
            string modelSlug,
            string? excludeId = null)
        {
            var vehicle = await _repository.GetByModelSlugAsync(modelSlug);

            if (vehicle == null)
            {
                return true;
            }

            return excludeId != null &&
                   vehicle.Id == excludeId;
        }

        // ─────────────────────────────────────────────────────────────
        // Statistics
        // ─────────────────────────────────────────────────────────────

        public async Task<long> GetTotalCountAsync()
        {
            return await _repository.GetTotalCountAsync();
        }

        public async Task<long> GetActiveCountAsync()
        {
            return await _repository.GetActiveCountAsync();
        }

        public async Task<long> GetSoldCountAsync()
        {
            var vehicles = await _repository.GetAllAsync(1, int.MaxValue);
            return vehicles.Count(v => v.ListingDetails?.IsSold == true);
        }

        // ─────────────────────────────────────────────────────────────
        // Helpers
        // ─────────────────────────────────────────────────────────────

        private static string GenerateSlug(
            string brandName,
            string modelName)
        {
            var slug = $"{brandName}-{modelName}"
                .ToLower()
                .Trim();

            slug = Regex.Replace(slug, @"\s+", "-");
            slug = Regex.Replace(slug, @"[^a-z0-9\-]", "");
            slug = Regex.Replace(slug, @"-+", "-");
            slug = slug.Trim('-');

            return slug;
        }

        public async Task<IEnumerable<UsedVehiclesResponseDto>> GetBySellerAsync(string sellerId, int limit = 20)
        {
            var vehicles = await _repository.GetBySellerAsync(sellerId, limit);
            return vehicles.ToResponseDtoList();
        }
    }
}