using Photo.Entity;
using Photo.Entity_and_Dto;

namespace Photo.Data
{
    public interface IPhotoRepository
    {
        Task<IEnumerable<Picture>> GetPhoto(string userName);
        Task SavePhoto(Picture picture);
    }
}
