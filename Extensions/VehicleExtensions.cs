using AutoNext.Platform.Listings.API.Models;

namespace AutoNext.Platform.Listings.API.Extensions
{
    public static class VehicleExtensions
    {
        public static void UpdateTimestamps(this Vehicle vehicle, string modifiedBy)
        {
            vehicle.ModifiedBy = modifiedBy;
            vehicle.ModifiedOn = DateTime.Now;
            vehicle.Version++;
            vehicle.Dates.LastUpdated = DateTime.Now;
        }

        public static void MarkAsSold(this Vehicle vehicle, string modifiedBy, DateTime? soldDate = null)
        {
            vehicle.Status.CurrentStatus = "sold";
            vehicle.Dates.SoldDate = soldDate ?? DateTime.Now;
            vehicle.ModifiedBy = modifiedBy;
            vehicle.ModifiedOn = DateTime.Now;
            vehicle.UpdateTimestamps(modifiedBy);
        }

        public static void IncrementView(this Vehicle vehicle)
        {
            vehicle.Analytics.Views++;
        }

        public static void IncrementFavorite(this Vehicle vehicle)
        {
            vehicle.Analytics.Favorites++;
        }

        public static string GenerateVehicleId(this Vehicle vehicle)
        {
            // Format: VC_2024_00001
            var year = DateTime.Now.Year;
            var random = new Random();
            var randomNum = random.Next(10000, 99999);
            return $"VC_{year}_{randomNum}";
        }

        public static bool IsVINValid(this Vehicle vehicle)
        {
            // VIN should be 17 characters alphanumeric (excluding I, O, Q)
            if (string.IsNullOrEmpty(vehicle.VIN) || vehicle.VIN.Length != 17)
                return false;

            foreach (char c in vehicle.VIN)
            {
                if (c == 'I' || c == 'O' || c == 'Q')
                    return false;
            }
            return true;
        }
    }
}
