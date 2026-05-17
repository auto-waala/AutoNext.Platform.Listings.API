using AutoNext.Platform.Listings.API.Models.DTOs;
using AutoNext.Platform.Listings.API.Models.Entities;
using System.Text.RegularExpressions;

namespace AutoNext.Platform.Listings.API.Models.Mappers
{
    public static class UsedVehiclesMapper
    {
        // ─────────────────────────────────────────────────────────────
        // Entity → Response DTO
        // ─────────────────────────────────────────────────────────────

        public static UsedVehiclesResponseDto ToResponseDto(this UsedVehicles entity)
        {
            if (entity == null)
                return null!;

            return new UsedVehiclesResponseDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Descriptions = entity.Descriptions,
                Slug = entity.Slug,
                MetaTitle = entity.MetaTitle,
                MetaDescription = entity.MetaDescription,

                BrandName = entity.BrandName,
                ModelName = entity.ModelName,
                ModelSlug = entity.ModelSlug,

                VehicleType = entity.VehicleType,
                BodyType = entity.BodyType,

                Price = entity.Price?.ToDto(),
                PriceRangeFrom = entity.PriceRangeFrom,
                PriceRangeTo = entity.PriceRangeTo,

                Images = entity.Images?
                    .Select(x => x.ToDto())
                    .ToList() ?? new List<VehicleImageDto>(),

                Videos = entity.Videos?
                    .Select(x => x.ToDto())
                    .ToList() ?? new List<VehicleMediaDto>(),

                Shorts = entity.Shorts?
                    .Select(x => x.ToDto())
                    .ToList() ?? new List<VehicleMediaDto>(),

                Thumbnail = entity.Thumbnail,
                ThumbnailWebp = entity.ThumbnailWebp,

                Variants = entity.Variants?
                    .Select(x => x.ToDto())
                    .ToList() ?? new List<VehicleVariantDto>(),

                KeySpecifications = entity.KeySpecifications?.ToDto(),

                TopFeatures = entity.TopFeatures?
                    .Select(x => x.ToDto())
                    .ToList() ?? new List<FeatureItemDto>(),

                StandOutFeatures = entity.StandOutFeatures?
                    .Select(x => x.ToDto())
                    .ToList() ?? new List<FeatureItemDto>(),

                Pros = entity.Pros?
                    .Select(x => x.ToDto())
                    .ToList() ?? new List<ProConItemDto>(),

                Cons = entity.Cons?
                    .Select(x => x.ToDto())
                    .ToList() ?? new List<ProConItemDto>(),

                Tags = entity.Tags?
                    .Select(x => x.ToDto())
                    .ToList() ?? new List<TagItemDto>(),

                UserRatings = entity.UserRatings?
                    .Select(x => x.ToDto())
                    .ToList() ?? new List<UserRatingDto>(),

                Rating = entity.Rating,
                Engagement = entity.Engagement?.ToDto(),

                Seller = entity.Seller?.ToDto(),

                Location = entity.Location?.ToDto(),

                Condition = entity.Condition?.ToDto(),

                ListingDetails = entity.ListingDetails?.ToDto(),
                ShareUrls = entity.ShareUrls?.ToDto(),
                TestDrive = entity.TestDrive?.ToDto(),

                Priority = entity.Priority,
                IsActive = entity.IsActive,

                StartDate = entity.StartDate,
                EndDate = entity.EndDate,

                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }


        public static List<UsedVehiclesResponseDto> ToResponseDtoList(this IEnumerable<UsedVehicles> entities)
        {
            return entities?
                .Select(x => x.ToResponseDto())
                .ToList() ?? new List<UsedVehiclesResponseDto>();
        }

        // ─────────────────────────────────────────────────────────────
        // Request DTO → Entity
        // ─────────────────────────────────────────────────────────────

