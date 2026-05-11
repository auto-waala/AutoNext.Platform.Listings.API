using AutoNext.Platform.Listings.API.Models.Common;
using AutoNext.Platform.Listings.API.Models.DTOs;
using AutoNext.Platform.Listings.API.Models.Mappers;
using AutoNext.Platform.Listings.API.Repositories;

namespace AutoNext.Platform.Listings.API.Services
{
    public class NewlyArrivedService : INewlyArrivedService
    {
        private readonly INewlyArrivedRepository _repository;

        public NewlyArrivedService(INewlyArrivedRepository repository)
        {
            _repository = repository;
        }

        public async Task<NewlyArrivedResponseDto?> GetByIdAsync(string id)
        {
            var vehicle = await _repository.GetByIdAsync(id);
            return vehicle?.ToResponseDto();
        }

        public async Task<NewlyArrivedResponseDto?> GetByModelSlugAsync(string modelSlug)
        {
            var vehicle = await _repository.GetByModelSlugAsync(modelSlug);
            return vehicle?.ToResponseDto();
        }

        public async Task<PagedResult<NewlyArrivedResponseDto>> GetAllAsync(int page, int pageSize)
        {
            var vehicles = await _repository.GetAllAsync(page, pageSize);
            var totalCount = await _repository.GetTotalCountAsync();

            return new PagedResult<NewlyArrivedResponseDto>
            {
                Items = vehicles.ToResponseDtoList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<IEnumerable<NewlyArrivedResponseDto>> GetWeeklyArrivalsAsync(int limit = 20)
        {
            var vehicles = await _repository.GetByArrivalPeriodAsync("weekly", limit);
            return vehicles.ToResponseDtoList();
        }

     
        public async Task<IEnumerable<NewlyArrivedResponseDto>> GetMonthlyArrivalsAsync(int month, int year, int limit = 20)
        {
            var vehicles = await _repository.GetMonthlyArrivalsAsync(month, year);
            return vehicles.Take(limit).ToResponseDtoList();
        }

        public async Task<IEnumerable<NewlyArrivedResponseDto>> GetYearlyArrivalsAsync(int year)
        {
            var vehicles = await _repository.GetYearlyArrivalsAsync(year);
            return vehicles.ToResponseDtoList();
        }

        public async Task<IEnumerable<NewlyArrivedResponseDto>> GetFeaturedArrivalsAsync(int limit = 10)
        {
            var vehicles = await _repository.GetFeaturedArrivalsAsync(limit);
            return vehicles.ToResponseDtoList();
        }

        public async Task<NewlyArrivedResponseDto> CreateAsync(NewlyArrivedRequestDto request, string publishedBy)
        {
            var vehicle = request.ToEntity();
            vehicle.PublishedBy = publishedBy;
            var created = await _repository.CreateAsync(vehicle);
            return created.ToResponseDto();
        }

        public async Task<NewlyArrivedResponseDto?> UpdateAsync(string id, NewlyArrivedRequestDto request)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            existing.UpdateFromRequest(request);
            var updated = await _repository.UpdateAsync(id, existing);
            return updated?.ToResponseDto();
        }

        public async Task<bool> DeleteAsync(string id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> PublishAsync(string id, string publishedBy)
        {
            return await _repository.PublishAsync(id, publishedBy);
        }

        public async Task<bool> UnpublishAsync(string id)
        {
            return await _repository.UnpublishAsync(id);
        }

        // ── Private Helpers ───────────────────────────────────────────────────

        private static (DateTime startDate, DateTime endDate) GetWeekRange(int weekNumber, int year)
        {
            var jan1 = new DateTime(year, 1, 1);
            var daysOffset = (int)jan1.DayOfWeek - (int)DayOfWeek.Monday;
            var firstMonday = jan1.AddDays(-daysOffset);
            var startDate = firstMonday.AddDays((weekNumber - 1) * 7);
            var endDate = startDate.AddDays(6);
            return (startDate, endDate);
        }
    }

}
