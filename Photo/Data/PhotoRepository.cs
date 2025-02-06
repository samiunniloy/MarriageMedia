using MongoDB.Driver;
using Photo.Entity;
using Photo.Entity_and_Dto;// Assuming the IPhotoRepository interface is here
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Photo.Data
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly IMongoCollection<Picture> _picture; // Corrected field name

        public PhotoRepository(IMongoDatabase database)
        {
            _picture = database.GetCollection<Picture>("photos"); // Corrected collection name
        }

        public async Task<IEnumerable<Picture>> GetPhoto(string userName)
        {
            return await _picture.Find(picture => picture.UserName == userName).ToListAsync();
        }

        public async Task SavePhoto(Picture photo)
        {
            await _picture.InsertOneAsync(photo);
        }
    }
}
