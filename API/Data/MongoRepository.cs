using MongoDB.Driver;
using System.Linq.Expressions;
using API.Helpers;

namespace API.Data.Repositories
{
    public interface IMongoRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetByIdAsync(int id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> filter);
        Task<PagedListM<TEntity>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<TEntity, bool>> filter = null);
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(int id);
    }

    public class MongoRepository<TEntity> : IMongoRepository<TEntity> where TEntity : class
    {
        protected readonly IMongoCollection<TEntity> _collection;

        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            _collection = database.GetCollection<TEntity>(collectionName);
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            var filter = Builders<TEntity>.Filter.Eq("Id", id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<PagedListM<TEntity>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<TEntity, bool>> filter = null)
        {
            var query = filter != null ? _collection.Find(filter) : _collection.Find(_ => true);

            var totalCount = await query.CountDocumentsAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return new PagedListM<TEntity>(items, (int)totalCount, pageNumber, pageSize);
        }

        public async Task AddAsync(TEntity entity)
        {
            // Get max ID and increment for new entity
            var maxId = await _collection.Find(_ => true)
                .SortByDescending(e => GetId(e))
                .Limit(1)
                .Project(e => GetId(e))
                .FirstOrDefaultAsync();

            SetId(entity, maxId + 1);
            await _collection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(TEntity entity)
        {
            var id = GetId(entity);
            var filter = Builders<TEntity>.Filter.Eq("Id", id);
            await _collection.ReplaceOneAsync(filter, entity);
        }

        public async Task DeleteAsync(int id)
        {
            var filter = Builders<TEntity>.Filter.Eq("Id", id);
            await _collection.DeleteOneAsync(filter);
        }

        // Helper methods to get/set Id using reflection
        private int GetId(TEntity entity)
        {
            var property = typeof(TEntity).GetProperty("Id");
            return (int)property?.GetValue(entity);
        }

        private void SetId(TEntity entity, int id)
        {
            var property = typeof(TEntity).GetProperty("Id");
            property?.SetValue(entity, id);
        }
    }
}