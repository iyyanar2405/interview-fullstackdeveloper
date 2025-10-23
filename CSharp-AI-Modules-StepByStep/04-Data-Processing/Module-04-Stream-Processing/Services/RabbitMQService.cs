using AI.StreamProcessing.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using System.Text;

namespace AI.StreamProcessing.Services;

public interface IRabbitMQService
{
    Task<bool> PublishAsync<T>(string exchange, string routingKey, T message);
    Task<bool> PublishBatchAsync<T>(string exchange, string routingKey, List<T> messages);
    Task StartConsumerAsync(string queueName, Func<RabbitMQMessage, Task<bool>> messageHandler, CancellationToken cancellationToken);
    Task<StreamMetrics> GetMetricsAsync();
}

public class RabbitMQService : IRabbitMQService
{
    private readonly RabbitMQConfig _config;
    private readonly ILogger<RabbitMQService> _logger;
    private readonly StreamMetrics _metrics;
    private IConnection? _connection;
    private IModel? _channel;

    public RabbitMQService(RabbitMQConfig config, ILogger<RabbitMQService> logger)
    {
        _config = config;
        _logger = logger;
        _metrics = new StreamMetrics { StreamName = "RabbitMQ" };
    }

    private IModel GetChannel()
    {
        if (_connection == null || !_connection.IsOpen)
        {
            var factory = new ConnectionFactory
            {
                HostName = _config.HostName,
                Port = _config.Port,
                UserName = _config.UserName,
                Password = _config.Password,
                VirtualHost = _config.VirtualHost,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };

            _connection = factory.CreateConnection();
        }

        if (_channel == null || _channel.IsClosed)
        {
            _channel = _connection.CreateModel();

            // Declare exchange if specified
            if (!string.IsNullOrEmpty(_config.ExchangeName))
            {
                _channel.ExchangeDeclare(
                    exchange: _config.ExchangeName,
                    type: _config.ExchangeType,
                    durable: _config.Durable,
                    autoDelete: _config.AutoDelete);
            }

            // Declare queue if specified
            if (!string.IsNullOrEmpty(_config.QueueName))
            {
                _channel.QueueDeclare(
                    queue: _config.QueueName,
                    durable: _config.Durable,
                    exclusive: false,
                    autoDelete: _config.AutoDelete);

                // Bind queue to exchange if both specified
                if (!string.IsNullOrEmpty(_config.ExchangeName))
                {
                    _channel.QueueBind(
                        queue: _config.QueueName,
                        exchange: _config.ExchangeName,
                        routingKey: _config.RoutingKey);
                }
            }

            _channel.BasicQos(0, (ushort)_config.PrefetchCount, false);
        }

        return _channel;
    }

    public Task<bool> PublishAsync<T>(string exchange, string routingKey, T message)
    {
        try
        {
            var channel = GetChannel();

            var serialized = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(serialized);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.MessageId = Guid.NewGuid().ToString();
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            properties.ContentType = "application/json";
            properties.ContentEncoding = "utf-8";
            properties.DeliveryMode = 2; // Persistent

            channel.BasicPublish(
                exchange: exchange,
                routingKey: routingKey,
                basicProperties: properties,
                body: body);

            _logger.LogInformation($"Message published to {exchange}/{routingKey}");
            _metrics.MessagesProcessed++;
            _metrics.MessagesSucceeded++;

            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing message to RabbitMQ");
            _metrics.MessagesFailed++;
            return Task.FromResult(false);
        }
    }

    public Task<bool> PublishBatchAsync<T>(string exchange, string routingKey, List<T> messages)
    {
        try
        {
            var channel = GetChannel();
            var batch = channel.CreateBasicPublishBatch();

            foreach (var message in messages)
            {
                var serialized = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(serialized);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.MessageId = Guid.NewGuid().ToString();
                properties.ContentType = "application/json";

                batch.Add(exchange, routingKey, false, properties, new ReadOnlyMemory<byte>(body));
            }

            batch.Publish();

            _logger.LogInformation($"Batch of {messages.Count} messages published to {exchange}/{routingKey}");
            _metrics.MessagesProcessed += messages.Count;
            _metrics.MessagesSucceeded += messages.Count;

            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing batch to RabbitMQ");
            _metrics.MessagesFailed += messages.Count;
            return Task.FromResult(false);
        }
    }

    public Task StartConsumerAsync(
        string queueName,
        Func<RabbitMQMessage, Task<bool>> messageHandler,
        CancellationToken cancellationToken)
    {
        var channel = GetChannel();

        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += async (model, ea) =>
        {
            var startTime = DateTime.UtcNow;

            try
            {
                var message = new RabbitMQMessage
                {
                    MessageId = ea.BasicProperties.MessageId ?? Guid.NewGuid().ToString(),
                    Body = ea.Body.ToArray(),
                    ContentType = ea.BasicProperties.ContentType ?? "application/json",
                    ContentEncoding = ea.BasicProperties.ContentEncoding ?? "utf-8",
                    CorrelationId = ea.BasicProperties.CorrelationId ?? string.Empty,
                    ReplyTo = ea.BasicProperties.ReplyTo ?? string.Empty,
                    Timestamp = DateTime.UtcNow
                };

                if (ea.BasicProperties.Headers != null)
                {
                    foreach (var header in ea.BasicProperties.Headers)
                    {
                        message.Headers[header.Key] = header.Value;
                    }
                }

                var success = await messageHandler(message);

                if (success)
                {
                    channel.BasicAck(ea.DeliveryTag, false);
                    _metrics.MessagesSucceeded++;
                    _logger.LogDebug($"Message {message.MessageId} acknowledged");
                }
                else
                {
                    channel.BasicNack(ea.DeliveryTag, false, true);
                    _metrics.MessagesFailed++;
                    _logger.LogWarning($"Message {message.MessageId} rejected");
                }

                var processingTime = (DateTime.UtcNow - startTime).TotalMilliseconds;
                _metrics.AverageProcessingTimeMs = 
                    (_metrics.AverageProcessingTimeMs * _metrics.MessagesProcessed + processingTime) 
                    / (_metrics.MessagesProcessed + 1);
                _metrics.MessagesProcessed++;
                _metrics.LastProcessedTime = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing RabbitMQ message");
                channel.BasicNack(ea.DeliveryTag, false, false);
                _metrics.MessagesFailed++;
            }
        };

        channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
        _logger.LogInformation($"RabbitMQ consumer started for queue: {queueName}");

        // Keep the consumer running until cancellation
        var tcs = new TaskCompletionSource<bool>();
        cancellationToken.Register(() => tcs.SetResult(true));

        return tcs.Task;
    }

    public Task<StreamMetrics> GetMetricsAsync()
    {
        return Task.FromResult(_metrics);
    }

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}
