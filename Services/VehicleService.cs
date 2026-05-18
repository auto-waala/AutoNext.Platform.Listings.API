using AutoNext.Platform.Listings.API.Models.Common;
using AutoNext.Platform.Listings.API.Models.DTOs;
using AutoNext.Platform.Listings.API.Models.Entities;
using AutoNext.Platform.Listings.API.Models.Mappers;
using AutoNext.Platform.Listings.API.Repositories;

namespace AutoNext.Platform.Listings.API.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VehicleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ── Get by ID ─────────────────────────────────────────────────────────

        public async Task<VehicleDto?> GetByIdAsync(string id)
        {
            var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(id);
            return vehicle?.ToDto();
        }

        // ── Get all (paginated) ───────────────────────────────────────────────

        public async Task<PagedResult<VehicleDto>> GetAllAsync(int page, int pageSize)
        {
            var vehicles = await _unitOfWork.Vehicles.GetAllAsync(page, pageSize);
            var totalCount = await _unitOfWork.Vehicles.GetTotalCountAsync();

            return new PagedResult<VehicleDto>
            {
                Items = vehicles.Select(v => v.ToDto()).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        // ── Search ────────────────────────────────────────────────────────────

        public async Task<PagedResult<VehicleDto>> SearchAsync(VehicleSearchRequest request)
        {
            var vehicles = await _unitOfWork.Vehicles.SearchAsync(request);
            var total = await _unitOfWork.Vehicles.GetSearchCountAsync(request);

            return new PagedResult<VehicleDto>
            {
                Items = vehicles.Select(v => v.ToDto()).ToList(),
                TotalCount = total,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }

        // ── Create ────────────────────────────────────────────────────────────

        public async Task<VehicleDto> CreateAsync(CreateVehicleRequest request, string sellerId)
        {
            var vehicle = new Vehicle
            {
                // ── Listing info ──────────────────────────────────────────────
                Title = request.Title,
                Description = request.Description,
                VehicleType = request.VehicleType,
                BodyType = request.BodyType,
                Color = request.Color,
                City = request.City,
                Locality = request.Locality,
                Images = request.Images,
                Benefits = request.Benefits,

                // ── Flat vehicle fields ───────────────────────────────────────
                Make = request.Make,
                Model = request.Model,
                Year = request.Year,
                Kilometers = request.Kilometers,
                FuelType = request.FuelType,
                Transmission = request.Transmission,

                // ── Price (implicit operator converts decimal → VehiclePrice) ─
                Price = request.Price,

                // ── Identity numbers ──────────────────────────────────────────
                VIN = request.VIN,
                ChassisNumber = request.ChassisNumber,
                EngineNumber = request.EngineNumber,

                // ── Specifications ────────────────────────────────────────────
                Specifications = request.Specifications is null ? new VehicleSpecifications
                {
                    Make = request.Make,
                    Model = request.Model,
                    Year = request.Year
                }
                : new VehicleSpecifications
                {
                    Make = request.Make,
                    Model = request.Model,
                    Year = request.Year,
                    Variant = request.Specifications.Variant,
                    EngineCC = request.Specifications.EngineCC,
                    MileageKmpl = request.Specifications.MileageKmpl,
                    SeatingCapacity = request.Specifications.SeatingCapacity,
                    OwnershipCount = request.Specifications.OwnershipCount
                },

                // ── Seller (structured + legacy flat fields) ──────────────────
                Seller = new SellerInfo
                {
                    SellerId = sellerId,
                    SellerName = request.Seller.Name,
                    ContactNumber = request.Seller.Phone,
                    Email = request.Seller.Email,
                    SellerType = request.Seller.SellerType,
                    DealerName = request.Seller.DealerId,
                    DealerAddress = request.Seller.Location,
                },

                // Legacy flat seller fields
                SellerId = sellerId,
                SellerName = request.Seller.Name,
                SellerPhone = request.Seller.Phone,

                // ── Audit ─────────────────────────────────────────────────────
                CreatedBy = sellerId,
                CreatedOn = DateTime.UtcNow,
                ModifiedBy = sellerId,
                ModifiedOn = DateTime.UtcNow,
                IsActive = true,
                Status = "active"
            };

            var created = await _unitOfWork.Vehicles.CreateAsync(vehicle);
            await _unitOfWork.SaveChangesAsync();

            return created.ToDto();
        }

        // ── Update ────────────────────────────────────────────────────────────

        public async Task<VehicleDto?> UpdateAsync(string id, UpdateVehicleRequest request)
        {
            var existing = await _unitOfWork.Vehicles.GetByIdAsync(id);
            if (existing is null) return null;

            // Only update fields that were provided (non-null)
            if (request.Title is not null) existing.Title = request.Title;
            if (request.Description is not null) existing.Description = request.Description;
            if (request.Color is not null) existing.Color = request.Color;
            if (request.City is not null) existing.City = request.City;
            if (request.Locality is not null) existing.Locality = request.Locality;
            if (request.Images is not null) existing.Images = request.Images;
            if (request.Benefits is not null) existing.Benefits = request.Benefits;
            if (request.Status is not null) existing.Status = request.Status;
            if (request.Kilometers is not null) existing.Kilometers = request.Kilometers.Value;

            // Price uses implicit operator
            if (request.Price is not null)
                existing.Price = request.Price.Value;

            existing.ModifiedBy = "Admin";
            existing.ModifiedOn = DateTime.UtcNow;

            var updated = await _unitOfWork.Vehicles.UpdateAsync(id, existing);
            await _unitOfWork.SaveChangesAsync();

            return updated?.ToDto();
        }

        // ── Soft delete (sets IsActive = false) ───────────────────────────────

        public async Task<bool> DeleteAsync(string id, string deletedBy = null)
        {
            var existing = await _unitOfWork.Vehicles.GetByIdAsync(id);
            if (existing is null) return false;

            existing.IsActive = false;
            existing.Status = "deleted";
            existing.ModifiedBy = deletedBy;
            existing.ModifiedOn = DateTime.UtcNow;

            await _unitOfWork.Vehicles.UpdateAsync(id, existing);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        // ── Hard delete (permanent) ───────────────────────────────────────────

        public async Task<bool> HardDeleteAsync(string id)
        {
            return await _unitOfWork.Vehicles.DeleteAsync(id);
        }

        // ── Increment view count ──────────────────────────────────────────────

        public async Task IncrementViewsAsync(string id)
        {
            await _unitOfWork.Vehicles.IncrementViewsAsync(id);
        }

        // ── Get by seller ─────────────────────────────────────────────────────

        public async Task<PagedResult<VehicleDto>> GetBySellerAsync(string sellerId, int page, int pageSize)
        {
            var vehicles = await _unitOfWork.Vehicles.GetBySellerAsync(sellerId, page, pageSize);
            var total = await _unitOfWork.Vehicles.GetSellerCountAsync(sellerId);

            return new PagedResult<VehicleDto>
            {
                Items = vehicles.Select(v => v.ToDto()).ToList(),
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
