using MongoDB.Driver;

namespace Service2.Worker.MongoDb
{
    public interface IProcessedImageRepository : IRepository<ProcessedImage>
    {
        Task<IEnumerable<ProcessedImage>> GetByUserIdAsync(int userId);
    }

    public class ProcessedImageRepository : MongoRepository<ProcessedImage>, IProcessedImageRepository
    {
        public ProcessedImageRepository(IMongoClient mongoClient, string databaseName)
            : base(mongoClient, databaseName, "ProcessedImages")
        {
        }

        public async Task<IEnumerable<ProcessedImage>> GetByUserIdAsync(int userId)
        {
            return await FindAsync(x => x.UserId == userId);
        }
    }
}
