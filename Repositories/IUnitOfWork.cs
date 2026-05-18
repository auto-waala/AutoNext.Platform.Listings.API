namespace AutoNext.Platform.Listings.API.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IVehicleRepository Vehicles { get; }

        INewlyArrivedRepository NewlyArrivedVehicles { get; }

        IFeaturedVehicleRepository FeaturedVehicles { get; }

        IPremiumVehicleRepository PremiumVehicles { get; }

        Task<int> SaveChangesAsync();
    }
}
