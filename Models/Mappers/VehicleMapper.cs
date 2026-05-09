using AutoNext.Platform.Listings.API.Models.DTOs;
using AutoNext.Platform.Listings.API.Models.Entities;

namespace AutoNext.Platform.Listings.API.Models.Mappers
{
    public static class VehicleMapper
    {
        public static VehicleDto ToDto(this Vehicle vehicle)
        {
            return new VehicleDto
            {
                // ── Identity ────────────────────────────────────────────────
                Id = vehicle.Id,
                VIN = vehicle.VIN,
                ChassisNumber = vehicle.ChassisNumber,
                EngineNumber = vehicle.EngineNumber,

                // ── Listing info ────────────────────────────────────────────
                Title = vehicle.Title,
                Description = vehicle.Description,
                VehicleType = vehicle.VehicleType,
                BodyType = vehicle.BodyType,
                Color = vehicle.Color,
                City = vehicle.City,
                Locality = vehicle.Locality,
                Images = vehicle.Images,
                Benefits = vehicle.Benefits,
                Status = vehicle.Status,
                IsActive = vehicle.IsActive,
                Views = vehicle.Views,

                // ── Flat fields (backward compat) ───────────────────────────
                Make = vehicle.Make,
                Model = vehicle.Model,
                Year = vehicle.Year,
                Kilometers = vehicle.Kilometers,
                FuelType = vehicle.FuelType,
                Transmission = vehicle.Transmission,

                // ── Structured price ────────────────────────────────────────
                Price = new VehiclePriceDto
                {
                    Raw = vehicle.Price.Raw,
                    Display = vehicle.Price.Display,
                    Currency = vehicle.Price.Currency
                },

                // ── Specifications ──────────────────────────────────────────
                Specifications = vehicle.Specifications is null ? null : new VehicleSpecificationsDto
                {
                    Make = vehicle.Specifications.Make,
                    Model = vehicle.Specifications.Model,
                    Variant = vehicle.Specifications.Variant,
                    Year = vehicle.Specifications.Year,
                    EngineCC = vehicle.Specifications.EngineCC,
                    MileageKmpl = vehicle.Specifications.MileageKmpl,
                    SeatingCapacity = vehicle.Specifications.SeatingCapacity,
                    OwnershipCount = vehicle.Specifications.OwnershipCount
                },

                // ── Seller ──────────────────────────────────────────────────
                Seller = vehicle.Seller is null ? null : new SellerInfoDto
                {
                    UserId = vehicle.Seller.UserId,
                    Name = vehicle.Seller.Name,
                    Phone = vehicle.Seller.Phone,
                    Email = vehicle.Seller.Email,
                    SellerType = vehicle.Seller.SellerType,
                    DealerId = vehicle.Seller.DealerId,
                    StoreId = vehicle.Seller.StoreId,
                    Location = vehicle.Seller.Location,
                    ChatEnabled = vehicle.Seller.ChatEnabled,
                    CallEnabled = vehicle.Seller.CallEnabled,
                    IsVerified = vehicle.Seller.IsVerified
                },

                // ── Legacy flat seller fields (backward compat) ─────────────
                SellerId = vehicle.SellerId,
                SellerName = vehicle.SellerName,
                SellerPhone = vehicle.SellerPhone,

                // ── Inspection ──────────────────────────────────────────────
                Inspection = vehicle.Inspection is null ? null : new InspectionReportDto
                {
                    ReportUrl = vehicle.Inspection.ReportUrl,
                    InspectionPoints = vehicle.Inspection.InspectionPoints,
                    InspectedOn = vehicle.Inspection.InspectedOn,
                    IsAvailable = vehicle.Inspection.IsAvailable,
                    Title = vehicle.Inspection.Title,
                    Description = vehicle.Inspection.Description
                },

                // ── Audit ───────────────────────────────────────────────────
                CreatedBy = vehicle.CreatedBy,
                CreatedOn = vehicle.CreatedOn,
                ModifiedBy = vehicle.ModifiedBy,
                ModifiedOn = vehicle.ModifiedOn
            };
        }

        public static List<VehicleDto> ToDtoList(this IEnumerable<Vehicle> vehicles)
            => vehicles.Select(v => v.ToDto()).ToList();
    }
}
