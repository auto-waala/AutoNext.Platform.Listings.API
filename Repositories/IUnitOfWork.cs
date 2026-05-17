namespace AutoNext.Platform.Listings.API.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IVehicleRepository Vehicles { get; }
        IFeaturedVehicleRepository FeaturedVehicles { get; }
        INewlyArrivedRepository NewlyArrivedVehicles { get; }
        IUsedVehiclesRepository UsedVehicles { get; }
        Task<int> SaveChangesAsync();
    }
}
