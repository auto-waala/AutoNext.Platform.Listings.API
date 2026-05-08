namespace AutoNext.Platform.Listings.API.Models
{
    public class CreateVehicleRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string VIN { get; set; } = string.Empty;
        public string ChassisNumber { get; set; } = string.Empty;
        public string EngineNumber { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsNegotiable { get; set; } = true;
        public VehicleSpecifications Specifications { get; set; } = new();
        public VehicleType VehicleType { get; set; } = new();
        public List<VehicleLocation> Locations { get; set; } = new();
        public List<VehicleImage> Images { get; set; } = new();
        public Seller Seller { get; set; } = new();
    }

    public class UpdateVehicleRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsNegotiable { get; set; } = true;
        public VehicleSpecifications Specifications { get; set; } = new();
        public List<VehicleLocation> Locations { get; set; } = new();
        public List<VehicleImage> Images { get; set; } = new();
    }

    public class MarkAsSoldRequest
    {
        public DateTime? SoldDate { get; set; }
    }

    public class ExtendListingRequest
    {
        public int AdditionalDays { get; set; } = 30;
    }

    public class TrackViewRequest
    {
        public string? Source { get; set; }
    }

    public class TrackInquiryRequest
    {
        public InquiryType InquiryType { get; set; }
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        public static ApiResponse<T> Ok(T data, string message = "Success")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                StatusCode = 200,
                Data = data
            };
        }

        public static ApiResponse<object> Error(string message, int statusCode = 400, List<string>? errors = null)
        {
            return new ApiResponse<object>
            {
                Success = false,
                Message = message,
                StatusCode = statusCode,
                Errors = errors
            };
        }

        public static ApiResponse<object> Unauthorized(string message = "Unauthorized")
        {
            return new ApiResponse<object>
            {
                Success = false,
                Message = message,
                StatusCode = 401
            };
        }
    }
}