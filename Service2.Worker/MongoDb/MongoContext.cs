using MongoDB.Driver;

namespace Service2.Worker.MongoDb
{
    public class MongoDbConfig
    {
        public string ConnectionString { get; set; } = "mongodb://localhost:27017";
        public string DatabaseName { get; set; } = "MarriageMedia";
    }

    public interface IMongoDbContext
    {
        IMongoCollection<T> GetCollection<T>(string name);
    }

    public class MongoDbContext : IMongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(MongoDbConfig config)
        {
            var client = new MongoClient(config.ConnectionString);
            _database = client.GetDatabase(config.DatabaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }
    }
}
