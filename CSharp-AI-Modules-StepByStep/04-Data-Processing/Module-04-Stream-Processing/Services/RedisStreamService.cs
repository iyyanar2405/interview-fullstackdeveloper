using AI.StreamProcessing.Models;
using StackExchange.Redis;
using Newtonsoft.Json;

namespace AI.StreamProcessing.Services;

public interface IRedisStreamService
{
    Task<bool> AddToStreamAsync<T>(string streamKey, T message);
    Task<bool> AddBatchToStreamAsync<T>(string streamKey, List<T> messages);
    Task StartConsumerAsync(string streamKey, string consumerGroup, Func<RedisStreamMessage, Task<bool>> messageHandler, CancellationToken cancellationToken);
    Task<List<RedisStreamMessage>> ReadStreamAsync(string streamKey, int count = 100);
    Task<StreamMetrics> GetMetricsAsync();
}

public class RedisStreamService : IRedisStreamService
{
    private readonly RedisStreamConfig _config;
    private readonly ILogger<RedisStreamService> _logger;
    private readonly StreamMetrics _metrics;
    private IConnectionMultiplexer? _connection;

    public RedisStreamService(RedisStreamConfig config, ILogger<RedisStreamService> logger)
    {
        _config = config;
        _logger = logger;
        _metrics = new StreamMetrics { StreamName = "Redis Streams" };
    }

    private IDatabase GetDatabase()
    {
        if (_connection == null || !_connection.IsConnected)
        {
            _connection = ConnectionMultiplexer.Connect(_config.ConnectionString);
        }
        return _connection.GetDatabase();
    }

    public async Task<bool> AddToStreamAsync<T>(string streamKey, T message)
    {
        try
        {
            var db = GetDatabase();
            var serialized = JsonConvert.SerializeObject(message);

            var entries = new NameValueEntry[]
            {
                new NameValueEntry("data", serialized),
                new NameValueEntry("timestamp", DateTime.UtcNow.ToString("o")),
                new NameValueEntry("type", typeof(T).Name)
            };

            var messageId = await db.StreamAddAsync(streamKey, entries, maxLength: _config.MaxStreamLength);

            _logger.LogInformation($"Message added to stream {streamKey}: {messageId}");
            _metrics.MessagesProcessed++;
            _metrics.MessagesSucceeded++;

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error adding message to stream {streamKey}");
            _metrics.MessagesFailed++;
            return false;
        }
    }

    public async Task<bool> AddBatchToStreamAsync<T>(string streamKey, List<T> messages)
    {
        try
        {
            var db = GetDatabase();

            foreach (var message in messages)
            {
                var serialized = JsonConvert.SerializeObject(message);
                var entries = new NameValueEntry[]
                {
                    new NameValueEntry("data", serialized),
                    new NameValueEntry("timestamp", DateTime.UtcNow.ToString("o"))
                };

                await db.StreamAddAsync(streamKey, entries, maxLength: _config.MaxStreamLength);
            }

            _logger.LogInformation($"Batch of {messages.Count} messages added to stream {streamKey}");
            _metrics.MessagesProcessed += messages.Count;
            _metrics.MessagesSucceeded += messages.Count;

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error adding batch to stream {streamKey}");
            _metrics.MessagesFailed += messages.Count;
            return false;
        }
    }

    public async Task StartConsumerAsync(
        string streamKey,
        string consumerGroup,
        Func<RedisStreamMessage, Task<bool>> messageHandler,
        CancellationToken cancellationToken)
    {
        var db = GetDatabase();

        // Create consumer group if it doesn't exist
        try
        {
            await db.StreamCreateConsumerGroupAsync(streamKey, consumerGroup, StreamPosition.NewMessages);
            _logger.LogInformation($"Consumer group '{consumerGroup}' created for stream '{streamKey}'");
        }
        catch (RedisServerException ex) when (ex.Message.Contains("BUSYGROUP"))
        {
            _logger.LogInformation($"Consumer group '{consumerGroup}' already exists");
        }

        var consumerName = _config.ConsumerName;

        _logger.LogInformation($"Redis Stream consumer started: {streamKey}/{consumerGroup}/{consumerName}");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var messages = await db.StreamReadGroupAsync(
                    streamKey,
                    consumerGroup,
                    consumerName,
                    StreamPosition.NewMessages,
                    count: _config.BatchSize,
                    noAck: false);

                if (messages.Length == 0)
                {
                    await Task.Delay(_config.BlockTimeMs, cancellationToken);
                    continue;
                }

                foreach (var message in messages)
                {
                    var startTime = DateTime.UtcNow;

                    try
                    {
                        var values = new Dictionary<string, string>();
                        foreach (var entry in message.Values)
                        {
                            values[entry.Name.ToString()] = entry.Value.ToString();
                        }

                        var streamMessage = new RedisStreamMessage
                        {
                            Id = message.Id.ToString(),
                            Values = values,
                            Timestamp = DateTime.UtcNow
                        };

                        var success = await messageHandler(streamMessage);

                        if (success)
                        {
                            await db.StreamAcknowledgeAsync(streamKey, consumerGroup, message.Id);
                            _metrics.MessagesSucceeded++;
                            _logger.LogDebug($"Message {message.Id} acknowledged");
                        }
                        else
                        {
                            _metrics.MessagesFailed++;
                            _logger.LogWarning($"Message {message.Id} processing failed");
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
                        _logger.LogError(ex, $"Error processing message {message.Id}");
                        _metrics.MessagesFailed++;
                    }
                }
            }
            catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogError(ex, "Error in Redis Stream consumer loop");
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }

        _logger.LogInformation("Redis Stream consumer stopped");
    }

    public async Task<List<RedisStreamMessage>> ReadStreamAsync(string streamKey, int count = 100)
    {
        var messages = new List<RedisStreamMessage>();

        try
        {
            var db = GetDatabase();
            var streamMessages = await db.StreamReadAsync(streamKey, StreamPosition.Beginning, count);

            foreach (var message in streamMessages)
            {
                var values = new Dictionary<string, string>();
                foreach (var entry in message.Values)
                {
                    values[entry.Name.ToString()] = entry.Value.ToString();
                }

                messages.Add(new RedisStreamMessage
                {
                    Id = message.Id.ToString(),
                    Values = values,
                    Timestamp = DateTime.UtcNow
                });
            }

            _logger.LogInformation($"Read {messages.Count} messages from stream {streamKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error reading stream {streamKey}");
        }

        return messages;
    }

    public Task<StreamMetrics> GetMetricsAsync()
    {
        return Task.FromResult(_metrics);
    }
}
