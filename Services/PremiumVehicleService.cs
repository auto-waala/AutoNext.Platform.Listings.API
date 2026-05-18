using AutoNext.Platform.Listings.API.Models.Common;
using AutoNext.Platform.Listings.API.Models.DTOs;
using AutoNext.Platform.Listings.API.Models.Entities;
using AutoNext.Platform.Listings.API.Models.Mappers;
using AutoNext.Platform.Listings.API.Repositories;

namespace AutoNext.Platform.Listings.API.Services
{
    public class PremiumVehicleService : IPremiumVehicleService
    {
        private readonly IPremiumVehicleRepository _repository;
        private readonly ILogger<PremiumVehicleService> _logger;

        public PremiumVehicleService(IPremiumVehicleRepository repository, ILogger<PremiumVehicleService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // Get single

        public async Task<PremiumVehicleResponseDto?> GetByIdAsync(string id)
        {
            var vehicle = await _repository.GetByIdAsync(id);
            return vehicle?.ToResponseDto();
        }

        public async Task<PremiumVehicleResponseDto?> GetBySlugAsync(string slug)
        {
            var vehicle = await _repository.GetBySlugAsync(slug);
            return vehicle?.ToResponseDto();
        }

        public async Task<PremiumVehicleResponseDto?> GetByModelSlugAsync(string modelSlug)
        {
            var vehicle = await _repository.GetByModelSlugAsync(modelSlug);
            return vehicle?.ToResponseDto();
        }

        // Get collections

        public async Task<PagedResult<PremiumVehicleResponseDto>> GetAllAsync(
            int page,
            int pageSize,
            string? sortBy = null,
            string? sortOrder = null)
        {
            var vehicles = await _repository.GetAllAsync(page, pageSize, sortBy, sortOrder);
            var totalCount = await _repository.GetTotalCountAsync(true);

            return new PagedResult<PremiumVehicleResponseDto>
            {
                Items = vehicles.ToResponseDtoList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<IEnumerable<PremiumVehicleSummaryDto>> GetActivePremiumVehiclesAsync(int limit = 50)
        {
            var vehicles = await _repository.GetActivePremiumVehiclesAsync(limit);
            return vehicles.ToSummaryDtoList();
        }

        public async Task<IEnumerable<PremiumVehicleSummaryDto>> GetTopPriorityPremiumVehiclesAsync(int limit = 10)
        {
            var vehicles = await _repository.GetTopPriorityPremiumVehiclesAsync(limit);
            return vehicles.ToSummaryDtoList();
        }

        // Filtered collections

        public async Task<IEnumerable<PremiumVehicleSummaryDto>> GetByBrandAsync(string brandName, int limit = 20)
        {
            var vehicles = await _repository.GetByBrandAsync(brandName, limit);
            return vehicles.ToSummaryDtoList();
        }

        public async Task<IEnumerable<PremiumVehicleSummaryDto>> GetByVehicleTypeAsync(string vehicleType, int limit = 20)
        {
            var vehicles = await _repository.GetByVehicleTypeAsync(vehicleType, limit);
            return vehicles.ToSummaryDtoList();
        }

        public async Task<IEnumerable<PremiumVehicleSummaryDto>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice, int limit = 20)
        {
            var vehicles = await _repository.GetByPriceRangeAsync(minPrice, maxPrice, limit);
            return vehicles.ToSummaryDtoList();
        }

        public async Task<IEnumerable<PremiumVehicleSummaryDto>> GetByCityAsync(string city, int limit = 20)
        {
            var vehicles = await _repository.GetByCityAsync(city, limit);
            return vehicles.ToSummaryDtoList();
        }

        // Search

        public async Task<IEnumerable<PremiumVehicleSummaryDto>> SearchAsync(string searchTerm, int limit = 20)
        {
            var vehicles = await _repository.SearchAsync(searchTerm, limit);
            return vehicles.ToSummaryDtoList();
        }

        // CRUD Operations

        public async Task<PremiumVehicleResponseDto> CreateAsync(PremiumVehicleRequestDto request)
        {
            if (!string.IsNullOrWhiteSpace(request.Slug) && !await IsSlugUniqueAsync(request.Slug))
                throw new InvalidOperationException($"Slug '{request.Slug}' already exists");

            var generatedModelSlug = GenerateSlug(request.BrandName, request.ModelName);
            if (!await IsModelSlugUniqueAsync(generatedModelSlug))
                throw new InvalidOperationException($"Model slug for '{request.BrandName} {request.ModelName}' already exists");

            var vehicle = request.ToEntity();
            var created = await _repository.CreateAsync(vehicle);

            _logger.LogInformation("Created premium vehicle with id: {Id}, slug: {Slug}", created.Id, created.Slug);
            return created.ToResponseDto();
        }

        public async Task<PremiumVehicleResponseDto?> UpdateAsync(string id, PremiumVehicleRequestDto request)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                _logger.LogWarning("Premium vehicle not found for update with id: {Id}", id);
                return null;
            }

            if (!string.IsNullOrWhiteSpace(request.Slug) &&
                request.Slug != existing.Slug &&
                !await IsSlugUniqueAsync(request.Slug, id))
            {
                throw new InvalidOperationException($"Slug '{request.Slug}' already exists");
            }

            existing.UpdateFromRequest(request);
            var updated = await _repository.UpdateAsync(id, existing);

            _logger.LogInformation("Updated premium vehicle with id: {Id}", id);
            return updated?.ToResponseDto();
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _repository.DeleteAsync(id);
            if (result)
            {
                _logger.LogInformation("Deleted premium vehicle with id: {Id}", id);
            }
            return result;
        }

