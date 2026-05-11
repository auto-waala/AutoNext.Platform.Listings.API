using AutoNext.Platform.Listings.API.Models.DTOs;
using AutoNext.Platform.Listings.API.Models.Entities;

namespace AutoNext.Platform.Listings.API.Models.Mappers
{
    public static class NewlyArrivedMapper
    {
        public static NewlyArrivedResponseDto ToResponseDto(this NewlyArrivedVehicle entity)
        {
            if (entity == null) return null!;

            return new NewlyArrivedResponseDto
            {
                Id = entity.Id,
                BrandName = entity.BrandName,
                ModelName = entity.ModelName,
                ModelSlug = entity.ModelSlug,
                VehicleType = entity.VehicleType,
                BodyType = entity.BodyType,
                PriceRange = entity.PriceRange,
                MinPrice = entity.MinPrice,
                MaxPrice = entity.MaxPrice,
                ArrivalDate = entity.ArrivalDate,
                ArrivalPeriod = entity.ArrivalPeriod,
                Featured = entity.Featured,
                Emi = entity.Emi,
                Images = entity.Images,
                ThumbnailImage = entity.ThumbnailImage,
                Videos = entity.Videos,
                Variants = entity.Variants,
                Rating = entity.Rating,
                ReviewCount = entity.ReviewCount,
                PageTitle = entity.PageTitle,
                DescriptionText = entity.DescriptionText
            };
        }

        public static List<NewlyArrivedResponseDto> ToResponseDtoList(this IEnumerable<NewlyArrivedVehicle> entities)
        {
            return entities?.Select(e => e.ToResponseDto()).ToList() ?? new List<NewlyArrivedResponseDto>();
        }

        // ── Request DTO to Entity ─────────────────────────────────────────────

        public static NewlyArrivedVehicle ToEntity(this NewlyArrivedRequestDto request)
        {
            if (request == null) return null!;

            return new NewlyArrivedVehicle
            {
                BrandName = request.BrandName,
                ModelName = request.ModelName,
                ModelSlug = GenerateSlug(request.BrandName, request.ModelName),
                VehicleType = request.VehicleType,
                BodyType = request.BodyType,
                MinPrice = request.MinPrice,
                MaxPrice = request.MaxPrice,
                PriceRange = $"{FormatPrice(request.MinPrice)} - {FormatPrice(request.MaxPrice)}",
                ArrivalPeriod = request.ArrivalPeriod,
                ArrivalDate = DateTime.UtcNow,
                Featured = false,
                Emi = request.Emi,
                Images = request.Images,
                Videos = request.Videos,
                Variants = request.Variants,
                Rating = request.Rating,
                ReviewCount = request.ReviewCount,
                PageTitle = request.PageTitle,
                DescriptionText = request.DescriptionText,
                Status = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        // ── Update Entity from Request ────────────────────────────────────────

        public static void UpdateFromRequest(this NewlyArrivedVehicle entity, NewlyArrivedRequestDto request)
        {
            if (entity == null || request == null) return;

            entity.BrandName = request.BrandName;
            entity.ModelName = request.ModelName;
            entity.ModelSlug = GenerateSlug(request.BrandName, request.ModelName);
            entity.VehicleType = request.VehicleType;
            entity.BodyType = request.BodyType;
            entity.MinPrice = request.MinPrice;
            entity.MaxPrice = request.MaxPrice;
            entity.PriceRange = $"{FormatPrice(request.MinPrice)} - {FormatPrice(request.MaxPrice)}";
            entity.ArrivalPeriod = request.ArrivalPeriod;
            entity.Emi = request.Emi;
            entity.Images = request.Images;
            entity.Videos = request.Videos;
            entity.Variants = request.Variants;
            entity.Rating = request.Rating;
            entity.ReviewCount = request.ReviewCount;
            entity.PageTitle = request.PageTitle;
            entity.DescriptionText = request.DescriptionText;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        // ── Private Helpers ───────────────────────────────────────────────────

        private static string GenerateSlug(string brandName, string modelName)
        {
            var slug = $"{brandName}-{modelName}".ToLower();
            slug = slug.Replace(" ", "-");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9-]", "");
            return slug;
        }

        private static string FormatPrice(decimal price)
        {
            if (price >= 10000000) // 1 Crore+
                return $"₹{price / 10000000:0.##} Crore";
            if (price >= 100000) // 1 Lakh+
                return $"₹{price / 100000:0.##} Lakh";
            return $"₹{price:N0}";
        }
    }
}
