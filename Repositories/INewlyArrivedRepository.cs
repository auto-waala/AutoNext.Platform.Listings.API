using AutoNext.Platform.Listings.API.Models.Entities;

namespace AutoNext.Platform.Listings.API.Repositories
{
    public interface INewlyArrivedRepository
    {
        // Queries
        Task<NewlyArrivedVehicle?> GetByIdAsync(string id);
        Task<NewlyArrivedVehicle?> GetByModelSlugAsync(string modelSlug);
        Task<IEnumerable<NewlyArrivedVehicle>> GetAllAsync(int page, int pageSize);

        // Period-specific queries
        Task<IEnumerable<NewlyArrivedVehicle>> GetByArrivalPeriodAsync(string period, int limit = 20);
        Task<IEnumerable<NewlyArrivedVehicle>> GetWeeklyArrivalsAsync(int weekNumber, int year);
        Task<IEnumerable<NewlyArrivedVehicle>> GetMonthlyArrivalsAsync(int month, int year);
        Task<IEnumerable<NewlyArrivedVehicle>> GetYearlyArrivalsAsync(int year);

        // Featured
        Task<IEnumerable<NewlyArrivedVehicle>> GetFeaturedArrivalsAsync(int limit = 10);

        // Counts
        Task<long> GetTotalCountAsync();

        // Commands
        Task<NewlyArrivedVehicle> CreateAsync(NewlyArrivedVehicle vehicle);
        Task<NewlyArrivedVehicle?> UpdateAsync(string id, NewlyArrivedVehicle vehicle);
        Task<bool> DeleteAsync(string id);
        Task<bool> PublishAsync(string id, string publishedBy);
        Task<bool> UnpublishAsync(string id);
    }
}
