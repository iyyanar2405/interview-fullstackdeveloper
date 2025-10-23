using AI.StreamProcessing.Models;
using Confluent.Kafka;
using System.Text;
using Newtonsoft.Json;

namespace AI.StreamProcessing.Services;

public interface IKafkaService
{
    Task<bool> ProduceAsync<T>(string topic, T message, string? key = null);
    Task<bool> ProduceBatchAsync<T>(string topic, List<T> messages);
    Task StartConsumerAsync(List<string> topics, Func<KafkaMessage, Task<bool>> messageHandler, CancellationToken cancellationToken);
    Task<StreamMetrics> GetMetricsAsync(string topic);
}

public class KafkaService : IKafkaService
{
    private readonly KafkaProducerConfig _producerConfig;
    private readonly KafkaConsumerConfig _consumerConfig;
    private readonly ILogger<KafkaService> _logger;
    private readonly StreamMetrics _metrics;

    public KafkaService(
        KafkaProducerConfig producerConfig,
        KafkaConsumerConfig consumerConfig,
        ILogger<KafkaService> logger)
    {
        _producerConfig = producerConfig;
        _consumerConfig = consumerConfig;
        _logger = logger;
        _metrics = new StreamMetrics { StreamName = "Kafka" };
    }

    public async Task<bool> ProduceAsync<T>(string topic, T message, string? key = null)
    {
        try
        {
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = _producerConfig.BootstrapServers,
                Acks = _producerConfig.Acks == "all" ? Acks.All : Acks.Leader,
                EnableIdempotence = _producerConfig.EnableIdempotence,
                MessageTimeoutMs = _producerConfig.MessageTimeoutMs,
                Retries = _producerConfig.Retries,
                CompressionType = _producerConfig.CompressionType == "snappy" 
                    ? CompressionType.Snappy 
                    : CompressionType.None
            };

            using var producer = new ProducerBuilder<string, string>(producerConfig).Build();

            var serializedMessage = JsonConvert.SerializeObject(message);
            var kafkaMessage = new Message<string, string>
            {
                Key = key ?? Guid.NewGuid().ToString(),
                Value = serializedMessage,
                Timestamp = new Timestamp(DateTime.UtcNow)
            };

            var result = await producer.ProduceAsync(topic, kafkaMessage);

            _logger.LogInformation($"Message produced to {result.Topic} [{result.Partition}] @ {result.Offset}");
            _metrics.MessagesProcessed++;
            _metrics.MessagesSucceeded++;

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error producing message to Kafka");
            _metrics.MessagesFailed++;
            return false;
        }
    }

    public async Task<bool> ProduceBatchAsync<T>(string topic, List<T> messages)
    {
        try
        {
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = _producerConfig.BootstrapServers,
                Acks = Acks.All,
                EnableIdempotence = true,
                BatchSize = _producerConfig.BatchSize,
                LingerMs = _producerConfig.LingerMs
            };

            using var producer = new ProducerBuilder<string, string>(producerConfig).Build();

            var tasks = messages.Select(async message =>
            {
                var serialized = JsonConvert.SerializeObject(message);
                var kafkaMessage = new Message<string, string>
                {
                    Key = Guid.NewGuid().ToString(),
                    Value = serialized
                };
                return await producer.ProduceAsync(topic, kafkaMessage);
            });

            await Task.WhenAll(tasks);
            producer.Flush(TimeSpan.FromSeconds(10));

            _logger.LogInformation($"Batch of {messages.Count} messages produced to {topic}");
            _metrics.MessagesProcessed += messages.Count;
            _metrics.MessagesSucceeded += messages.Count;

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error producing batch to Kafka");
            _metrics.MessagesFailed += messages.Count;
            return false;
        }
    }

    public async Task StartConsumerAsync(
        List<string> topics,
        Func<KafkaMessage, Task<bool>> messageHandler,
        CancellationToken cancellationToken)
    {
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _consumerConfig.BootstrapServers,
            GroupId = _consumerConfig.GroupId,
            AutoOffsetReset = _consumerConfig.AutoOffsetReset == "earliest" 
                ? AutoOffsetReset.Earliest 
                : AutoOffsetReset.Latest,
            EnableAutoCommit = _consumerConfig.EnableAutoCommit,
            SessionTimeoutMs = _consumerConfig.SessionTimeoutMs,
            MaxPollIntervalMs = _consumerConfig.MaxPollIntervalMs
        };

        using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        consumer.Subscribe(topics);

        _logger.LogInformation($"Kafka consumer started for topics: {string.Join(", ", topics)}");

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var consumeResult = consumer.Consume(cancellationToken);

                if (consumeResult?.Message == null)
                    continue;

                var startTime = DateTime.UtcNow;

                var kafkaMessage = new KafkaMessage
                {
                    Topic = consumeResult.Topic,
                    Key = consumeResult.Message.Key,
                    Value = consumeResult.Message.Value,
                    Partition = consumeResult.Partition.Value,
                    Offset = consumeResult.Offset.Value,
                    Timestamp = consumeResult.Message.Timestamp.UtcDateTime
                };

                try
                {
                    var success = await messageHandler(kafkaMessage);

                    if (success)
                    {
                        consumer.Commit(consumeResult);
                        _metrics.MessagesSucceeded++;
                        _logger.LogDebug($"Message processed successfully from {consumeResult.Topic}");
                    }
                    else
                    {
                        _metrics.MessagesFailed++;
                        _logger.LogWarning($"Message processing returned false for {consumeResult.Topic}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing message from {consumeResult.Topic}");
                    _metrics.MessagesFailed++;
                }

                var processingTime = (DateTime.UtcNow - startTime).TotalMilliseconds;
                _metrics.AverageProcessingTimeMs = 
                    (_metrics.AverageProcessingTimeMs * _metrics.MessagesProcessed + processingTime) 
                    / (_metrics.MessagesProcessed + 1);
                _metrics.MessagesProcessed++;
                _metrics.LastProcessedTime = DateTime.UtcNow;
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Kafka consumer cancelled");
        }
        finally
        {
            consumer.Close();
        }
    }

    public Task<StreamMetrics> GetMetricsAsync(string topic)
    {
        _metrics.StreamName = topic;
        return Task.FromResult(_metrics);
    }
}
