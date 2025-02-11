using MongoDB.Driver;
using System.Linq.Expressions;

namespace Service2.Worker.MongoDb
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(string id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter);
        Task InsertOneAsync(T document);
        Task InsertManyAsync(ICollection<T> documents);
        Task UpdateOneAsync(string id, T document);
        Task DeleteOneAsync(string id);
        Task DeleteManyAsync(Expression<Func<T, bool>> filter);
    }

    public class MongoRepository<T> : IRepository<T> where T : class
    {
        private readonly IMongoCollection<T> _collection;

        public MongoRepository(IMongoClient mongoClient, string databaseName, string collectionName)
        {
            var database = mongoClient.GetDatabase(databaseName);
            _collection = database.GetCollection<T>(collectionName);
        }

        public async Task<T> GetByIdAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter)
        {
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task InsertOneAsync(T document)
        {
            await _collection.InsertOneAsync(document);
        }

        public async Task InsertManyAsync(ICollection<T> documents)
        {
            await _collection.InsertManyAsync(documents);
        }

        public async Task UpdateOneAsync(string id, T document)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            await _collection.ReplaceOneAsync(filter, document);
        }

        public async Task DeleteOneAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            await _collection.DeleteOneAsync(filter);
        }

        public async Task DeleteManyAsync(Expression<Func<T, bool>> filter)
        {
            await _collection.DeleteManyAsync(filter);
        }
    }
}
