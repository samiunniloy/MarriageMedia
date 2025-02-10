namespace API.Rabbit
{
   
    public interface IRabbitMQService
    {
        Task SendImageForProcessingAsync(ProcessedImage processedImage);
        Task<List<ProcessedImage>> RequestStoredImagesAsync(int id);
        void Dispose();
    }
}
