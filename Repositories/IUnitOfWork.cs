namespace AutoNext.Platform.Listings.API.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IVehicleRepository Vehicles { get; }
        Task<int> SaveChangesAsync();
    }
}
