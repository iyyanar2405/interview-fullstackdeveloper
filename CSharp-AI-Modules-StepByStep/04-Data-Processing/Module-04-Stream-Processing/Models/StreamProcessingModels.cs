namespace AI.StreamProcessing.Models;

// Enums
public enum StreamPlatform
{
    Kafka,
    AzureServiceBus,
    AzureEventHubs,
    RabbitMQ,
    RedisStreams,
    InMemory
}

public enum MessagePriority
{
    Low,
    Normal,
    High,
    Critical
}

public enum ProcessingMode
{
    RealTime,
    Batch,
    MicroBatch,
    Windowed
}

public enum DeliveryGuarantee
{
    AtMostOnce,
    AtLeastOnce,
    ExactlyOnce
}

public enum SerializationFormat
{
    Json,
    MessagePack,
    Protobuf,
    Avro,
    Xml
}

// Stream Message Models
public class StreamMessage<T>
{
    public string MessageId { get; set; } = Guid.NewGuid().ToString();
    public string CorrelationId { get; set; } = string.Empty;
    public T? Payload { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public MessagePriority Priority { get; set; } = MessagePriority.Normal;
    public string Source { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public int RetryCount { get; set; } = 0;
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class MessageEnvelope
{
    public string MessageId { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string Partition { get; set; } = string.Empty;
    public long Offset { get; set; }
    public byte[] Body { get; set; } = Array.Empty<byte>();
    public Dictionary<string, string> Headers { get; set; } = new();
    public DateTime Timestamp { get; set; }
}

// Kafka Models
public class KafkaProducerConfig
{
    public string BootstrapServers { get; set; } = "localhost:9092";
    public string Topic { get; set; } = string.Empty;
    public int MessageTimeoutMs { get; set; } = 10000;
    public string Acks { get; set; } = "all";
    public bool EnableIdempotence { get; set; } = true;
    public int Retries { get; set; } = 3;
    public string CompressionType { get; set; } = "snappy";
    public int BatchSize { get; set; } = 16384;
    public int LingerMs { get; set; } = 10;
    public Dictionary<string, string> AdditionalConfig { get; set; } = new();
}

public class KafkaConsumerConfig
{
    public string BootstrapServers { get; set; } = "localhost:9092";
    public string GroupId { get; set; } = "default-group";
    public List<string> Topics { get; set; } = new();
    public string AutoOffsetReset { get; set; } = "earliest";
    public bool EnableAutoCommit { get; set; } = false;
    public int SessionTimeoutMs { get; set; } = 10000;
    public int MaxPollIntervalMs { get; set; } = 300000;
    public int MaxPollRecords { get; set; } = 500;
    public Dictionary<string, string> AdditionalConfig { get; set; } = new();
}

public class KafkaMessage
{
    public string Topic { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int Partition { get; set; }
    public long Offset { get; set; }
    public DateTime Timestamp { get; set; }
    public Dictionary<string, byte[]> Headers { get; set; } = new();
}

// Azure Service Bus Models
public class ServiceBusConfig
{
    public string ConnectionString { get; set; } = string.Empty;
    public string QueueName { get; set; } = string.Empty;
    public string TopicName { get; set; } = string.Empty;
    public string SubscriptionName { get; set; } = string.Empty;
    public int MaxConcurrentCalls { get; set; } = 10;
    public int PrefetchCount { get; set; } = 0;
    public TimeSpan MaxAutoLockRenewalDuration { get; set; } = TimeSpan.FromMinutes(5);
    public bool EnableAutoComplete { get; set; } = false;
}

public class ServiceBusMessage
{
    public string MessageId { get; set; } = Guid.NewGuid().ToString();
    public string CorrelationId { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public Dictionary<string, object> ApplicationProperties { get; set; } = new();
    public TimeSpan? TimeToLive { get; set; }
    public DateTime ScheduledEnqueueTime { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public string ReplyTo { get; set; } = string.Empty;
}

// Azure Event Hubs Models
public class EventHubConfig
{
    public string ConnectionString { get; set; } = string.Empty;
    public string EventHubName { get; set; } = string.Empty;
    public string ConsumerGroup { get; set; } = "$Default";
    public string BlobStorageConnectionString { get; set; } = string.Empty;
    public string BlobContainerName { get; set; } = "checkpoints";
    public int MaxBatchSize { get; set; } = 100;
    public TimeSpan MaxWaitTime { get; set; } = TimeSpan.FromSeconds(60);
}

public class EventData
{
    public string EventId { get; set; } = Guid.NewGuid().ToString();
    public byte[] Body { get; set; } = Array.Empty<byte>();
    public Dictionary<string, object> Properties { get; set; } = new();
    public Dictionary<string, object> SystemProperties { get; set; } = new();
    public long SequenceNumber { get; set; }
    public long Offset { get; set; }
    public string PartitionKey { get; set; } = string.Empty;
    public DateTime EnqueuedTime { get; set; }
}

// RabbitMQ Models
public class RabbitMQConfig
{
    public string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";
    public string ExchangeName { get; set; } = string.Empty;
    public string QueueName { get; set; } = string.Empty;
    public string RoutingKey { get; set; } = string.Empty;
    public string ExchangeType { get; set; } = "direct";
    public bool Durable { get; set; } = true;
    public bool AutoDelete { get; set; } = false;
    public int PrefetchCount { get; set; } = 10;
}

public class RabbitMQMessage
{
    public string MessageId { get; set; } = Guid.NewGuid().ToString();
    public byte[] Body { get; set; } = Array.Empty<byte>();
    public Dictionary<string, object> Headers { get; set; } = new();
    public string ContentType { get; set; } = "application/json";
    public string ContentEncoding { get; set; } = "utf-8";
    public byte DeliveryMode { get; set; } = 2; // Persistent
    public byte Priority { get; set; } = 0;
    public string CorrelationId { get; set; } = string.Empty;
    public string ReplyTo { get; set; } = string.Empty;
    public string Expiration { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

// Redis Streams Models
public class RedisStreamConfig
{
    public string ConnectionString { get; set; } = "localhost:6379";
    public string StreamKey { get; set; } = string.Empty;
    public string ConsumerGroup { get; set; } = "default-group";
    public string ConsumerName { get; set; } = Environment.MachineName;
    public int MaxStreamLength { get; set; } = 10000;
    public int BlockTimeMs { get; set; } = 5000;
    public int BatchSize { get; set; } = 100;
}

public class RedisStreamMessage
{
    public string Id { get; set; } = string.Empty;
    public Dictionary<string, string> Values { get; set; } = new();
    public DateTime Timestamp { get; set; }
}

// Stream Processing Models
public class StreamProcessingConfig
{
    public StreamPlatform Platform { get; set; }
    public ProcessingMode Mode { get; set; }
    public DeliveryGuarantee DeliveryGuarantee { get; set; }
    public SerializationFormat SerializationFormat { get; set; } = SerializationFormat.Json;
    public int MaxRetries { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(5);
    public bool EnableDeadLetterQueue { get; set; } = true;
    public int MaxConcurrency { get; set; } = 10;
    public TimeSpan ProcessingTimeout { get; set; } = TimeSpan.FromMinutes(5);
}

public class ProcessingResult
{
    public bool Success { get; set; }
    public string MessageId { get; set; } = string.Empty;
    public TimeSpan ProcessingTime { get; set; }
    public string? Error { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

// Windowing Models
public class WindowConfig
{
    public TimeSpan WindowSize { get; set; } = TimeSpan.FromMinutes(5);
    public TimeSpan SlideInterval { get; set; } = TimeSpan.FromMinutes(1);
    public WindowType Type { get; set; } = WindowType.Tumbling;
    public int MaxEventsPerWindow { get; set; } = 1000;
}

public enum WindowType
{
    Tumbling,
    Sliding,
    Session,
    Count
}

public class WindowedResult<T>
{
    public DateTime WindowStart { get; set; }
    public DateTime WindowEnd { get; set; }
    public List<T> Events { get; set; } = new();
    public int EventCount { get; set; }
    public Dictionary<string, object> Aggregates { get; set; } = new();
}

// Event Sourcing Models
public class Event
{
    public string EventId { get; set; } = Guid.NewGuid().ToString();
    public string EventType { get; set; } = string.Empty;
    public string AggregateId { get; set; } = string.Empty;
    public string AggregateType { get; set; } = string.Empty;
    public long Version { get; set; }
    public string Data { get; set; } = string.Empty;
    public string Metadata { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string UserId { get; set; } = string.Empty;
}

public class EventStream
{
    public string StreamId { get; set; } = string.Empty;
    public string AggregateId { get; set; } = string.Empty;
    public List<Event> Events { get; set; } = new();
    public long CurrentVersion { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

// Metrics Models
public class StreamMetrics
{
    public string StreamName { get; set; } = string.Empty;
    public long MessagesProcessed { get; set; }
    public long MessagesSucceeded { get; set; }
    public long MessagesFailed { get; set; }
    public double AverageProcessingTimeMs { get; set; }
    public double MessagesPerSecond { get; set; }
    public long CurrentLag { get; set; }
    public DateTime LastProcessedTime { get; set; }
    public Dictionary<string, long> ErrorCounts { get; set; } = new();
}

// Dead Letter Queue Models
public class DeadLetterMessage
{
    public string OriginalMessageId { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public byte[] Body { get; set; } = Array.Empty<byte>();
    public Dictionary<string, string> Headers { get; set; } = new();
    public string ErrorReason { get; set; } = string.Empty;
    public string StackTrace { get; set; } = string.Empty;
    public int FailureCount { get; set; }
    public DateTime FirstFailureTime { get; set; }
    public DateTime LastFailureTime { get; set; }
    public DateTime EnqueuedTime { get; set; } = DateTime.UtcNow;
}

// API Response Model
public class ApiResponseModel<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Error { get; set; }
    public List<string> Warnings { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponseModel<T> SuccessResponse(T data)
    {
        return new ApiResponseModel<T>
        {
            Success = true,
            Data = data
        };
    }

    public static ApiResponseModel<T> ErrorResponse(string error)
    {
        return new ApiResponseModel<T>
        {
            Success = false,
            Error = error
        };
    }
}
