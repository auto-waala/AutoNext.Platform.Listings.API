using AutoNext.Platform.Listings.API.Models.DTOs;
using AutoNext.Platform.Listings.API.Models.Entities;
using System.Text.RegularExpressions;

namespace AutoNext.Platform.Listings.API.Models.Mappers
{
    public static class UsedVehiclesMapper
    {
        // ── Entity to Response DTO ─────────────────────────────────────────────

        public static UsedVehiclesResponseDto ToResponseDto(this UsedVehicles entity)
        {
            if (entity == null) return null!;

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
                Price = entity.Price != null ? new PriceInfoDto
                {
                    Amount = entity.Price.Amount,
                    Currency = entity.Price.Currency,
                    Negotiable = entity.Price.Negotiable,
                    OnRoadPrice = entity.Price.OnRoadPrice
                } : null!,
                PriceRangeFrom = entity.PriceRangeFrom,
                PriceRangeTo = entity.PriceRangeTo,
                Images = entity.Images?.Select(i => new VehicleImageDto
                {
                    FileId = i.FileId,
                    FileUrl = i.FileUrl,
                    IsPrimary = i.IsPrimary
                }).ToList() ?? new List<VehicleImageDto>(),
                Videos = entity.Videos?.Select(v => new VehicleMediaDto
                {
                    FileId = v.FileId,
                    FileUrl = v.FileUrl
                }).ToList() ?? new List<VehicleMediaDto>(),
                Shorts = entity.Shorts?.Select(s => new VehicleMediaDto
                {
                    FileId = s.FileId,
                    FileUrl = s.FileUrl
                }).ToList() ?? new List<VehicleMediaDto>(),
                Thumbnail = entity.Thumbnail,
                ThumbnailWebp = entity.ThumbnailWebp ?? string.Empty,
                Variants = entity.Variants?.Select(v => new VehicleVariantDto
                {
                    Color = v.Color,
                    Engine = v.Engine,
                    Transmission = v.Transmission,
                    FuelType = v.FuelType,
                    Mileage = v.Mileage,
                    YearOfManufacture = v.YearOfManufacture
                }).ToList() ?? new List<VehicleVariantDto>(),
                KeySpecifications = entity.KeySpecifications != null ? new KeySpecificationsDto
                {
                    Engine = entity.KeySpecifications.Engine,
                    Transmission = entity.KeySpecifications.Transmission,
                    FuelType = entity.KeySpecifications.FuelType,
                    Mileage = entity.KeySpecifications.Mileage,
                    YearOfManufacture = entity.KeySpecifications.YearOfManufacture
                } : null!,
                TopFeatures = entity.TopFeatures?.Select(tf => new FeatureItemDto
                {
                    Feature = tf.Feature
                }).ToList() ?? new List<FeatureItemDto>(),
                StandOutFeatures = entity.StandOutFeatures?.Select(sf => new FeatureItemDto
                {
                    Feature = sf.Feature
                }).ToList() ?? new List<FeatureItemDto>(),
                Pros = entity.Pros?.Select(p => new ProConItemDto
                {
                    Pro = p.Pro,
                    Con = p.Con
                }).ToList() ?? new List<ProConItemDto>(),
                Cons = entity.Cons?.Select(c => new ProConItemDto
                {
                    Pro = c.Pro,
                    Con = c.Con
                }).ToList() ?? new List<ProConItemDto>(),
                Tags = entity.Tags?.Select(t => new TagItemDto
                {
                    TagName = t.TagName
                }).ToList() ?? new List<TagItemDto>(),
                Rating = entity.Rating,
                UserRatings = entity.UserRatings?.Select(ur => new UserRatingDto
                {
                    UserId = ur.UserId,
                    Rating = ur.Rating,
                    Comment = ur.Comment,
                    CreatedAt = ur.CreatedAt
                }).ToList() ?? new List<UserRatingDto>(),
                Engagement = entity.Engagement != null ? new EngagementMetricsDto
                {
                    Views = entity.Engagement.Views,
                    Likes = entity.Engagement.Likes,
                    Shares = entity.Engagement.Shares,
                    Enquiries = entity.Engagement.Enquiries
                } : null!,
                Seller = MapSellerInfo(entity.Seller),
                Location = entity.Location != null ? new LocationInfoDto
                {
                    City = entity.Location.City,
                    State = entity.Location.State,
                    Pincode = entity.Location.Pincode,
                    Latitude = entity.Location.Latitude,
                    Longitude = entity.Location.Longitude
                } : null!,
                Condition = entity.Condition != null ? new VehicleConditionDto
                {
                    IsNew = entity.Condition.IsNew,
                    OwnerCount = entity.Condition.OwnerCount,
                    KMDriven = entity.Condition.KMDriven,
                    Accidental = entity.Condition.Accidental,
                    ServiceHistoryAvailable = entity.Condition.ServiceHistoryAvailable,
                    RegistrationYear = entity.Condition.RegistrationYear,
                    RegistrationMonth = entity.Condition.RegistrationMonth
                } : null!,
                ListingDetails = entity.ListingDetails != null ? new ListingDetailsDto
                {
                    IsAvailable = entity.ListingDetails.IsAvailable,
                    IsFeatured = entity.ListingDetails.IsFeatured,
                    IsSold = entity.ListingDetails.IsSold,
                    PostedDate = entity.ListingDetails.PostedDate,
                    ExpiryDate = entity.ListingDetails.ExpiryDate,
                    IsVerified = entity.ListingDetails.IsVerified,
                    VerifiedBy = entity.ListingDetails.VerifiedBy,
                    VerificationDate = entity.ListingDetails.VerificationDate
                } : null!,
                ShareUrls = entity.ShareUrls != null ? new ShareUrlsDto
                {
                    Facebook = entity.ShareUrls.Facebook,
                    Twitter = entity.ShareUrls.Twitter,
                    WhatsApp = entity.ShareUrls.WhatsApp,
                    LinkedIn = entity.ShareUrls.LinkedIn
                } : null!,
                TestDrive = entity.TestDrive != null ? new TestDriveInfoDto
                {
                    Available = entity.TestDrive.Available,
                    BookingAmount = entity.TestDrive.BookingAmount
                } : null!,
                Priority = entity.Priority,
                IsActive = entity.IsActive,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        // Helper method to handle Seller mapping
        private static SellerInfoDto? MapSellerInfo(SellerInfo seller)
        {
            if (seller == null) return null!;

            return new SellerInfoDto
            {
                UserId = seller.SellerId ?? string.Empty,
                Name = seller.SellerName ?? string.Empty,
                Email = seller.Email ?? string.Empty,
                SellerType = seller.SellerType ?? string.Empty,
                Phone = seller.ContactNumber ?? string.Empty,
                IsVerified = seller.IsVerifiedSeller,
                DealerId = seller.DealerName ?? string.Empty,
                Location = seller.DealerAddress ?? string.Empty
            };
        }

        public static List<UsedVehiclesResponseDto> ToResponseDtoList(this IEnumerable<UsedVehicles> entities)
        {
            return entities?.Select(e => e.ToResponseDto()).ToList() ?? new List<UsedVehiclesResponseDto>();
        }

        // ── Request DTO to Entity ─────────────────────────────────────────────

        public static UsedVehicles ToEntity(this UsedVehiclesRequestDto request)
        {
            if (request == null) return null!;

            return new UsedVehicles
            {
                Title = request.Title,
                Descriptions = request.Descriptions,
                Slug = string.IsNullOrEmpty(request.Slug) ? GenerateSlug(request.BrandName, request.ModelName) : request.Slug,
                MetaTitle = request.MetaTitle,
                MetaDescription = request.MetaDescription,
                BrandName = request.BrandName,
                ModelName = request.ModelName,
                ModelSlug = GenerateSlug(request.BrandName, request.ModelName),
                VehicleType = request.VehicleType,
                BodyType = request.BodyType,
                Price = request.Price != null ? new PriceInfo
                {
                    Amount = request.Price.Amount,
                    Currency = request.Price.Currency,
                    Negotiable = request.Price.Negotiable,
                    OnRoadPrice = request.Price.OnRoadPrice
                } : null!,
                PriceRangeFrom = request.PriceRangeFrom,
                PriceRangeTo = request.PriceRangeTo,
                Images = request.Images?.Select(i => new VehicleImage
                {
                    FileId = i.FileId,
                    FileUrl = i.FileUrl,
                    IsPrimary = i.IsPrimary
                }).ToList() ?? new List<VehicleImage>(),
                Videos = request.Videos?.Select(v => new VehicleMedia
                {
                    FileId = v.FileId,
                    FileUrl = v.FileUrl
                }).ToList() ?? new List<VehicleMedia>(),
                Shorts = request.Shorts?.Select(s => new VehicleMedia
                {
                    FileId = s.FileId,
                    FileUrl = s.FileUrl
                }).ToList() ?? new List<VehicleMedia>(),
                Thumbnail = GetThumbnailFromImages(request.Images),
                ThumbnailWebp = null,
                Variants = request.Variants?.Select(v => new VehicleVariant
                {
                    Color = v.Color,
                    Engine = v.Engine,
                    Transmission = v.Transmission,
                    FuelType = v.FuelType,
                    Mileage = v.Mileage,
                    YearOfManufacture = v.YearOfManufacture
                }).ToList() ?? new List<VehicleVariant>(),
                KeySpecifications = request.KeySpecifications != null ? new KeySpecifications
                {
                    Engine = request.KeySpecifications.Engine,
                    Transmission = request.KeySpecifications.Transmission,
                    FuelType = request.KeySpecifications.FuelType,
                    Mileage = request.KeySpecifications.Mileage,
                    YearOfManufacture = request.KeySpecifications.YearOfManufacture
                } : null!,
                TopFeatures = request.TopFeatures?.Select(tf => new FeatureItem
                {
                    Feature = tf.Feature
                }).ToList() ?? new List<FeatureItem>(),
                StandOutFeatures = request.StandOutFeatures?.Select(sf => new FeatureItem
                {
                    Feature = sf.Feature
                }).ToList() ?? new List<FeatureItem>(),
                Pros = request.Pros?.Select(p => new ProConItem
                {
                    Pro = p.Pro,
                    Con = p.Con
                }).ToList() ?? new List<ProConItem>(),
                Cons = request.Cons?.Select(c => new ProConItem
                {
                    Pro = c.Pro,
                    Con = c.Con
                }).ToList() ?? new List<ProConItem>(),
                Tags = request.Tags?.Select(t => new TagItem
                {
                    TagName = t.TagName
                }).ToList() ?? new List<TagItem>(),
                UserRatings = request.UserRatings?.Select(ur => new UserRating
                {
                    UserId = ur.UserId,
                    Rating = ur.Rating,
                    Comment = ur.Comment,
                    CreatedAt = DateTime.UtcNow
                }).ToList() ?? new List<UserRating>(),
                Rating = CalculateAverageRating(request.UserRatings),
                Seller = request.Seller != null ? new SellerInfo
                {
                    SellerId = request.Seller.UserId ?? string.Empty,
                    SellerName = request.Seller.Name ?? string.Empty,
                    Email = request.Seller.Email ?? string.Empty,
                    SellerType = request.Seller.SellerType ?? string.Empty,
                    ContactNumber = request.Seller.Phone ?? string.Empty,
                    IsVerifiedSeller = request.Seller.IsVerified,
                    DealerName = request.Seller.DealerId ?? string.Empty,
                    DealerAddress = request.Seller.Location ?? string.Empty
                } : null!,
                Location = request.Location != null ? new LocationInfo
                {
                    City = request.Location.City,
                    State = request.Location.State,
                    Pincode = request.Location.Pincode,
                    Latitude = request.Location.Latitude,
                    Longitude = request.Location.Longitude
                } : null!,
                Condition = request.Condition != null ? new VehicleCondition
                {
                    IsNew = request.Condition.IsNew,
                    OwnerCount = request.Condition.OwnerCount,
                    KMDriven = request.Condition.KMDriven,
                    Accidental = request.Condition.Accidental,
                    ServiceHistoryAvailable = request.Condition.ServiceHistoryAvailable,
                    RegistrationYear = request.Condition.RegistrationYear,
                    RegistrationMonth = request.Condition.RegistrationMonth
                } : null!,
                ListingDetails = new ListingDetails
                {
                    IsAvailable = request.ListingDetails?.IsAvailable ?? true,
                    IsFeatured = request.ListingDetails?.IsFeatured ?? false,
                    IsSold = request.ListingDetails?.IsSold ?? false,
                    PostedDate = DateTime.UtcNow,
                    ExpiryDate = request.ListingDetails?.ExpiryDate,
                    IsVerified = request.ListingDetails?.IsVerified ?? false,
                    VerifiedBy = request.ListingDetails?.VerifiedBy ?? string.Empty,
                    VerificationDate = request.ListingDetails?.VerificationDate
                },
                Engagement = new EngagementMetrics
                {
                    Views = 0,
                    Likes = 0,
                    Shares = 0,
                    Enquiries = 0
                },
                ShareUrls = new ShareUrls(),
                TestDrive = request.TestDrive != null ? new TestDriveInfo
                {
                    Available = request.TestDrive.Available,
                    BookingAmount = request.TestDrive.BookingAmount
                } : null!,
                Priority = request.Priority,
                IsActive = request.IsActive,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        // ── Update Entity from Request ────────────────────────────────────────

        public static void UpdateFromRequest(this UsedVehicles entity, UsedVehiclesRequestDto request)
        {
            if (entity == null || request == null) return;

            entity.Title = request.Title;
            entity.Descriptions = request.Descriptions;
            entity.Slug = string.IsNullOrEmpty(request.Slug) ? GenerateSlug(request.BrandName, request.ModelName) : request.Slug;
            entity.MetaTitle = request.MetaTitle;
            entity.MetaDescription = request.MetaDescription;
            entity.BrandName = request.BrandName;
            entity.ModelName = request.ModelName;
            entity.ModelSlug = GenerateSlug(request.BrandName, request.ModelName);
            entity.VehicleType = request.VehicleType;
            entity.BodyType = request.BodyType;

            if (entity.Price == null) entity.Price = new PriceInfo();
            entity.Price.Amount = request.Price?.Amount ?? 0;
            entity.Price.Currency = request.Price?.Currency ?? "INR";
            entity.Price.Negotiable = request.Price?.Negotiable ?? true;
            entity.Price.OnRoadPrice = request.Price?.OnRoadPrice ?? 0;

            entity.PriceRangeFrom = request.PriceRangeFrom;
            entity.PriceRangeTo = request.PriceRangeTo;

            entity.Images = request.Images?.Select(i => new VehicleImage
            {
                FileId = i.FileId,
                FileUrl = i.FileUrl,
                IsPrimary = i.IsPrimary
            }).ToList() ?? new List<VehicleImage>();

            entity.Thumbnail = GetThumbnailFromImages(request.Images);

            entity.Videos = request.Videos?.Select(v => new VehicleMedia
            {
                FileId = v.FileId,
                FileUrl = v.FileUrl
            }).ToList() ?? new List<VehicleMedia>();

            entity.Shorts = request.Shorts?.Select(s => new VehicleMedia
            {
                FileId = s.FileId,
                FileUrl = s.FileUrl
            }).ToList() ?? new List<VehicleMedia>();

            entity.Variants = request.Variants?.Select(v => new VehicleVariant
            {
                Color = v.Color,
                Engine = v.Engine,
                Transmission = v.Transmission,
                FuelType = v.FuelType,
                Mileage = v.Mileage,
                YearOfManufacture = v.YearOfManufacture
            }).ToList() ?? new List<VehicleVariant>();

            if (entity.KeySpecifications == null) entity.KeySpecifications = new KeySpecifications();
            entity.KeySpecifications.Engine = request.KeySpecifications?.Engine ?? string.Empty;
            entity.KeySpecifications.Transmission = request.KeySpecifications?.Transmission ?? string.Empty;
            entity.KeySpecifications.FuelType = request.KeySpecifications?.FuelType ?? string.Empty;
            entity.KeySpecifications.Mileage = request.KeySpecifications?.Mileage ?? string.Empty;
            entity.KeySpecifications.YearOfManufacture = request.KeySpecifications?.YearOfManufacture ?? string.Empty;

            entity.TopFeatures = request.TopFeatures?.Select(tf => new FeatureItem
            {
                Feature = tf.Feature
            }).ToList() ?? new List<FeatureItem>();

            entity.StandOutFeatures = request.StandOutFeatures?.Select(sf => new FeatureItem
            {
                Feature = sf.Feature
            }).ToList() ?? new List<FeatureItem>();

            entity.Pros = request.Pros?.Select(p => new ProConItem
            {
                Pro = p.Pro,
                Con = p.Con
            }).ToList() ?? new List<ProConItem>();

            entity.Cons = request.Cons?.Select(c => new ProConItem
            {
                Pro = c.Pro,
                Con = c.Con
            }).ToList() ?? new List<ProConItem>();

            entity.Tags = request.Tags?.Select(t => new TagItem
            {
                TagName = t.TagName
            }).ToList() ?? new List<TagItem>();

            entity.UserRatings = request.UserRatings?.Select(ur => new UserRating
            {
                UserId = ur.UserId,
                Rating = ur.Rating,
                Comment = ur.Comment,
                CreatedAt = ur.CreatedAt == default ? DateTime.UtcNow : ur.CreatedAt
            }).ToList() ?? new List<UserRating>();

            entity.Rating = CalculateAverageRating(entity.UserRatings);

            if (entity.Seller == null) entity.Seller = new SellerInfo();
            entity.Seller.SellerId = request.Seller?.UserId ?? entity.Seller.SellerId;
            entity.Seller.SellerName = request.Seller?.Name ?? entity.Seller.SellerName;
            entity.Seller.Email = request.Seller?.Email ?? entity.Seller.Email;
            entity.Seller.SellerType = request.Seller?.SellerType ?? entity.Seller.SellerType;
            entity.Seller.ContactNumber = request.Seller?.Phone ?? entity.Seller.ContactNumber;
            entity.Seller.IsVerifiedSeller = request.Seller?.IsVerified ?? entity.Seller.IsVerifiedSeller;
            entity.Seller.DealerName = request.Seller?.DealerId ?? entity.Seller.DealerName;
            entity.Seller.DealerAddress = request.Seller?.Location ?? entity.Seller.DealerAddress;

            if (entity.Location == null) entity.Location = new LocationInfo();
            entity.Location.City = request.Location?.City ?? string.Empty;
            entity.Location.State = request.Location?.State ?? string.Empty;
            entity.Location.Pincode = request.Location?.Pincode ?? string.Empty;
            entity.Location.Latitude = request.Location?.Latitude ?? string.Empty;
            entity.Location.Longitude = request.Location?.Longitude ?? string.Empty;

            if (entity.Condition == null) entity.Condition = new VehicleCondition();
            entity.Condition.IsNew = request.Condition?.IsNew ?? true;
            entity.Condition.OwnerCount = request.Condition?.OwnerCount ?? 0;
            entity.Condition.KMDriven = request.Condition?.KMDriven ?? 0;
            entity.Condition.Accidental = request.Condition?.Accidental ?? false;
            entity.Condition.ServiceHistoryAvailable = request.Condition?.ServiceHistoryAvailable ?? true;
            entity.Condition.RegistrationYear = request.Condition != null ? request.Condition.RegistrationYear : 0;
            entity.Condition.RegistrationMonth = request.Condition != null ? request.Condition.RegistrationMonth : 0;

            if (entity.ListingDetails == null) entity.ListingDetails = new ListingDetails();
            entity.ListingDetails.IsAvailable = request.ListingDetails?.IsAvailable ?? true;
            entity.ListingDetails.IsFeatured = request.ListingDetails?.IsFeatured ?? false;
            entity.ListingDetails.IsSold = request.ListingDetails?.IsSold ?? false;
            entity.ListingDetails.ExpiryDate = request.ListingDetails?.ExpiryDate;
            entity.ListingDetails.IsVerified = request.ListingDetails?.IsVerified ?? false;
            entity.ListingDetails.VerifiedBy = request.ListingDetails?.VerifiedBy ?? string.Empty;
            entity.ListingDetails.VerificationDate = request.ListingDetails?.VerificationDate;

            if (entity.TestDrive == null) entity.TestDrive = new TestDriveInfo();
            entity.TestDrive.Available = request.TestDrive?.Available ?? true;
            entity.TestDrive.BookingAmount = request.TestDrive?.BookingAmount ?? 0;

            entity.Priority = request.Priority;
            entity.IsActive = request.IsActive;
            entity.StartDate = request.StartDate;
            entity.EndDate = request.EndDate;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        // ── Private Helpers ───────────────────────────────────────────────────

        private static string GenerateSlug(string brandName, string modelName)
        {
            if (string.IsNullOrEmpty(brandName) && string.IsNullOrEmpty(modelName))
                return string.Empty;

            var slug = $"{brandName}-{modelName}".ToLower();
            slug = slug.Replace(" ", "-");
            slug = Regex.Replace(slug, @"[^a-z0-9-]", "");
            return slug.Trim('-');
        }

        private static string GetThumbnailFromImages(List<VehicleImageDto>? images)
        {
            if (images == null || !images.Any())
                return string.Empty;

            var primaryImage = images.FirstOrDefault(i => i.IsPrimary);
            return primaryImage?.FileUrl ?? images.First().FileUrl;
        }

        private static double CalculateAverageRating(List<UserRating>? ratings)
        {
            if (ratings == null || !ratings.Any())
                return 0;

            return Math.Round(ratings.Average(r => r.Rating), 1);
        }

        private static double CalculateAverageRating(List<UserRatingDto>? ratings)
        {
            if (ratings == null || !ratings.Any())
                return 0;

            return Math.Round(ratings.Average(r => r.Rating), 1);
        }
    }
}