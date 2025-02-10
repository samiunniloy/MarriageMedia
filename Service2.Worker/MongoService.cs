using MongoDB.Driver;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service2.Worker
{
    public class MongoDBService : IMongoService
    {
        private readonly IMongoCollection<ProcessedImage> _images;

        public MongoDBService()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("MarriageMedia");
            _images = database.GetCollection<ProcessedImage>("ProcessedImages");
        }

        public async Task SaveImageAsync(ProcessedImage image)
        {
            await _images.InsertOneAsync(image);
        }

        public async Task<List<ProcessedImage>> GetAllImagesAsync(int id)
        {
            return await _images.Find(img=>img.UserId==id).ToListAsync();
        }
    }
}
