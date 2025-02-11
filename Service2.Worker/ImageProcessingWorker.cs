using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Service2.Worker.MongoDb;
using System.Text;
using System.Text.Json;

namespace Service2.Worker
{
    public class ImageProcessingWorker : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IProcessedImageRepository _imageRepository;
        public ImageProcessingWorker(IConnectionFactory connectionFactory, IProcessedImageRepository imageRepository)
        {
            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _imageRepository = imageRepository;

            _channel.QueueDeclare("rpc_request_queue", false, false, false, null);
            _channel.BasicQos(0, 1, false);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                string response = string.Empty;
                var body = ea.Body.ToArray();
                var props = ea.BasicProperties;
                var replyProps = _channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                try
                {
                    var message = Encoding.UTF8.GetString(body);

                    var processedImage = JsonSerializer.Deserialize<ProcessedImage>(message);
                    if (processedImage.Base64Image != null)
                    {
                        await _imageRepository.InsertOneAsync(processedImage);
                        response = JsonSerializer.Serialize(new List<ProcessedImage>());
                    }
                    else
                    {
                        var fetchRequest = JsonSerializer.Deserialize<Dictionary<string, int>>(message);
                        if (fetchRequest != null && fetchRequest.TryGetValue("id", out int userId))
                        {
                            var images = await _imageRepository.GetByUserIdAsync(userId);
                            response = JsonSerializer.Serialize(images);
                        }
                    }
                }
                catch (Exception ex)
                {
                    response = JsonSerializer.Serialize(new List<ProcessedImage>());
                    Console.WriteLine($"Error processing request: {ex.Message}");
                }
                finally
                {
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    _channel.BasicPublish(
                        exchange: "",
                        routingKey: props.ReplyTo,
                        basicProperties: replyProps,
                        body: responseBytes);

                    _channel.BasicAck(ea.DeliveryTag, false);
                }
            };

            _channel.BasicConsume(
                queue: "rpc_request_queue",
                autoAck: false,
                consumer: consumer);

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