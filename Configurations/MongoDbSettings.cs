namespace AutoNext.Platform.Listings.API.Configurations
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string VehiclesCollectionName { get; set; } = "vehicles";
        public string ClientsCollectionName { get; set; } = "api_clients";
        public string LogsCollectionName { get; set; } = "api_logs";
        public string OutboxCollectionName { get; set; } = "outbox_messages";
        public string EventStoreCollectionName { get; set; } = "event_store";

        // Connection Pool Settings
        public int? MinConnectionPoolSize { get; set; }
        public int? MaxConnectionPoolSize { get; set; }
        public int? ConnectTimeoutSeconds { get; set; }
        public int? SocketTimeoutSeconds { get; set; }
        public int? ServerSelectionTimeoutSeconds { get; set; }
        public bool? RetryWrites { get; set; }
        public bool? RetryReads { get; set; }
    }
}