        public static UsedVehicles ToEntity(this UsedVehiclesRequestDto request)
        {
            if (request == null)
                return null!;

            var entity = new UsedVehicles
            {
                Title = request.Title,
                Descriptions = request.Descriptions,

                Slug = string.IsNullOrWhiteSpace(request.Slug)
                    ? GenerateSlug(request.BrandName, request.ModelName)
                    : GenerateSlug(request.Slug),

                MetaTitle = request.MetaTitle,
                MetaDescription = request.MetaDescription,

                BrandName = request.BrandName,
                ModelName = request.ModelName,
                ModelSlug = GenerateSlug(request.BrandName, request.ModelName),

                VehicleType = request.VehicleType,
                BodyType = request.BodyType,

                Price = request.Price?.ToEntity(),
                PriceRangeFrom = request.PriceRangeFrom,
                PriceRangeTo = request.PriceRangeTo,

                Images = request.Images?
                    .Select(x => x.ToEntity())
                    .ToList() ?? new List<VehicleImage>(),

                Videos = request.Videos?
                    .Select(x => x.ToEntity())
                    .ToList() ?? new List<VehicleMedia>(),

                Shorts = request.Shorts?
                    .Select(x => x.ToEntity())
                    .ToList() ?? new List<VehicleMedia>(),

                Variants = request.Variants?
                    .Select(x => x.ToEntity())
                    .ToList() ?? new List<VehicleVariant>(),

                KeySpecifications = request.KeySpecifications?.ToEntity(),

                TopFeatures = request.TopFeatures?
                    .Select(x => x.ToEntity())
                    .ToList() ?? new List<FeatureItem>(),

                StandOutFeatures = request.StandOutFeatures?
                    .Select(x => x.ToEntity())
                    .ToList() ?? new List<FeatureItem>(),

                Pros = request.Pros?
                    .Select(x => x.ToEntity())
                    .ToList() ?? new List<ProConItem>(),

                Cons = request.Cons?
                    .Select(x => x.ToEntity())
                    .ToList() ?? new List<ProConItem>(),

                Tags = request.Tags?
                    .Select(x => x.ToEntity())
                    .ToList() ?? new List<TagItem>(),

                UserRatings = request.UserRatings?
                    .Select(x => x.ToEntity())
                    .ToList() ?? new List<UserRating>(),

                Rating = request.Rating,

                Seller = request.Seller?.ToEntity(),

                Location = request.Location?.ToEntity(),

                Condition = request.Condition?.ToEntity(),

                ListingDetails = request.ListingDetails?.ToEntity(),
                TestDrive = request.TestDrive?.ToEntity(),

                Priority = request.Priority,
                IsActive = request.IsActive,

                StartDate = request.StartDate,
                EndDate = request.EndDate,

                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Update thumbnail from primary image
            entity.UpdateThumbnailFromPrimaryImage();

            return entity;
        }

        // ─────────────────────────────────────────────────────────────
        // Update Existing Entity
        // ─────────────────────────────────────────────────────────────

        public static void UpdateFromRequest(this UsedVehicles entity, UsedVehiclesRequestDto request)
        {
            if (entity == null || request == null)
                return;

            entity.Title = request.Title;
            entity.Descriptions = request.Descriptions;

            entity.Slug = string.IsNullOrWhiteSpace(request.Slug)
                ? GenerateSlug(request.BrandName, request.ModelName)
                : GenerateSlug(request.Slug);

            entity.MetaTitle = request.MetaTitle;
            entity.MetaDescription = request.MetaDescription;

            entity.BrandName = request.BrandName;
            entity.ModelName = request.ModelName;
            entity.ModelSlug = GenerateSlug(request.BrandName, request.ModelName);

            entity.VehicleType = request.VehicleType;
            entity.BodyType = request.BodyType;

            entity.Price = request.Price?.ToEntity();
            entity.PriceRangeFrom = request.PriceRangeFrom;
            entity.PriceRangeTo = request.PriceRangeTo;

            entity.Images = request.Images?
                .Select(x => x.ToEntity())
                .ToList() ?? new List<VehicleImage>();

            entity.Videos = request.Videos?
                .Select(x => x.ToEntity())
                .ToList() ?? new List<VehicleMedia>();

            entity.Shorts = request.Shorts?
                .Select(x => x.ToEntity())
                .ToList() ?? new List<VehicleMedia>();

            entity.Variants = request.Variants?
                .Select(x => x.ToEntity())
                .ToList() ?? new List<VehicleVariant>();

            entity.KeySpecifications = request.KeySpecifications?.ToEntity();

            entity.TopFeatures = request.TopFeatures?
                .Select(x => x.ToEntity())
                .ToList() ?? new List<FeatureItem>();

            entity.StandOutFeatures = request.StandOutFeatures?
                .Select(x => x.ToEntity())
                .ToList() ?? new List<FeatureItem>();

            entity.Pros = request.Pros?
                .Select(x => x.ToEntity())
                .ToList() ?? new List<ProConItem>();

            entity.Cons = request.Cons?
                .Select(x => x.ToEntity())
                .ToList() ?? new List<ProConItem>();

            entity.Tags = request.Tags?
                .Select(x => x.ToEntity())
                .ToList() ?? new List<TagItem>();

            entity.UserRatings = request.UserRatings?
                .Select(x => x.ToEntity())
                .ToList() ?? new List<UserRating>();

            entity.Rating = request.Rating;

            entity.Seller = request.Seller?.ToEntity();

            entity.Location = request.Location?.ToEntity();

            entity.Condition = request.Condition?.ToEntity();

            entity.ListingDetails = request.ListingDetails?.ToEntity();
            entity.TestDrive = request.TestDrive?.ToEntity();

            entity.Priority = request.Priority;
            entity.IsActive = request.IsActive;

            entity.StartDate = request.StartDate;
            entity.EndDate = request.EndDate;

            entity.UpdatedAt = DateTime.UtcNow;

            // Update thumbnail from primary image
            entity.UpdateThumbnailFromPrimaryImage();
        }

        // ─────────────────────────────────────────────────────────────
        // PriceInfo Mapping
        // ─────────────────────────────────────────────────────────────

        public static PriceInfoDto ToDto(this PriceInfo entity)
        {
            if (entity == null)
                return null!;

            return new PriceInfoDto
            {
                Amount = entity.Amount,
                Currency = entity.Currency,
                Negotiable = entity.Negotiable,
                OnRoadPrice = entity.OnRoadPrice
            };
        }

        public static PriceInfo ToEntity(this PriceInfoDto dto)
        {
            if (dto == null)
                return null!;

            return new PriceInfo
            {
                Amount = dto.Amount,
                Currency = dto.Currency,
                Negotiable = dto.Negotiable,
                OnRoadPrice = dto.OnRoadPrice
            };
        }

        // ─────────────────────────────────────────────────────────────
        // VehicleImage Mapping
        // ─────────────────────────────────────────────────────────────

        public static VehicleImageDto ToDto(this VehicleImage entity)
        {
            if (entity == null)
                return null!;

            return new VehicleImageDto
            {
                FileId = entity.FileId,
                FileUrl = entity.FileUrl,
                IsPrimary = entity.IsPrimary
            };
        }

        public static VehicleImage ToEntity(this VehicleImageDto dto)
        {
            if (dto == null)
                return null!;

            return new VehicleImage
            {
                FileId = dto.FileId,
                FileUrl = dto.FileUrl,
                IsPrimary = dto.IsPrimary
            };
        }

        // ─────────────────────────────────────────────────────────────
        // VehicleMedia Mapping
        // ─────────────────────────────────────────────────────────────

        public static VehicleMediaDto ToDto(this VehicleMedia entity)
        {
            if (entity == null)
                return null!;

            return new VehicleMediaDto
            {
                FileId = entity.FileId,
                FileUrl = entity.FileUrl
            };
        }

        public static VehicleMedia ToEntity(this VehicleMediaDto dto)
        {
            if (dto == null)
                return null!;

            return new VehicleMedia
            {
                FileId = dto.FileId,
                FileUrl = dto.FileUrl
            };
        }

        // ─────────────────────────────────────────────────────────────
        // VehicleVariant Mapping
        // ─────────────────────────────────────────────────────────────

        public static VehicleVariantDto ToDto(this VehicleVariant entity)
        {
            if (entity == null)
                return null!;

            return new VehicleVariantDto
            {
                Color = entity.Color,
                Engine = entity.Engine,
                Transmission = entity.Transmission,
                FuelType = entity.FuelType,
                Mileage = entity.Mileage,
                YearOfManufacture = entity.YearOfManufacture
            };
        }

        public static VehicleVariant ToEntity(this VehicleVariantDto dto)
        {
            if (dto == null)
                return null!;

            return new VehicleVariant
            {
                Color = dto.Color,
                Engine = dto.Engine,
                Transmission = dto.Transmission,
                FuelType = dto.FuelType,
                Mileage = dto.Mileage,
                YearOfManufacture = dto.YearOfManufacture
            };
        }

        // ─────────────────────────────────────────────────────────────
        // KeySpecifications Mapping
        // ─────────────────────────────────────────────────────────────

        public static KeySpecificationsDto ToDto(this KeySpecifications entity)
        {
            if (entity == null)
                return null!;

            return new KeySpecificationsDto
            {
                Engine = entity.Engine,
                Transmission = entity.Transmission,
                FuelType = entity.FuelType,
                Mileage = entity.Mileage,
                YearOfManufacture = entity.YearOfManufacture
            };
        }

        public static KeySpecifications ToEntity(this KeySpecificationsDto dto)
        {
            if (dto == null)
                return null!;

            return new KeySpecifications
            {
                Engine = dto.Engine,
                Transmission = dto.Transmission,
                FuelType = dto.FuelType,
                Mileage = dto.Mileage,
                YearOfManufacture = dto.YearOfManufacture
            };
        }

        // ─────────────────────────────────────────────────────────────
        // FeatureItem Mapping
        // ─────────────────────────────────────────────────────────────

        public static FeatureItemDto ToDto(this FeatureItem entity)
        {
            if (entity == null)
                return null!;

            return new FeatureItemDto
            {
                Feature = entity.Feature
            };
        }

        public static FeatureItem ToEntity(this FeatureItemDto dto)
        {
            if (dto == null)
                return null!;

            return new FeatureItem
            {
                Feature = dto.Feature
            };
        }

        // ─────────────────────────────────────────────────────────────
        // ProConItem Mapping
        // ─────────────────────────────────────────────────────────────

        public static ProConItemDto ToDto(this ProConItem entity)
        {
            if (entity == null)
                return null!;

            return new ProConItemDto
            {
                Pro = entity.Pro,
                Con = entity.Con
            };
        }

        public static ProConItem ToEntity(this ProConItemDto dto)
        {
            if (dto == null)
                return null!;

            return new ProConItem
            {
                Pro = dto.Pro,
                Con = dto.Con
            };
        }

        // ─────────────────────────────────────────────────────────────
        // TagItem Mapping
        // ─────────────────────────────────────────────────────────────

        public static TagItemDto ToDto(this TagItem entity)
        {
            if (entity == null)
                return null!;

            return new TagItemDto
            {
                TagName = entity.TagName
            };
        }

        public static TagItem ToEntity(this TagItemDto dto)
        {
            if (dto == null)
                return null!;

            return new TagItem
            {
                TagName = dto.TagName
            };
        }

        // ─────────────────────────────────────────────────────────────
        // UserRating Mapping
        // ─────────────────────────────────────────────────────────────

        public static UserRatingDto ToDto(this UserRating entity)
        {
            if (entity == null)
                return null!;

            return new UserRatingDto
            {
                UserId = entity.UserId,
                Rating = entity.Rating,
                Comment = entity.Comment,
                CreatedAt = entity.CreatedAt
            };
        }

        public static UserRating ToEntity(this UserRatingDto dto)
        {
            if (dto == null)
                return null!;

            return new UserRating
            {
                UserId = dto.UserId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = dto.CreatedAt == default ? DateTime.UtcNow : dto.CreatedAt
            };
        }

        // ─────────────────────────────────────────────────────────────
        // SellerInfo Mapping
        // ─────────────────────────────────────────────────────────────

        public static SellerInfoDto ToDto(this SellerInfo entity)
        {
            if (entity == null)
                return null!;

            return new SellerInfoDto
            {
                UserId = entity.SellerId,
                Name = entity.SellerName,
                SellerType = entity.SellerType,
                Phone = entity.ContactNumber,
                Email = entity.Email,
                IsVerified = entity.IsVerifiedSeller,
                DealerId = entity.DealerName,
                Location = entity.DealerAddress
            };
        }

        public static SellerInfo ToEntity(this SellerInfoDto dto)
        {
            if (dto == null)
                return null!;

            return new SellerInfo
            {
                SellerId = dto.UserId,
                SellerName = dto.Name,
                SellerType = dto.SellerType,
                ContactNumber = dto.Phone,
                Email = dto.Email,
                IsVerifiedSeller = dto.IsVerified,
                DealerName = dto.DealerId,
                DealerAddress = dto.Location
            };
        }

        // ─────────────────────────────────────────────────────────────
        // LocationInfo Mapping
        // ─────────────────────────────────────────────────────────────

        public static LocationInfoDto ToDto(this LocationInfo entity)
        {
            if (entity == null)
                return null!;

            return new LocationInfoDto
            {
                City = entity.City,
                State = entity.State,
                Pincode = entity.Pincode,
                Latitude = entity.Latitude,
                Longitude = entity.Longitude
            };
        }

        public static LocationInfo ToEntity(this LocationInfoDto dto)
        {
            if (dto == null)
                return null!;

            return new LocationInfo
            {
                City = dto.City,
                State = dto.State,
                Pincode = dto.Pincode,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude
            };
        }

        // ─────────────────────────────────────────────────────────────
        // VehicleCondition Mapping
        // ─────────────────────────────────────────────────────────────

        public static VehicleConditionDto ToDto(this VehicleCondition entity)
        {
            if (entity == null)
                return null!;

            return new VehicleConditionDto
            {
                IsNew = entity.IsNew,
                OwnerCount = entity.OwnerCount,
                KMDriven = entity.KMDriven,
                Accidental = entity.Accidental,
                ServiceHistoryAvailable = entity.ServiceHistoryAvailable,
                RegistrationYear = entity.RegistrationYear,
                RegistrationMonth = entity.RegistrationMonth
            };
        }

        public static VehicleCondition ToEntity(this VehicleConditionDto dto)
        {
            if (dto == null)
                return null!;

            return new VehicleCondition
            {
                IsNew = dto.IsNew,
                OwnerCount = dto.OwnerCount,
                KMDriven = dto.KMDriven,
                Accidental = dto.Accidental,
                ServiceHistoryAvailable = dto.ServiceHistoryAvailable,
                RegistrationYear = dto.RegistrationYear,
                RegistrationMonth = dto.RegistrationMonth
            };
        }

        // ─────────────────────────────────────────────────────────────
        // EngagementMetrics Mapping
        // ─────────────────────────────────────────────────────────────

        public static EngagementMetricsDto ToDto(this EngagementMetrics entity)
        {
            if (entity == null)
                return null!;

            return new EngagementMetricsDto
            {
                Views = entity.Views,
                Likes = entity.Likes,
                Shares = entity.Shares,
                Enquiries = entity.Enquiries
            };
        }

        public static EngagementMetrics ToEntity(this EngagementMetricsDto dto)
        {
            if (dto == null)
                return null!;

            return new EngagementMetrics
            {
                Views = dto.Views,
                Likes = dto.Likes,
                Shares = dto.Shares,
                Enquiries = dto.Enquiries
            };
        }

        // ─────────────────────────────────────────────────────────────
        // ListingDetails Mapping
        // ─────────────────────────────────────────────────────────────

        public static ListingDetailsDto ToDto(this ListingDetails entity)
        {
            if (entity == null)
                return null!;

            return new ListingDetailsDto
            {
                IsAvailable = entity.IsAvailable,
                IsFeatured = entity.IsFeatured,
                IsSold = entity.IsSold,
                PostedDate = entity.PostedDate,
                ExpiryDate = entity.ExpiryDate,
                IsVerified = entity.IsVerified,
                VerifiedBy = entity.VerifiedBy,
                VerificationDate = entity.VerificationDate
            };
        }

        public static ListingDetails ToEntity(this ListingDetailsDto dto)
        {
            if (dto == null)
                return null!;

            return new ListingDetails
            {
                IsAvailable = dto.IsAvailable,
                IsFeatured = dto.IsFeatured,
                IsSold = dto.IsSold,
                PostedDate = dto.PostedDate == default ? DateTime.UtcNow : dto.PostedDate,
                ExpiryDate = dto.ExpiryDate,
                IsVerified = dto.IsVerified,
                VerifiedBy = dto.VerifiedBy,
                VerificationDate = dto.VerificationDate
            };
        }

        // ─────────────────────────────────────────────────────────────
        // ShareUrls Mapping
        // ─────────────────────────────────────────────────────────────

        public static ShareUrlsDto ToDto(this ShareUrls entity)
        {
            if (entity == null)
                return null!;

            return new ShareUrlsDto
            {
                Facebook = entity.Facebook,
                Twitter = entity.Twitter,
                WhatsApp = entity.WhatsApp,
                LinkedIn = entity.LinkedIn
            };
        }

        public static ShareUrls ToEntity(this ShareUrlsDto dto)
        {
            if (dto == null)
                return null!;

            return new ShareUrls
            {
                Facebook = dto.Facebook,
                Twitter = dto.Twitter,
                WhatsApp = dto.WhatsApp,
                LinkedIn = dto.LinkedIn
            };
        }

        // ─────────────────────────────────────────────────────────────
        // TestDriveInfo Mapping
        // ─────────────────────────────────────────────────────────────

        public static TestDriveInfoDto ToDto(this TestDriveInfo entity)
        {
            if (entity == null)
                return null!;

            return new TestDriveInfoDto
            {
                Available = entity.Available,
                BookingAmount = entity.BookingAmount
            };
        }

        public static TestDriveInfo ToEntity(this TestDriveInfoDto dto)
        {
            if (dto == null)
                return null!;

            return new TestDriveInfo
            {
                Available = dto.Available,
                BookingAmount = dto.BookingAmount
            };
        }

        // ─────────────────────────────────────────────────────────────
        // Helpers
        // ─────────────────────────────────────────────────────────────

        private static string GenerateSlug(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            value = value.ToLower().Trim();
            value = Regex.Replace(value, @"\s+", "-");
            value = Regex.Replace(value, @"[^a-z0-9\-]", "");
            value = Regex.Replace(value, @"-+", "-");
            value = value.Trim('-');

            return value;
        }

        private static string GenerateSlug(string brandName, string modelName)
        {
            return GenerateSlug($"{brandName}-{modelName}");
        }
    }
}