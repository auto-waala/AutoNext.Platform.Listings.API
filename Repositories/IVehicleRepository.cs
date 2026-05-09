using AutoNext.Platform.Listings.API.Models.DTOs;
using AutoNext.Platform.Listings.API.Models.Entities;

namespace AutoNext.Platform.Listings.API.Repositories
{
    public interface IVehicleRepository
    {
        // ── Queries ───────────────────────────────────────────────────────────

        Task<Vehicle?> GetByIdAsync(string id);
        Task<IEnumerable<Vehicle>> GetAllAsync(int page, int pageSize);
        Task<IEnumerable<Vehicle>> SearchAsync(VehicleSearchRequest request);
        Task<IEnumerable<Vehicle>> GetBySellerAsync(string sellerId, int page, int pageSize);

        // ── Counts ────────────────────────────────────────────────────────────

        Task<long> GetTotalCountAsync();
        Task<long> GetSearchCountAsync(VehicleSearchRequest request);
        Task<long> GetSellerCountAsync(string sellerId);

        // ── Commands ──────────────────────────────────────────────────────────

        Task<Vehicle> CreateAsync(Vehicle vehicle);
        Task<Vehicle?> UpdateAsync(string id, Vehicle vehicle);
        Task<bool> DeleteAsync(string id);                  // hard delete
        Task IncrementViewsAsync(string id);          // atomic $inc
    }
}
