using AutoNext.Platform.Listings.API.Configurations;
using MongoDB.Driver;

namespace AutoNext.Platform.Listings.API.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IClientSessionHandle? _session;
        private bool _disposed;

        public IVehicleRepository Vehicles { get; }
        public INewlyArrivedRepository NewlyArrivedVehicles { get; }
        public IFeaturedVehicleRepository FeaturedVehicles { get; }
        public IUsedVehiclesRepository UsedVehicles { get; }
        public IPremiumVehicleRepository PremiumVehicles { get; }

        public UnitOfWork(MongoDbContext context)
        {
            Vehicles = new VehicleRepository(context);
            NewlyArrivedVehicles = new NewlyArrivedRepository(context);
            FeaturedVehicles = new FeaturedVehicleRepository(context);
            UsedVehicles = new UsedVehiclesRepository(context);
            PremiumVehicles = new PremiumVehicleRepository(context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await Task.FromResult(1);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _session?.Dispose();
                _disposed = true;
            }
        }
    }
}
