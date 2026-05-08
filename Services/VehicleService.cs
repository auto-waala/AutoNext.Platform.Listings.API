using AutoNext.Platform.Listings.API.Models;
using AutoNext.Platform.Listings.API.Repositories;
using Microsoft.Extensions.Logging;

namespace AutoNext.Platform.Listings.API.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly ILogger<VehicleService> _logger;

        public VehicleService(IVehicleRepository vehicleRepository, ILogger<VehicleService> logger)
        {
            _vehicleRepository = vehicleRepository;
            _logger = logger;
        }

        public async Task<Vehicle> GetVehicleByIdAsync(string id)
        {
            try
            {
                var vehicle = await _vehicleRepository.GetByIdAsync(id);
                if (vehicle == null)
                    throw new KeyNotFoundException($"Vehicle with ID {id} not found");

                return vehicle;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting vehicle by ID: {Id}", id);
                throw;
            }
        }

        public async Task<Vehicle> GetVehicleByVINAsync(string vin)
        {
            try
            {
                var vehicle = await _vehicleRepository.GetByVINAsync(vin);
                if (vehicle == null)
                    throw new KeyNotFoundException($"Vehicle with VIN {vin} not found");

                return vehicle;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting vehicle by VIN: {Vin}", vin);
                throw;
            }
        }

        public async Task<Vehicle> GetVehicleByChassisNumberAsync(string chassisNumber)
        {
            try
            {
                var vehicle = await _vehicleRepository.GetByChassisNumberAsync(chassisNumber);
                if (vehicle == null)
                    throw new KeyNotFoundException($"Vehicle with Chassis Number {chassisNumber} not found");

                return vehicle;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting vehicle by Chassis Number: {ChassisNumber}", chassisNumber);
                throw;
            }
        }

        public async Task<Vehicle> GetVehicleByEngineNumberAsync(string engineNumber)
        {
            try
            {
                var vehicle = await _vehicleRepository.GetByEngineNumberAsync(engineNumber);
                if (vehicle == null)
                    throw new KeyNotFoundException($"Vehicle with Engine Number {engineNumber} not found");

                return vehicle;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting vehicle by Engine Number: {EngineNumber}", engineNumber);
                throw;
            }
        }

        public async Task<Vehicle> GetVehicleByRegistrationNumberAsync(string registrationNumber)
        {
            try
            {
                var vehicle = await _vehicleRepository.GetByRegistrationNumberAsync(registrationNumber);
                if (vehicle == null)
                    throw new KeyNotFoundException($"Vehicle with Registration Number {registrationNumber} not found");

                return vehicle;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting vehicle by Registration Number: {RegistrationNumber}", registrationNumber);
                throw;
            }
        }

        public async Task<IEnumerable<Vehicle>> GetAllActiveVehiclesAsync(int page = 1, int pageSize = 20)
        {
            try
            {
                return await _vehicleRepository.GetActiveVehiclesAsync(page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all active vehicles. Page: {Page}, PageSize: {PageSize}", page, pageSize);
                throw;
            }
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesBySellerAsync(string sellerId)
        {
            try
            {
                return await _vehicleRepository.GetVehiclesBySellerAsync(sellerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting vehicles by seller: {SellerId}", sellerId);
                throw;
            }
        }

        public async Task<Vehicle> CreateVehicleAsync(Vehicle vehicle, string createdBy)
        {
            try
            {
                vehicle.CreatedBy = createdBy;
                vehicle.ModifiedBy = createdBy;

                var createdVehicle = await _vehicleRepository.CreateAsync(vehicle);
                _logger.LogInformation("Vehicle created successfully: {VehicleId} by {CreatedBy}", createdVehicle.VehicleId, createdBy);

                return createdVehicle;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating vehicle. CreatedBy: {CreatedBy}", createdBy);
                throw;
            }
        }

        public async Task<Vehicle> UpdateVehicleAsync(string id, Vehicle vehicle, string modifiedBy)
        {
            try
            {
                vehicle.Id = id;
                vehicle.ModifiedBy = modifiedBy;

                var updatedVehicle = await _vehicleRepository.UpdateAsync(vehicle);
                _logger.LogInformation("Vehicle updated successfully: {VehicleId} by {ModifiedBy}", id, modifiedBy);

                return updatedVehicle;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating vehicle: {Id} by {ModifiedBy}", id, modifiedBy);
                throw;
            }
        }

        public async Task<bool> DeleteVehicleAsync(string id)
        {
            try
            {
                var result = await _vehicleRepository.DeleteAsync(id);
                if (result)
                    _logger.LogInformation("Vehicle deleted successfully: {Id}", id);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting vehicle: {Id}", id);
                throw;
            }
        }

        public async Task<bool> SoftDeleteVehicleAsync(string id, string modifiedBy)
        {
            try
            {
                var result = await _vehicleRepository.SoftDeleteAsync(id);
                if (result)
                    _logger.LogInformation("Vehicle soft deleted successfully: {Id} by {ModifiedBy}", id, modifiedBy);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error soft deleting vehicle: {Id} by {ModifiedBy}", id, modifiedBy);
                throw;
            }
        }

        public async Task<IEnumerable<Vehicle>> SearchVehiclesAsync(VehicleSearchCriteria criteria)
        {
            try
            {
                return await _vehicleRepository.SearchVehiclesAsync(criteria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching vehicles with criteria: {@Criteria}", criteria);
                throw;
            }
        }

        public async Task<IEnumerable<Vehicle>> GetFeaturedVehiclesAsync(int limit = 10)
        {
            try
            {
                return await _vehicleRepository.GetFeaturedVehiclesAsync(limit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting featured vehicles. Limit: {Limit}", limit);
                throw;
            }
        }

        public async Task<IEnumerable<Vehicle>> GetSimilarVehiclesAsync(string vehicleId, int limit = 5)
        {
            try
            {
                return await _vehicleRepository.GetSimilarVehiclesAsync(vehicleId, limit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting similar vehicles for: {VehicleId}", vehicleId);
                throw;
            }
        }

        public async Task<IEnumerable<Vehicle>> GetRecentVehiclesAsync(int hours = 24)
        {
            try
            {
                return await _vehicleRepository.GetRecentVehiclesAsync(hours);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent vehicles. Hours: {Hours}", hours);
                throw;
            }
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesByPriceRangeAsync(decimal minPrice, decimal maxPrice, int limit = 50)
        {
            try
            {
                return await _vehicleRepository.GetVehiclesByPriceRangeAsync(minPrice, maxPrice, limit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting vehicles by price range: {MinPrice} - {MaxPrice}", minPrice, maxPrice);
                throw;
            }
        }

        public async Task<bool> ActivateVehicleAsync(string id, string modifiedBy)
        {
            return await _vehicleRepository.UpdateVehicleStatusAsync(id, "active", modifiedBy);
        }

        public async Task<bool> DeactivateVehicleAsync(string id, string modifiedBy)
        {
            return await _vehicleRepository.UpdateVehicleStatusAsync(id, "inactive", modifiedBy);
        }

        public async Task<bool> MarkVehicleAsSoldAsync(string id, string modifiedBy, DateTime? soldDate = null)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle == null)
                return false;

            vehicle.Status.CurrentStatus = "sold";
            vehicle.Dates.SoldDate = soldDate ?? DateTime.UtcNow;

            await _vehicleRepository.UpdateAsync(vehicle);
            _logger.LogInformation("Vehicle marked as sold: {Id} by {ModifiedBy}", id, modifiedBy);

            return true;
        }

        public async Task<bool> ExtendVehicleListingAsync(string id, int additionalDays, string modifiedBy)
        {
            return await _vehicleRepository.ExtendListingAsync(id, additionalDays, modifiedBy);
        }

        public async Task<bool> RecordVehicleViewAsync(string id)
        {
            return await _vehicleRepository.IncrementVehicleViewsAsync(id);
        }

        public async Task<bool> RecordVehicleFavoriteAsync(string id)
        {
            return await _vehicleRepository.IncrementFavoritesAsync(id);
        }

        public async Task<VehicleAnalytics> GetVehicleAnalyticsAsync(string id)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            return vehicle?.Analytics ?? new VehicleAnalytics();
        }

        public async Task<Dictionary<string, long>> GetVehicleStatisticsByMakeAsync()
        {
            return await _vehicleRepository.GetVehicleCountByMakeAsync();
        }

        public async Task<long> GetTotalActiveVehiclesCountAsync()
        {
            return await _vehicleRepository.GetTotalCountAsync();
        }

        public async Task<IEnumerable<Vehicle>> BulkCreateVehiclesAsync(IEnumerable<Vehicle> vehicles, string createdBy)
        {
            var createdVehicles = new List<Vehicle>();

            foreach (var vehicle in vehicles)
            {
                vehicle.CreatedBy = createdBy;
                vehicle.ModifiedBy = createdBy;
                var created = await _vehicleRepository.CreateAsync(vehicle);
                createdVehicles.Add(created);
            }

            _logger.LogInformation("Bulk created {Count} vehicles by {CreatedBy}", createdVehicles.Count, createdBy);
            return createdVehicles;
        }

        public async Task<bool> BulkUpdateVehicleStatusAsync(IEnumerable<string> ids, string status, string modifiedBy)
        {
            var success = true;

            foreach (var id in ids)
            {
                var result = await _vehicleRepository.UpdateVehicleStatusAsync(id, status, modifiedBy);
                if (!result) success = false;
            }

            return success;
        }

        public async Task<bool> BulkSoftDeleteVehiclesAsync(IEnumerable<string> ids, string modifiedBy)
        {
            var success = true;

            foreach (var id in ids)
            {
                var result = await _vehicleRepository.SoftDeleteAsync(id);
                if (!result) success = false;
            }

            return success;
        }

        public async Task<bool> IsVINUniqueAsync(string vin, string? excludeVehicleId = null)
        {
            var vehicle = await _vehicleRepository.GetByVINAsync(vin);
            return vehicle == null || vehicle.Id == excludeVehicleId;
        }

        public async Task<bool> IsChassisNumberUniqueAsync(string chassisNumber, string? excludeVehicleId = null)
        {
            var vehicle = await _vehicleRepository.GetByChassisNumberAsync(chassisNumber);
            return vehicle == null || vehicle.Id == excludeVehicleId;
        }

        public async Task<bool> IsEngineNumberUniqueAsync(string engineNumber, string? excludeVehicleId = null)
        {
            var vehicle = await _vehicleRepository.GetByEngineNumberAsync(engineNumber);
            return vehicle == null || vehicle.Id == excludeVehicleId;
        }

        public async Task<bool> IsRegistrationNumberUniqueAsync(string registrationNumber, string? excludeVehicleId = null)
        {
            var vehicle = await _vehicleRepository.GetByRegistrationNumberAsync(registrationNumber);
            return vehicle == null || vehicle.Id == excludeVehicleId;
        }

        public async Task<IEnumerable<Vehicle>> GetExpiringListingsAsync(int daysThreshold = 7)
        {
            return await _vehicleRepository.GetExpiringListingsAsync(daysThreshold);
        }

        public async Task<int> ProcessExpiredListingsAsync()
        {
            var expiredVehicles = await _vehicleRepository.GetExpiringListingsAsync(0);
            var processedCount = 0;

            foreach (var vehicle in expiredVehicles)
            {
                if (vehicle.Dates.ValidTo < DateTime.UtcNow)
                {
                    vehicle.Status.CurrentStatus = "expired";
                    await _vehicleRepository.UpdateAsync(vehicle);
                    processedCount++;
                }
            }

            _logger.LogInformation("Processed {Count} expired listings", processedCount);
            return processedCount;
        }
    }
}