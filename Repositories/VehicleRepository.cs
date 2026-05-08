using AutoNext.Platform.Listings.API.Models;
using AutoNext.Platform.Listings.API.Extensions;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;

namespace AutoNext.Platform.Listings.API.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly IMongoCollection<Vehicle> _vehiclesCollection;
        private readonly IClientSessionHandle? _session;
        private readonly ILogger<VehicleRepository>? _logger;

        public VehicleRepository(IMongoDatabase database, IClientSessionHandle? session = null, ILogger<VehicleRepository>? logger = null)
        {
            _vehiclesCollection = database.GetCollection<Vehicle>("vehicles");
            _session = session;
            _logger = logger;
        }

        public async Task<Vehicle> GetByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    throw new ArgumentException("Id cannot be null or empty", nameof(id));

                var filter = Builders<Vehicle>.Filter.Eq(v => v.Id, id);
                return await _vehiclesCollection.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error getting vehicle by Id: {Id}", id);
                throw new Exception($"Error getting vehicle by Id: {ex.Message}", ex);
            }
        }

        public async Task<Vehicle> GetByVINAsync(string vin)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(vin))
                    return null!;

                var filter = Builders<Vehicle>.Filter.Eq(v => v.VIN, vin.ToUpper().Trim());
                return await _vehiclesCollection.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error getting vehicle by VIN: {Vin}", vin);
                throw new Exception($"Error getting vehicle by VIN: {ex.Message}", ex);
            }
        }

        public async Task<Vehicle> GetByChassisNumberAsync(string chassisNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(chassisNumber))
                    return null!;

                var filter = Builders<Vehicle>.Filter.Eq(v => v.ChassisNumber, chassisNumber.ToUpper().Trim());
                return await _vehiclesCollection.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error getting vehicle by Chassis Number: {ChassisNumber}", chassisNumber);
                throw new Exception($"Error getting vehicle by Chassis Number: {ex.Message}", ex);
            }
        }

        public async Task<Vehicle> GetByEngineNumberAsync(string engineNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(engineNumber))
                    return null!;

                var filter = Builders<Vehicle>.Filter.Eq(v => v.EngineNumber, engineNumber.ToUpper().Trim());
                return await _vehiclesCollection.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error getting vehicle by Engine Number: {EngineNumber}", engineNumber);
                throw new Exception($"Error getting vehicle by Engine Number: {ex.Message}", ex);
            }
        }

        public async Task<Vehicle> GetByRegistrationNumberAsync(string registrationNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(registrationNumber))
                    return null!;

                var filter = Builders<Vehicle>.Filter.Eq(v => v.RegistrationNumber, registrationNumber.ToUpper().Trim());
                return await _vehiclesCollection.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error getting vehicle by Registration Number: {RegistrationNumber}", registrationNumber);
                throw new Exception($"Error getting vehicle by Registration Number: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Vehicle>> GetActiveVehiclesAsync(int page = 1, int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var filter = Builders<Vehicle>.Filter.And(
                    Builders<Vehicle>.Filter.Eq(v => v.IsActive, true),
                    Builders<Vehicle>.Filter.Eq(v => v.Status.CurrentStatus, "active"),
                    Builders<Vehicle>.Filter.Gte(v => v.Dates.ValidTo, DateTime.UtcNow)
                );

                var vehicles = await _vehiclesCollection
                    .Find(filter)
                    .SortByDescending(v => v.CreatedOn)
                    .Skip((page - 1) * pageSize)
                    .Limit(pageSize)
                    .ToListAsync();

                return vehicles;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error getting active vehicles. Page: {Page}, PageSize: {PageSize}", page, pageSize);
                throw new Exception($"Error getting active vehicles: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesBySellerAsync(string sellerId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sellerId))
                    throw new ArgumentException("SellerId cannot be null or empty", nameof(sellerId));

                var filter = Builders<Vehicle>.Filter.Eq(v => v.Seller.UserId, sellerId);
                var vehicles = await _vehiclesCollection
                    .Find(filter)
                    .SortByDescending(v => v.CreatedOn)
                    .ToListAsync();

                return vehicles;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error getting vehicles by seller: {SellerId}", sellerId);
                throw new Exception($"Error getting vehicles by seller: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Vehicle>> SearchVehiclesAsync(VehicleSearchCriteria criteria)
        {
            try
            {
                if (criteria == null)
                    throw new ArgumentNullException(nameof(criteria));

                var filters = new List<FilterDefinition<Vehicle>>();

                // Base filters for active listings
                filters.Add(Builders<Vehicle>.Filter.Eq(v => v.IsActive, true));
                filters.Add(Builders<Vehicle>.Filter.Eq(v => v.Status.CurrentStatus, "active"));
                filters.Add(Builders<Vehicle>.Filter.Gte(v => v.Dates.ValidTo, DateTime.UtcNow));

                // Apply search criteria
                if (!string.IsNullOrWhiteSpace(criteria.Make))
                {
                    filters.Add(Builders<Vehicle>.Filter.Eq(v => v.Specifications.Make, criteria.Make));
                }

                if (!string.IsNullOrWhiteSpace(criteria.Model))
                {
                    filters.Add(Builders<Vehicle>.Filter.Eq(v => v.Specifications.Model, criteria.Model));
                }

                if (criteria.MinYear.HasValue)
                {
                    filters.Add(Builders<Vehicle>.Filter.Gte(v => v.Specifications.Year, criteria.MinYear.Value));
                }

                if (criteria.MaxYear.HasValue)
                {
                    filters.Add(Builders<Vehicle>.Filter.Lte(v => v.Specifications.Year, criteria.MaxYear.Value));
                }

                if (criteria.MinPrice.HasValue)
                {
                    filters.Add(Builders<Vehicle>.Filter.Gte(v => v.Price.Raw, criteria.MinPrice.Value));
                }

                if (criteria.MaxPrice.HasValue)
                {
                    filters.Add(Builders<Vehicle>.Filter.Lte(v => v.Price.Raw, criteria.MaxPrice.Value));
                }

                if (!string.IsNullOrWhiteSpace(criteria.FuelType))
                {
                    filters.Add(Builders<Vehicle>.Filter.Eq(v => v.Specifications.FuelType, criteria.FuelType));
                }

                if (!string.IsNullOrWhiteSpace(criteria.Transmission))
                {
                    filters.Add(Builders<Vehicle>.Filter.Eq(v => v.Specifications.Transmission, criteria.Transmission));
                }

                if (!string.IsNullOrWhiteSpace(criteria.CityName))
                {
                    filters.Add(Builders<Vehicle>.Filter.ElemMatch(v => v.Locations,
                        loc => loc.CityName == criteria.CityName));
                }

                if (!string.IsNullOrWhiteSpace(criteria.VehicleType))
                {
                    filters.Add(Builders<Vehicle>.Filter.Eq(v => v.VehicleType.Type, criteria.VehicleType));
                }

                if (criteria.MaxKilometers.HasValue)
                {
                    filters.Add(Builders<Vehicle>.Filter.Lte(v => v.Specifications.KilometersDriven, criteria.MaxKilometers.Value));
                }

                if (!string.IsNullOrWhiteSpace(criteria.Color))
                {
                    filters.Add(Builders<Vehicle>.Filter.Eq(v => v.Specifications.Color, criteria.Color));
                }

                var finalFilter = filters.Any()
                    ? Builders<Vehicle>.Filter.And(filters)
                    : Builders<Vehicle>.Filter.Empty;

                var vehicles = await _vehiclesCollection
                    .Find(finalFilter)
                    .SortByDescending(v => v.CreatedOn)
                    .ToListAsync();

                return vehicles;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error searching vehicles with criteria: {@Criteria}", criteria);
                throw new Exception($"Error searching vehicles: {ex.Message}", ex);
            }
        }

        public async Task<Vehicle> CreateAsync(Vehicle vehicle)
        {
            try
            {
                if (vehicle == null)
                    throw new ArgumentNullException(nameof(vehicle));

                // Validate unique identifiers
                if (!string.IsNullOrWhiteSpace(vehicle.VIN))
                {
                    var existingVin = await GetByVINAsync(vehicle.VIN);
                    if (existingVin != null)
                        throw new InvalidOperationException($"Vehicle with VIN {vehicle.VIN} already exists");
                }

                if (!string.IsNullOrWhiteSpace(vehicle.ChassisNumber))
                {
                    var existingChassis = await GetByChassisNumberAsync(vehicle.ChassisNumber);
                    if (existingChassis != null)
                        throw new InvalidOperationException($"Vehicle with Chassis Number {vehicle.ChassisNumber} already exists");
                }

                if (!string.IsNullOrWhiteSpace(vehicle.EngineNumber))
                {
                    var existingEngine = await GetByEngineNumberAsync(vehicle.EngineNumber);
                    if (existingEngine != null)
                        throw new InvalidOperationException($"Vehicle with Engine Number {vehicle.EngineNumber} already exists");
                }

                if (!string.IsNullOrWhiteSpace(vehicle.RegistrationNumber))
                {
                    var existingReg = await GetByRegistrationNumberAsync(vehicle.RegistrationNumber);
                    if (existingReg != null)
                        throw new InvalidOperationException($"Vehicle with Registration Number {vehicle.RegistrationNumber} already exists");
                }

                // Set default values
                if (string.IsNullOrWhiteSpace(vehicle.VehicleId))
                    vehicle.VehicleId = vehicle.GenerateVehicleId();

                vehicle.Revision = "1";
                vehicle.Version = 1;
                vehicle.CreatedOn = DateTime.UtcNow;
                vehicle.ModifiedOn = DateTime.UtcNow;

                vehicle.Dates ??= new VehicleDates();
                vehicle.Dates.CreatedAt = DateTime.UtcNow;
                vehicle.Dates.CreatedAtFirst = DateTime.UtcNow;
                vehicle.Dates.LastUpdated = DateTime.UtcNow;
                vehicle.Dates.ValidTo = DateTime.UtcNow.AddDays(30);
                vehicle.Dates.ExpiresOn = DateTime.UtcNow.AddDays(30);

                vehicle.Status ??= new VehicleStatus();
                vehicle.Status.CurrentStatus = "active";
                vehicle.Status.DisplayStatus = "active";
                vehicle.Status.AllowEdit = true;
                vehicle.Status.AllowDeactivate = true;

                // Initialize collections if null
                vehicle.Locations ??= new List<VehicleLocation>();
                vehicle.Images ??= new List<VehicleImage>();
                vehicle.Videos ??= new List<VehicleVideo>();
                vehicle.ServiceRecords ??= new List<ServiceRecord>();
                vehicle.Metadata ??= new VehicleMetadata();
                vehicle.Metadata.Tags ??= new List<string>();
                vehicle.Metadata.SearchKeywords ??= new List<string>();

                vehicle.Analytics ??= new VehicleAnalytics();

                if (_session != null && _session.IsInTransaction)
                    await _vehiclesCollection.InsertOneAsync(_session, vehicle);
                else
                    await _vehiclesCollection.InsertOneAsync(vehicle);

                _logger?.LogInformation("Vehicle created successfully with Id: {Id}, VIN: {VIN}", vehicle.Id, vehicle.VIN);
                return vehicle;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error creating vehicle: {@Vehicle}", vehicle);
                throw new Exception($"Error creating vehicle: {ex.Message}", ex);
            }
        }

        public async Task<Vehicle> UpdateAsync(Vehicle vehicle)
        {
            try
            {
                if (vehicle == null)
                    throw new ArgumentNullException(nameof(vehicle));

                if (string.IsNullOrWhiteSpace(vehicle.Id))
                    throw new ArgumentException("Vehicle Id cannot be null or empty", nameof(vehicle));

                // Ensure vehicle exists
                var existingVehicle = await GetByIdAsync(vehicle.Id);
                if (existingVehicle == null)
                    throw new KeyNotFoundException($"Vehicle with Id {vehicle.Id} not found");

                // Update revision and version
                if (int.TryParse(existingVehicle.Revision, out int revisionNum))
                {
                    vehicle.Revision = (revisionNum + 1).ToString();
                }
                else
                {
                    vehicle.Revision = "2";
                }

                vehicle.Version = existingVehicle.Version + 1;
                vehicle.ModifiedOn = DateTime.UtcNow;

                if (vehicle.Dates == null)
                    vehicle.Dates = new VehicleDates();

                vehicle.Dates.LastUpdated = DateTime.UtcNow;

                // Preserve original creation dates
                vehicle.CreatedOn = existingVehicle.CreatedOn;
                vehicle.Dates.CreatedAt = existingVehicle.Dates.CreatedAt;
                vehicle.Dates.CreatedAtFirst = existingVehicle.Dates.CreatedAtFirst;
                vehicle.Dates.ValidTo = existingVehicle.Dates.ValidTo;
                vehicle.Dates.ExpiresOn = existingVehicle.Dates.ExpiresOn;

                // Preserve analytics if not provided
                if (vehicle.Analytics == null)
                    vehicle.Analytics = existingVehicle.Analytics;

                var filter = Builders<Vehicle>.Filter.Eq(v => v.Id, vehicle.Id);

                if (_session != null && _session.IsInTransaction)
                {
                    var result = await _vehiclesCollection.ReplaceOneAsync(_session, filter, vehicle);
                    if (result.ModifiedCount == 0)
                        throw new Exception($"Failed to update vehicle with Id {vehicle.Id}");
                }
                else
                {
                    var result = await _vehiclesCollection.ReplaceOneAsync(filter, vehicle);
                    if (result.ModifiedCount == 0)
                        throw new Exception($"Failed to update vehicle with Id {vehicle.Id}");
                }

                _logger?.LogInformation("Vehicle updated successfully with Id: {Id}, Version: {Version}", vehicle.Id, vehicle.Version);
                return vehicle;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error updating vehicle: {@Vehicle}", vehicle);
                throw new Exception($"Error updating vehicle: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    throw new ArgumentException("Id cannot be null or empty", nameof(id));

                var filter = Builders<Vehicle>.Filter.Eq(v => v.Id, id);

                DeleteResult result;
                if (_session != null && _session.IsInTransaction)
                    result = await _vehiclesCollection.DeleteOneAsync(_session, filter);
                else
                    result = await _vehiclesCollection.DeleteOneAsync(filter);

                var deleted = result.DeletedCount > 0;

                if (deleted)
                    _logger?.LogInformation("Vehicle deleted successfully with Id: {Id}", id);

                return deleted;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error deleting vehicle with Id: {Id}", id);
                throw new Exception($"Error deleting vehicle: {ex.Message}", ex);
            }
        }

        public async Task<bool> SoftDeleteAsync(string id)
        {
            try
            {
                var vehicle = await GetByIdAsync(id);
                if (vehicle == null)
                    return false;

                vehicle.IsActive = false;
                vehicle.Status.CurrentStatus = "deleted";
                vehicle.Status.DisplayStatus = "deleted";
                vehicle.Status.AllowEdit = false;
                vehicle.Status.AllowDeactivate = false;
                vehicle.ModifiedOn = DateTime.UtcNow;

                if (vehicle.Dates == null)
                    vehicle.Dates = new VehicleDates();
                vehicle.Dates.LastUpdated = DateTime.UtcNow;

                var filter = Builders<Vehicle>.Filter.Eq(v => v.Id, id);

                if (_session != null && _session.IsInTransaction)
                {
                    var result = await _vehiclesCollection.ReplaceOneAsync(_session, filter, vehicle);
                    return result.ModifiedCount > 0;
                }
                else
                {
                    var result = await _vehiclesCollection.ReplaceOneAsync(filter, vehicle);
                    return result.ModifiedCount > 0;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error soft deleting vehicle with Id: {Id}", id);
                throw new Exception($"Error soft deleting vehicle: {ex.Message}", ex);
            }
        }

        public async Task<long> GetTotalCountAsync()
        {
            try
            {
                var filter = Builders<Vehicle>.Filter.Eq(v => v.IsActive, true);
                return await _vehiclesCollection.CountDocumentsAsync(filter);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error getting total count");
                throw new Exception($"Error getting total count: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdateVehicleStatusAsync(string id, string status, string modifiedBy)
        {
            try
            {
                var vehicle = await GetByIdAsync(id);
                if (vehicle == null)
                    return false;

                vehicle.Status.CurrentStatus = status;
                vehicle.Status.DisplayStatus = status;
                vehicle.UpdateTimestamps(modifiedBy);

                var filter = Builders<Vehicle>.Filter.Eq(v => v.Id, id);
                var result = await _vehiclesCollection.ReplaceOneAsync(filter, vehicle);
                return result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error updating vehicle status for Id: {Id}", id);
                throw new Exception($"Error updating vehicle status: {ex.Message}", ex);
            }
        }

        public async Task<bool> IncrementVehicleViewsAsync(string id)
        {
            try
            {
                var update = Builders<Vehicle>.Update.Inc(v => v.Analytics.Views, 1);
                var filter = Builders<Vehicle>.Filter.Eq(v => v.Id, id);
                var result = await _vehiclesCollection.UpdateOneAsync(filter, update);
                return result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error incrementing views for Id: {Id}", id);
                throw new Exception($"Error incrementing views: {ex.Message}", ex);
            }
        }

        public async Task<bool> IncrementFavoritesAsync(string id)
        {
            try
            {
                var update = Builders<Vehicle>.Update.Inc(v => v.Analytics.Favorites, 1);
                var filter = Builders<Vehicle>.Filter.Eq(v => v.Id, id);
                var result = await _vehiclesCollection.UpdateOneAsync(filter, update);
                return result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error incrementing favorites for Id: {Id}", id);
                throw new Exception($"Error incrementing favorites: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Vehicle>> GetFeaturedVehiclesAsync(int limit = 10)
        {
            try
            {
                var filter = Builders<Vehicle>.Filter.And(
                    Builders<Vehicle>.Filter.Eq(v => v.IsActive, true),
                    Builders<Vehicle>.Filter.Eq(v => v.Status.CurrentStatus, "active"),
                    Builders<Vehicle>.Filter.Eq(v => v.Monetization.IsFeatured, true),
                    Builders<Vehicle>.Filter.Gte(v => v.Monetization.FeaturedUntil, DateTime.UtcNow)
                );

                var vehicles = await _vehiclesCollection
                    .Find(filter)
                    .SortByDescending(v => v.Monetization.FeaturedUntil)
                    .Limit(limit)
                    .ToListAsync();

                return vehicles;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error getting featured vehicles");
                throw new Exception($"Error getting featured vehicles: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Vehicle>> GetSimilarVehiclesAsync(string vehicleId, int limit = 5)
        {
            try
            {
                var vehicle = await GetByIdAsync(vehicleId);
                if (vehicle == null)
                    return new List<Vehicle>();

                var filter = Builders<Vehicle>.Filter.And(
                    Builders<Vehicle>.Filter.Ne(v => v.Id, vehicleId),
                    Builders<Vehicle>.Filter.Eq(v => v.IsActive, true),
                    Builders<Vehicle>.Filter.Eq(v => v.Status.CurrentStatus, "active"),
                    Builders<Vehicle>.Filter.Eq(v => v.Specifications.Make, vehicle.Specifications.Make),
                    Builders<Vehicle>.Filter.Eq(v => v.Specifications.FuelType, vehicle.Specifications.FuelType)
                );

                var similarVehicles = await _vehiclesCollection
                    .Find(filter)
                    .SortBy(v => Math.Abs(v.Price.Raw - vehicle.Price.Raw))
                    .Limit(limit)
                    .ToListAsync();

                return similarVehicles;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error getting similar vehicles for Id: {VehicleId}", vehicleId);
                throw new Exception($"Error getting similar vehicles: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Vehicle>> GetRecentVehiclesAsync(int hours = 24)
        {
            try
            {
                var cutoffTime = DateTime.UtcNow.AddHours(-hours);
                var filter = Builders<Vehicle>.Filter.And(
                    Builders<Vehicle>.Filter.Eq(v => v.IsActive, true),
                    Builders<Vehicle>.Filter.Eq(v => v.Status.CurrentStatus, "active"),
                    Builders<Vehicle>.Filter.Gte(v => v.CreatedOn, cutoffTime)
                );

                var vehicles = await _vehiclesCollection
                    .Find(filter)
                    .SortByDescending(v => v.CreatedOn)
                    .ToListAsync();

                return vehicles;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error getting recent vehicles for last {Hours} hours", hours);
                throw new Exception($"Error getting recent vehicles: {ex.Message}", ex);
            }
        }

        public async Task<bool> ExtendListingAsync(string id, int additionalDays, string modifiedBy)
        {
            try
            {
                var vehicle = await GetByIdAsync(id);
                if (vehicle == null)
                    return false;

                vehicle.Dates.ValidTo = vehicle.Dates.ValidTo.AddDays(additionalDays);
                vehicle.Dates.ExpiresOn = vehicle.Dates.ExpiresOn.AddDays(additionalDays);
                vehicle.UpdateTimestamps(modifiedBy);

                var filter = Builders<Vehicle>.Filter.Eq(v => v.Id, id);
                var result = await _vehiclesCollection.ReplaceOneAsync(filter, vehicle);
                return result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error extending listing for Id: {Id}", id);
                throw new Exception($"Error extending listing: {ex.Message}", ex);
            }
        }

        public async Task<bool> AddServiceRecordAsync(string id, ServiceRecord serviceRecord)
        {
            try
            {
                if (serviceRecord == null)
                    throw new ArgumentNullException(nameof(serviceRecord));

                var update = Builders<Vehicle>.Update.Push(v => v.ServiceRecords, serviceRecord);
                var filter = Builders<Vehicle>.Filter.Eq(v => v.Id, id);
                var result = await _vehiclesCollection.UpdateOneAsync(filter, update);
                return result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error adding service record for Id: {Id}", id);
                throw new Exception($"Error adding service record: {ex.Message}", ex);
            }
        }

        public async Task<Dictionary<string, long>> GetVehicleCountByMakeAsync()
        {
            try
            {
                var filter = Builders<Vehicle>.Filter.And(
                    Builders<Vehicle>.Filter.Eq(v => v.IsActive, true),
                    Builders<Vehicle>.Filter.Eq(v => v.Status.CurrentStatus, "active")
                );

                var result = await _vehiclesCollection
                    .Aggregate()
                    .Match(filter)
                    .Group(v => v.Specifications.Make,
                           g => new { Make = g.Key, Count = g.Count() })
                    .ToListAsync();

                return result.ToDictionary(x => x.Make ?? "Unknown", x => x.Count);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error getting vehicle count by make");
                throw new Exception($"Error getting vehicle count by make: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesByPriceRangeAsync(decimal minPrice, decimal maxPrice, int limit = 50)
        {
            try
            {
                var filter = Builders<Vehicle>.Filter.And(
                    Builders<Vehicle>.Filter.Eq(v => v.IsActive, true),
                    Builders<Vehicle>.Filter.Eq(v => v.Status.CurrentStatus, "active"),
                    Builders<Vehicle>.Filter.Gte(v => v.Price.Raw, minPrice),
                    Builders<Vehicle>.Filter.Lte(v => v.Price.Raw, maxPrice)
                );

                var vehicles = await _vehiclesCollection
                    .Find(filter)
                    .SortBy(v => v.Price.Raw)
                    .Limit(limit)
                    .ToListAsync();

                return vehicles;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error getting vehicles by price range: {MinPrice} - {MaxPrice}", minPrice, maxPrice);
                throw new Exception($"Error getting vehicles by price range: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Vehicle>> GetExpiringListingsAsync(int daysThreshold = 7)
        {
            try
            {
                var expiryDate = DateTime.UtcNow.AddDays(daysThreshold);
                var filter = Builders<Vehicle>.Filter.And(
                    Builders<Vehicle>.Filter.Eq(v => v.IsActive, true),
                    Builders<Vehicle>.Filter.Eq(v => v.Status.CurrentStatus, "active"),
                    Builders<Vehicle>.Filter.Lte(v => v.Dates.ValidTo, expiryDate),
                    Builders<Vehicle>.Filter.Gte(v => v.Dates.ValidTo, DateTime.UtcNow)
                );

                var vehicles = await _vehiclesCollection
                    .Find(filter)
                    .SortBy(v => v.Dates.ValidTo)
                    .ToListAsync();

                return vehicles;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error getting expiring listings");
                throw new Exception($"Error getting expiring listings: {ex.Message}", ex);
            }
        }
    }
}