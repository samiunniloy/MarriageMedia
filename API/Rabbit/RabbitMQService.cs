using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace API.Rabbit
{
    public class RabbitMQService : IRabbitMQService, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _replyQueueName;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<List<ProcessedImage>>> _callbackMapper = new();
        private readonly IBasicProperties _props;
        private readonly EventingBasicConsumer _consumer;

        public RabbitMQService(IConnectionFactory connectionFactory)
        {
            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare("rpc_request_queue", false, false, false, null);
            _replyQueueName = _channel.QueueDeclare().QueueName;

            _props = _channel.CreateBasicProperties();
            _props.ReplyTo = _replyQueueName;

            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += (model, ea) =>
            {
                if (!_callbackMapper.TryRemove(ea.BasicProperties.CorrelationId, out var tcs))
                    return;

                var response = Encoding.UTF8.GetString(ea.Body.ToArray());
                var images = JsonSerializer.Deserialize<List<ProcessedImage>>(response);
                tcs.TrySetResult(images ?? new List<ProcessedImage>());
            };

            _channel.BasicConsume(consumer: _consumer,
                                queue: _replyQueueName,
                                autoAck: true);
        }

        public async Task SendImageForProcessingAsync(ProcessedImage processedImage)
        {
            var correlationId = Guid.NewGuid().ToString();
            var tcs = new TaskCompletionSource<List<ProcessedImage>>();
            _callbackMapper.TryAdd(correlationId, tcs);

            _props.CorrelationId = correlationId;
            var messageBytes = JsonSerializer.SerializeToUtf8Bytes(processedImage);

            _channel.BasicPublish(
                exchange: "",
                routingKey: "rpc_request_queue",
                basicProperties: _props,
                body: messageBytes);

            await tcs.Task;
        }

        public async Task<List<ProcessedImage>> RequestStoredImagesAsync(int id)
        {
            var correlationId = Guid.NewGuid().ToString();
            var tcs = new TaskCompletionSource<List<ProcessedImage>>();
            _callbackMapper.TryAdd(correlationId, tcs);

            _props.CorrelationId = correlationId;
            var message = new { id };
            var messageBytes = JsonSerializer.SerializeToUtf8Bytes(message);

            _channel.BasicPublish(
                exchange: "",
                routingKey: "rpc_request_queue",
                basicProperties: _props,
                body: messageBytes);

            return await tcs.Task;
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}