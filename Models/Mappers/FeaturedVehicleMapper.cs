using AutoNext.Platform.Listings.API.Models.DTOs;
using AutoNext.Platform.Listings.API.Models.Entities;

namespace AutoNext.Platform.Listings.API.Models.Mappers
{
    public static class FeaturedVehicleMapper
    {
        // ── Entity to Response DTO ─────────────────────────────────────────────

        public static FeaturedVehicleResponseDto ToResponseDto(this FeaturedVehicle entity)
        {
            if (entity == null) return null!;

            return new FeaturedVehicleResponseDto
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
                PriceRange = GetPriceRange(entity.Price?.Amount, entity.PriceRangeFrom, entity.PriceRangeTo),
                PriceRangeFrom = entity.PriceRangeFrom,
                PriceRangeTo = entity.PriceRangeTo,
                Images = entity.Images?.Select(i => new ImageDto
                {
                    FileId = i.FileId,
                    FileUrl = i.FileUrl,
                    IsPrimary = i.IsPrimary
                }).ToList() ?? new List<ImageDto>(),
                Videos = entity.Videos?.Select(v => new VideoDto
                {
                    FileUrl = v.FileUrl
                }).ToList() ?? new List<VideoDto>(),
                Shorts = entity.Shorts?.Select(s => new VideoDto
                {
                    FileUrl = s.FileUrl
                }).ToList() ?? new List<VideoDto>(),
                ThumbnailImage = entity.Thumbnail,
                ThumbnailWebp = entity.ThumbnailWebp ?? string.Empty,
                Variants = entity.Variants?.Select(v => new VariantDetailDto
                {
                    Color = v.Color,
                    Engine = v.Engine,
                    Transmission = v.Transmission,
                    FuelType = v.FuelType,
                    Mileage = v.Mileage,
                    YearOfManufacture = v.YearOfManufacture
                }).ToList() ?? new List<VariantDetailDto>(),
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
                ReviewCount = entity.UserRatings?.Count ?? 0,
                UserRatings = entity.UserRatings?.Select(ur => new UserRatingDto
                {
                    UserId = ur.UserId,
                    Rating = ur.Rating,
                    Comment = ur.Comment,
                    CreatedAt = ur.CreatedAt
                }).ToList() ?? new List<UserRatingDto>(),
                Seller = entity.Seller != null ? new SellerInfoDto
                {
                    UserId = entity.Seller.UserId,
                    Name = entity.Seller.Name,
                    Email = entity.Seller.Email,
                } : null!,
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
                    ServiceHistoryAvailable = entity.Condition.ServiceHistoryAvailable
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
                Engagement = entity.Engagement != null ? new EngagementMetricsDto
                {
                    Views = entity.Engagement.Views,
                    Likes = entity.Engagement.Likes,
                    Shares = entity.Engagement.Shares,
                    Enquiries = entity.Engagement.Enquiries
                } : null!,
                ShareUrls = entity.ShareUrls != null ? new ShareUrlsDto
                {
                    Facebook = entity.ShareUrls.Facebook,
                    Twitter = entity.ShareUrls.Twitter,
                    WhatsApp = entity.ShareUrls.WhatsApp,
                    LinkedIn = entity.ShareUrls.LinkedIn
                } : null!,
                Badges = entity.Badges ?? new List<string>(),
                Highlight = entity.Highlight ?? string.Empty,
                TestDrive = entity.TestDrive != null ? new TestDriveInfoDto
                {
                    Available = entity.TestDrive.Available,
                    BookingAmount = entity.TestDrive.BookingAmount
                } : null!,
                Priority = entity.Priority,
                IsActive = entity.IsActive,
                IsFeatured = entity.ListingDetails?.IsFeatured ?? false,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        public static List<FeaturedVehicleResponseDto> ToResponseDtoList(this IEnumerable<FeaturedVehicle> entities)
        {
            return entities?.Select(e => e.ToResponseDto()).ToList() ?? new List<FeaturedVehicleResponseDto>();
        }

        // ── Summary DTO for List Views ─────────────────────────────────────────

        public static FeaturedVehicleSummaryDto ToSummaryDto(this FeaturedVehicle entity)
        {
            if (entity == null) return null!;

            return new FeaturedVehicleSummaryDto
            {
                Id = entity.Id,
                Title = entity.Title,
                BrandName = entity.BrandName,
                ModelName = entity.ModelName,
                ThumbnailImage = entity.Thumbnail,
                Price = entity.Price?.Amount ?? 0,
                FormattedPrice = FormatPrice(entity.Price?.Amount ?? 0, entity.Price?.Currency ?? "INR"),
                Badges = entity.Badges ?? new List<string>(),
                Rating = entity.Rating,
                Priority = entity.Priority,
                IsActive = entity.IsActive,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate
            };
        }

        public static List<FeaturedVehicleSummaryDto> ToSummaryDtoList(this IEnumerable<FeaturedVehicle> entities)
        {
            return entities?.Select(e => e.ToSummaryDto()).ToList() ?? new List<FeaturedVehicleSummaryDto>();
        }

        // ── Request DTO to Entity ─────────────────────────────────────────────

        public static FeaturedVehicle ToEntity(this FeaturedVehicleRequestDto request)
        {
            if (request == null) return null!;

            return new FeaturedVehicle
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
                    FileUrl = v.FileUrl
                }).ToList() ?? new List<VehicleMedia>(),
                Shorts = request.Shorts?.Select(s => new VehicleMedia
                {
                    FileUrl = s.FileUrl
                }).ToList() ?? new List<VehicleMedia>(),
                Thumbnail = GetThumbnailFromImages(request.Images),
                ThumbnailWebp = null, // Will be generated by image processing service
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
                    UserId = request.Seller.UserId,
                    Name = request.Seller.Name,
                    Email = request.Seller.Email,
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
                    ServiceHistoryAvailable = request.Condition.ServiceHistoryAvailable
                } : null!,
                ListingDetails = new ListingDetails
                {
                    IsAvailable = request.ListingDetails?.IsAvailable ?? true,
                    IsFeatured = true, // Featured vehicle is always featured
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
                ShareUrls = new ShareUrls(), // Will be generated by URL service
                Badges = request.Badges ?? new List<string>(),
                Highlight = request.Highlight,
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

        public static void UpdateFromRequest(this FeaturedVehicle entity, FeaturedVehicleRequestDto request)
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
                FileUrl = v.FileUrl
            }).ToList() ?? new List<VehicleMedia>();

            entity.Shorts = request.Shorts?.Select(s => new VehicleMedia
            {
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

            // Don't update UserRatings from request, use AddRating method instead
            entity.Rating = CalculateAverageRating(entity.UserRatings);

            if (entity.Seller == null) entity.Seller = new SellerInfo();
            entity.Seller.UserId = request.Seller?.UserId ?? string.Empty;
            entity.Seller.Name = request.Seller?.Name ?? string.Empty;
            entity.Seller.Email = request.Seller?.Email ?? string.Empty;

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

            if (entity.ListingDetails == null) entity.ListingDetails = new ListingDetails();
            entity.ListingDetails.IsAvailable = request.ListingDetails?.IsAvailable ?? true;
            entity.ListingDetails.IsSold = request.ListingDetails?.IsSold ?? false;
            entity.ListingDetails.ExpiryDate = request.ListingDetails?.ExpiryDate;
            entity.ListingDetails.IsVerified = request.ListingDetails?.IsVerified ?? false;
            entity.ListingDetails.VerifiedBy = request.ListingDetails?.VerifiedBy ?? string.Empty;
            entity.ListingDetails.VerificationDate = request.ListingDetails?.VerificationDate;

            entity.Badges = request.Badges ?? new List<string>();
            entity.Highlight = request.Highlight;

            if (entity.TestDrive == null) entity.TestDrive = new TestDriveInfo();
            entity.TestDrive.Available = request.TestDrive?.Available ?? true;
            entity.TestDrive.BookingAmount = request.TestDrive?.BookingAmount ?? 0;

            entity.Priority = request.Priority;
            entity.IsActive = request.IsActive;
            entity.StartDate = request.StartDate;
            entity.EndDate = request.EndDate;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        // ── Featured Vehicle Specific Operations ─────────────────────────────

        public static void UpdatePriority(this FeaturedVehicle entity, int priority)
        {
            if (entity == null) return;
            entity.Priority = priority;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        public static void Activate(this FeaturedVehicle entity, DateTime? endDate = null)
        {
            if (entity == null) return;
            entity.IsActive = true;
            entity.StartDate = DateTime.UtcNow;
            entity.EndDate = endDate;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        public static void Deactivate(this FeaturedVehicle entity)
        {
            if (entity == null) return;
            entity.IsActive = false;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        public static void AddRating(this FeaturedVehicle entity, UserRatingDto rating)
        {
            if (entity == null || rating == null) return;

            if (entity.UserRatings == null)
                entity.UserRatings = new List<UserRating>();

            entity.UserRatings.Add(new UserRating
            {
                UserId = rating.UserId,
                Rating = rating.Rating,
                Comment = rating.Comment,
                CreatedAt = DateTime.UtcNow
            });

            entity.Rating = CalculateAverageRating(entity.UserRatings);
            entity.UpdatedAt = DateTime.UtcNow;
        }

        public static void IncrementEngagement(this FeaturedVehicle entity, string metricType)
        {
            if (entity == null) return;

            if (entity.Engagement == null)
                entity.Engagement = new EngagementMetrics();

            switch (metricType.ToLower())
            {
                case "views":
                    entity.Engagement.Views++;
                    break;
                case "likes":
                    entity.Engagement.Likes++;
                    break;
                case "shares":
                    entity.Engagement.Shares++;
                    break;
                case "enquiries":
                    entity.Engagement.Enquiries++;
                    break;
            }

            entity.UpdatedAt = DateTime.UtcNow;
        }

        public static bool IsCurrentlyFeatured(this FeaturedVehicle entity)
        {
            if (entity == null) return false;

            return entity.IsActive &&
                   entity.ListingDetails?.IsAvailable == true &&
                   entity.StartDate <= DateTime.UtcNow &&
                   (!entity.EndDate.HasValue || entity.EndDate.Value >= DateTime.UtcNow);
        }

        // ── Private Helpers ───────────────────────────────────────────────────

        private static string GenerateSlug(string brandName, string modelName)
        {
            if (string.IsNullOrEmpty(brandName) && string.IsNullOrEmpty(modelName))
                return string.Empty;

            var slug = $"{brandName}-{modelName}".ToLower();
            slug = slug.Replace(" ", "-");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9-]", "");
            return slug.Trim('-');
        }

        private static string FormatPrice(decimal price, string currency)
        {
            if (price >= 10000000) // 1 Crore+
                return $"{currency} {price / 10000000:0.##} Crore";
            if (price >= 100000) // 1 Lakh+
                return $"{currency} {price / 100000:0.##} Lakh";
            return $"{currency} {price:N0}";
        }

        private static string GetPriceRange(decimal? amount, string rangeFrom, string rangeTo)
        {
            if (!string.IsNullOrEmpty(rangeFrom) && !string.IsNullOrEmpty(rangeTo))
                return $"{rangeFrom} - {rangeTo}";

            if (amount.HasValue && amount.Value > 0)
                return FormatPrice(amount.Value, "₹");

            return "Price on request";
        }

        private static string GetThumbnailFromImages(List<ImageDto>? images)
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