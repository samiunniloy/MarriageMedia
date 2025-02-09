namespace API.Rabbit
{
    public interface IRabbitMQService
    {
        void SendImageForProcessing(ProcessedImage processedImage);
        void RequestStoredImages();
        List<ProcessedImage> ReceiveProcessedImages();
    }
}
