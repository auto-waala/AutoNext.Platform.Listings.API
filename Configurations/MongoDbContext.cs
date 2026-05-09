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

            if (string.IsNullOrEmpty(mongoSettings.ConnectionString))
                throw new ArgumentNullException(nameof(mongoSettings.ConnectionString), "MongoDB connection string is not configured");

            if (string.IsNullOrEmpty(mongoSettings.DatabaseName))
                throw new ArgumentNullException(nameof(mongoSettings.DatabaseName), "MongoDB database name is not configured");

            var clientSettings = MongoClientSettings.FromConnectionString(mongoSettings.ConnectionString);

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

        public IMongoCollection<T> GetCollection<T>(string collectionName) => _database.GetCollection<T>(collectionName);

        public IMongoDatabase GetDatabase() => _database;

        public async Task<IClientSessionHandle> StartSessionAsync() => await _client.StartSessionAsync();
    }
}
