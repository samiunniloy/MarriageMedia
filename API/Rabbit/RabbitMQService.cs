using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Text;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using IConnectionFactory = RabbitMQ.Client.IConnectionFactory;
namespace API.Rabbit
{
    public class RabbitMQService : IRabbitMQService, IDisposable
    {
        private readonly IConnection _connection;
        private readonly RabbitMQ.Client.IModel _channel;
        private const string RequestQueue = "request_queue";
        private const string FetchQueue = "fetch_queue";
        private const string ResponseQueue = "response_queue";

        //public RabbitMQService(IConnectionFactory connectionFactory)
        //{
        //    _connection = connectionFactory.CreateConnection();
        //    _channel = _connection.CreateModel();

        //    _channel.QueueDeclare(RequestQueue, durable: true, exclusive: false, autoDelete: false);
        //    _channel.QueueDeclare(FetchQueue, durable: true, exclusive: false, autoDelete: false);
        //    _channel.QueueDeclare(ResponseQueue, durable: true, exclusive: false, autoDelete: false);
        //}

        public RabbitMQService(IConnectionFactory connectionFactory)
        {
            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();

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




        public void RequestStoredImages()
        {
            var message = new { Request = "FetchAll" };
            var body = JsonSerializer.SerializeToUtf8Bytes(message);
            _channel.BasicPublish("", FetchQueue, null, body);
        }

        public List<ProcessedImage> ReceiveProcessedImages()
        {
            var result = new List<ProcessedImage>();
            var consumer = new EventingBasicConsumer(_channel);

            var received = false;
            consumer.Received += (model, ea) =>
            {
                var response = Encoding.UTF8.GetString(ea.Body.ToArray());
                var processedImage = JsonSerializer.Deserialize<List<ProcessedImage>>(response);
                result = (processedImage);
            };

            _channel.BasicConsume("response_queue", true, consumer);


            var timeout = DateTime.Now.AddSeconds(10);
            while (!received && DateTime.Now < timeout)
            {
                Thread.Sleep(100);
            }

            return result;
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }

        void IRabbitMQService.SendImageForProcessing(ProcessedImage processedImage)
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(processedImage));

            _channel.BasicPublish("", RequestQueue, null, body);

        }
    }

}
