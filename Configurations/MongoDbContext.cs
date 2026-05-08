using AutoNext.Platform.Listings.API.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AutoNext.Platform.Listings.API.Configurations
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        private readonly ILogger<MongoDbContext> _logger;
        private readonly MongoDbSettings _settings;

        public MongoDbContext(
            IConfiguration configuration,
            IOptions<MongoDbSettings> settings,
            ILogger<MongoDbContext> logger)
        {
            _logger = logger;
            _settings = settings.Value;

            try
            {
                var connectionString = configuration.GetConnectionString("MongoDB");
                var databaseName = _settings.DatabaseName;

                if (string.IsNullOrEmpty(connectionString))
                    throw new InvalidOperationException("MongoDB connection string is missing");

                if (string.IsNullOrEmpty(databaseName))
                    throw new InvalidOperationException("Database name is missing in configuration");

                _logger.LogInformation("Connecting to MongoDB...");

                var client = new MongoClient(connectionString);
                _database = client.GetDatabase(databaseName);

                // Test connection
                var pingCommand = new BsonDocument("ping", 1);
                _database.RunCommand<BsonDocument>(pingCommand);

                _logger.LogInformation("Successfully connected to MongoDB database: {DatabaseName}", databaseName);

                // Create indexes asynchronously
                Task.Run(async () => await CreateIndexesAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to MongoDB");
                throw;
            }
        }

        public IMongoCollection<Vehicle> Vehicles =>
            _database.GetCollection<Vehicle>(_settings.VehicleCollection);

        //public IMongoCollection<Client> Clients =>
        //    _database.GetCollection<Client>(_settings.ClientsCollection);

        private async Task CreateIndexesAsync()
        {
            try
            {
                var vehiclesCollection = _database.GetCollection<Vehicle>(_settings.VehicleCollection);

                var indexKeysDefinition = Builders<Vehicle>.IndexKeys;

                // Create indexes for common queries
                vehiclesCollection.Indexes.CreateOne(new CreateIndexModel<Vehicle>(
                    indexKeysDefinition.Ascending(v => v.VIN),
                    new CreateIndexOptions { Unique = true, Name = "idx_vin" }
                ));

                vehiclesCollection.Indexes.CreateOne(new CreateIndexModel<Vehicle>(
                    indexKeysDefinition.Ascending(v => v.ChassisNumber),
                    new CreateIndexOptions { Unique = true, Name = "idx_chassis" }
                ));

                vehiclesCollection.Indexes.CreateOne(new CreateIndexModel<Vehicle>(
                    indexKeysDefinition.Ascending(v => v.EngineNumber),
                    new CreateIndexOptions { Unique = true, Name = "idx_engine" }
                ));

                vehiclesCollection.Indexes.CreateOne(new CreateIndexModel<Vehicle>(
                    indexKeysDefinition.Ascending(v => v.RegistrationNumber),
                    new CreateIndexOptions { Name = "idx_registration" }
                ));

                vehiclesCollection.Indexes.CreateOne(new CreateIndexModel<Vehicle>(
                    indexKeysDefinition.Ascending(v => v.Seller.UserId),
                    new CreateIndexOptions { Name = "idx_seller" }
                ));

                vehiclesCollection.Indexes.CreateOne(new CreateIndexModel<Vehicle>(
                    indexKeysDefinition.Combine(
                        indexKeysDefinition.Ascending(v => v.VehicleType.Type),
                        indexKeysDefinition.Ascending(v => v.Status.CurrentStatus),
                        indexKeysDefinition.Descending(v => v.CreatedOn)
                    ),
                    new CreateIndexOptions { Name = "idx_type_status_date" }
                ));

                // Geospatial index
                vehiclesCollection.Indexes.CreateOne(new CreateIndexModel<Vehicle>(
                    indexKeysDefinition.Geo2DSphere(v => v.Locations[0]),
                    new CreateIndexOptions { Name = "idx_location" }
                ));

                _logger.LogInformation("MongoDB indexes created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to create indexes: {Message}", ex.Message);
            }
        }
    }
}
