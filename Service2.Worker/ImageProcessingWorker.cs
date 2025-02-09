
using RabbitMQ.Client.Events;
using RabbitMQ.Client;

using System.Text.Json;
using System.Text;

namespace Service2.Worker
{
   

    public class ImageProcessingWorker : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IMongoService _mongoDBService;

        public ImageProcessingWorker(IConnectionFactory connectionFactory, IMongoService mongoDBService)
        {
            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _mongoDBService = mongoDBService;

            _channel.QueueDeclare(
                queue: "request_queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _channel.QueueDeclare(
                queue: "fetch_queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _channel.QueueDeclare(
                queue: "response_queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var requestConsumer = new EventingBasicConsumer(_channel);
            var fetchConsumer = new EventingBasicConsumer(_channel);

            requestConsumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var messageJson = Encoding.UTF8.GetString(body);
                var processedImage = JsonSerializer.Deserialize<ProcessedImage>(messageJson);

                try
                {
                    if (processedImage != null)
                    {
                        await _mongoDBService.SaveImageAsync(processedImage);
                    }
                    else
                    {
                        Console.WriteLine("Failed to deserialize ProcessedImage.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing image: {ex.Message}");
                }
            };

            fetchConsumer.Received += async (model, ea) =>
            {
                var images = await _mongoDBService.GetAllImagesAsync();
                var responseBody = JsonSerializer.SerializeToUtf8Bytes(images);

                _channel.BasicPublish("", "response_queue", null, responseBody);
            };

            _channel.BasicConsume("request_queue", true, requestConsumer);
            _channel.BasicConsume("fetch_queue", true, fetchConsumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }



        public override void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }
    }

}
