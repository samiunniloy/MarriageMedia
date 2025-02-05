using Microsoft.Extensions.Options;
using MongoDB.Driver;
using API.Entities;
using API.Helpers;

namespace API.Data
{
    public class MongoDBContext
    {
        private readonly IMongoDatabase _database;

        public MongoDBContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<Message> Messages => _database.GetCollection<Message>("Messages");
        public IMongoCollection<Group> Groups => _database.GetCollection<Group>("Groups");
        public IMongoCollection<Connection> Connections => _database.GetCollection<Connection>("Connections");
    }

    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}