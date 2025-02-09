
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service2.Worker
{
    public interface IMongoService
    {
        Task SaveImageAsync(ProcessedImage image);
        Task<List<ProcessedImage>> GetAllImagesAsync();
    }
}