        // Premium-specific operations

        public async Task<bool> UpdatePriorityAsync(string id, int priority)
        {
            if (priority < 1)
                throw new ArgumentException("Priority must be greater than 0");

            var result = await _repository.UpdatePriorityAsync(id, priority);
            if (result)
            {
                _logger.LogInformation("Updated priority for premium vehicle {Id} to {Priority}", id, priority);
            }
            return result;
        }

        public async Task<bool> ActivateAsync(string id, DateTime? endDate = null)
        {
            var result = await _repository.ActivateAsync(id, endDate);
            if (result)
            {
                _logger.LogInformation("Activated premium vehicle with id: {Id}", id);
            }
            return result;
        }

        public async Task<bool> DeactivateAsync(string id)
        {
            var result = await _repository.DeactivateAsync(id);
            if (result)
            {
                _logger.LogInformation("Deactivated premium vehicle with id: {Id}", id);
            }
            return result;
        }

        public async Task<bool> BulkUpdatePriorityAsync(Dictionary<string, int> priorityUpdates)
        {
            if (priorityUpdates == null || priorityUpdates.Count == 0)
                return false;

            var result = await _repository.BulkUpdatePriorityAsync(priorityUpdates);
            _logger.LogInformation("Bulk updated priorities for {Count} premium vehicles", priorityUpdates.Count);
            return result;
        }

        // Engagement

        public async Task<bool> IncrementViewsAsync(string id)
            => await _repository.IncrementViewsAsync(id);

        public async Task<bool> IncrementLikesAsync(string id)
            => await _repository.IncrementLikesAsync(id);

        public async Task<bool> IncrementSharesAsync(string id)
            => await _repository.IncrementSharesAsync(id);

        public async Task<bool> IncrementEnquiriesAsync(string id)
            => await _repository.IncrementEnquiriesAsync(id);

        // Rating

        public async Task<bool> AddRatingAsync(string id, PremiumVehicleUserRatingDto rating)
        {
            if (rating.Rating < 1 || rating.Rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5");

            var userRating = new UserRating
            {
                UserId = rating.UserId,
                Rating = rating.Rating,
                Comment = rating.Comment,
                CreatedAt = DateTime.UtcNow
            };

            return await _repository.AddRatingAsync(id, userRating);
        }

        // Validation

        public async Task<bool> IsSlugUniqueAsync(string slug, string? excludeId = null)
        {
            var vehicle = await _repository.GetBySlugAsync(slug);
            if (vehicle == null) return true;
            return excludeId != null && vehicle.Id == excludeId;
        }

        public async Task<bool> IsModelSlugUniqueAsync(string modelSlug, string? excludeId = null)
        {
            var vehicle = await _repository.GetByModelSlugAsync(modelSlug);
            if (vehicle == null) return true;
            return excludeId != null && vehicle.Id == excludeId;
        }

        private static string GenerateSlug(string brandName, string modelName)
        {
            var slug = $"{brandName}-{modelName}".ToLower();
            slug = slug.Replace(" ", "-");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9-]", "");
            return slug.Trim('-');
        }
    }
}