using AutoNext.Platform.Listings.API.Models.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AutoNext.Platform.Listings.API.Configurations
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        private readonly MongoClient _client;

        // Only one constructor - using IOptions pattern
        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var mongoSettings = settings.Value ?? throw new ArgumentNullException(nameof(settings), "MongoDB settings are null");

            // PRIORITY: Environment Variable > appsettings.json
            var connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTIONSTRING")
                                   ?? Environment.GetEnvironmentVariable("MongoDB__ConnectionString")
                                   ?? Environment.GetEnvironmentVariable("ConnectionStrings__MongoDB")
                                   ?? mongoSettings.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString), "MongoDB connection string is not configured");

            // Check for unreplaced placeholders
            if (connectionString.Contains("__"))
            {
                throw new InvalidOperationException(
                    $"Invalid MongoDB connection string. It still contains placeholders: {connectionString}. " +
                    $"Environment: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Not set"}. " +
                    "Please check your configuration injection in the deployment pipeline."
                );
            }

            if (string.IsNullOrEmpty(mongoSettings.DatabaseName))
                throw new ArgumentNullException(nameof(mongoSettings.DatabaseName), "MongoDB database name is not configured");

            var clientSettings = MongoClientSettings.FromConnectionString(connectionString);

            // Configure connection pool
            clientSettings.MinConnectionPoolSize = mongoSettings.MinConnectionPoolSize ?? 10;
            clientSettings.MaxConnectionPoolSize = mongoSettings.MaxConnectionPoolSize ?? 100;
            clientSettings.ConnectTimeout = TimeSpan.FromSeconds(mongoSettings.ConnectTimeoutSeconds ?? 10);
            clientSettings.SocketTimeout = TimeSpan.FromSeconds(mongoSettings.SocketTimeoutSeconds ?? 30);
            clientSettings.ServerSelectionTimeout = TimeSpan.FromSeconds(mongoSettings.ServerSelectionTimeoutSeconds ?? 15);
            clientSettings.RetryWrites = mongoSettings.RetryWrites ?? true;
            clientSettings.RetryReads = mongoSettings.RetryReads ?? true;

            _client = new MongoClient(clientSettings);
            _database = _client.GetDatabase(mongoSettings.DatabaseName);
        }

        public IMongoCollection<Vehicle> Vehicles => _database.GetCollection<Vehicle>("vehicles");
        public IMongoCollection<NewlyArrivedVehicle> NewlyArrivedVehicles => _database.GetCollection<NewlyArrivedVehicle>("newly_arrived_vehicles");

        public IMongoCollection<T> GetCollection<T>(string collectionName) => _database.GetCollection<T>(collectionName);

        public IMongoDatabase GetDatabase() => _database;

        public async Task<IClientSessionHandle> StartSessionAsync() => await _client.StartSessionAsync();
    }
}