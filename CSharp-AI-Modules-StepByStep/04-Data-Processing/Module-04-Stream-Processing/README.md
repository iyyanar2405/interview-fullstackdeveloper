# Stream Processing Module

## Overview

The **Stream Processing Module** provides comprehensive real-time data processing capabilities with support for multiple streaming platforms including Apache Kafka, Azure Service Bus, Azure Event Hubs, RabbitMQ, and Redis Streams.

### Supported Platforms

- **Apache Kafka**: Distributed streaming platform with high throughput
- **Azure Service Bus**: Enterprise messaging service with queues and topics
- **Azure Event Hubs**: Big data streaming platform
- **RabbitMQ**: Message broker with flexible routing
- **Redis Streams**: Lightweight stream processing with Redis

---

## Architecture

```
AI.StreamProcessing/
├── Models/
│   └── StreamProcessingModels.cs   # Comprehensive models for all platforms
├── Services/
│   ├── KafkaService.cs             # Apache Kafka integration
│   ├── ServiceBusService.cs        # Azure Service Bus integration
│   ├── EventHubService.cs          # Azure Event Hubs integration
│   ├── RabbitMQService.cs          # RabbitMQ integration
│   ├── RedisStreamService.cs       # Redis Streams integration
│   └── StreamProcessingService.cs  # Core processing logic
├── Controllers/
│   └── StreamController.cs         # REST API endpoints
├── Extensions/
│   └── ServiceCollectionExtensions.cs
└── Program.cs
```

---

## Quick Start

### 1. Installation

```bash
dotnet add package Confluent.Kafka
dotnet add package Azure.Messaging.ServiceBus
dotnet add package Azure.Messaging.EventHubs
dotnet add package RabbitMQ.Client
dotnet add package StackExchange.Redis
dotnet add package Serilog.AspNetCore
```

### 2. Configuration

Add to `appsettings.json`:

```json
{
  "Kafka": {
    "Producer": {
      "BootstrapServers": "localhost:9092",
      "Acks": "all",
      "EnableIdempotence": true
    },
    "Consumer": {
      "BootstrapServers": "localhost:9092",
      "GroupId": "my-consumer-group",
      "AutoOffsetReset": "earliest"
    }
  },
  "ServiceBus": {
    "ConnectionString": "Endpoint=sb://...",
    "QueueName": "my-queue"
  }
}
```

### 3. Register Services

```csharp
builder.Services.AddStreamProcessingServices(builder.Configuration);
```

### 4. Run the Application

```bash
dotnet run
```

Access Swagger UI: `https://localhost:5001/swagger`

---

## Apache Kafka

### Producer - Send Single Message

**Endpoint**: `POST /api/stream/kafka/produce`

```csharp
var request = new KafkaProduceRequest
{
    Topic = "user-events",
    Message = new { UserId = 123, Action = "Login", Timestamp = DateTime.UtcNow },
    Key = "user-123"
};

var result = await _kafkaService.ProduceAsync("user-events", request.Message, "user-123");
```

**cURL Example**:
```bash
curl -X POST "https://localhost:5001/api/stream/kafka/produce" \
  -H "Content-Type: application/json" \
  -d '{
    "topic": "user-events",
    "message": {"userId": 123, "action": "Login"},
    "key": "user-123"
  }'
```

### Producer - Send Batch

**Endpoint**: `POST /api/stream/kafka/produce-batch`

```csharp
var messages = new List<object>
{
    new { UserId = 1, Action = "Login" },
    new { UserId = 2, Action = "Purchase" },
    new { UserId = 3, Action = "Logout" }
};

var result = await _kafkaService.ProduceBatchAsync("user-events", messages);
```

### Consumer - Process Messages

```csharp
var topics = new List<string> { "user-events", "order-events" };

await _kafkaService.StartConsumerAsync(topics, async (message) =>
{
    Console.WriteLine($"Received: {message.Value}");
    
    // Process the message
    var userData = JsonConvert.DeserializeObject<UserEvent>(message.Value);
    await ProcessUserEvent(userData);
    
    return true; // Acknowledge success
}, cancellationToken);
```

### Kafka Configuration

```json
{
  "Kafka": {
    "Producer": {
      "BootstrapServers": "localhost:9092",
      "Acks": "all",
      "EnableIdempotence": true,
      "Retries": 3,
      "CompressionType": "snappy",
      "BatchSize": 16384,
      "LingerMs": 10
    },
    "Consumer": {
      "BootstrapServers": "localhost:9092",
      "GroupId": "my-consumer-group",
      "AutoOffsetReset": "earliest",
      "EnableAutoCommit": false,
      "MaxPollRecords": 500
    }
  }
}
```

---

## Azure Service Bus

### Send to Queue

**Endpoint**: `POST /api/stream/servicebus/send`

```csharp
var request = new ServiceBusSendRequest
{
    QueueOrTopic = "order-queue",
    Message = new { OrderId = 456, Status = "Pending" }
};

var result = await _serviceBusService.SendMessageAsync("order-queue", request.Message);
```

### Send to Topic

```csharp
var message = new { OrderId = 789, Status = "Completed" };
await _serviceBusService.SendMessageAsync("order-topic", message);
```

### Queue Processor

```csharp
await _serviceBusService.StartQueueProcessorAsync(
    "order-queue",
    async (message) =>
    {
        var order = JsonConvert.DeserializeObject<Order>(message.Body);
        await ProcessOrder(order);
        return true;
    },
    cancellationToken);
```

### Topic Processor with Subscription

```csharp
await _serviceBusService.StartTopicProcessorAsync(
    "order-topic",
    "processing-subscription",
    async (message) =>
    {
        await ProcessOrderUpdate(message);
        return true;
    },
    cancellationToken);
```

### Service Bus Configuration

```json
{
  "ServiceBus": {
    "ConnectionString": "Endpoint=sb://myservicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=...",
    "QueueName": "order-queue",
    "TopicName": "order-topic",
    "SubscriptionName": "processing-subscription",
    "MaxConcurrentCalls": 10,
    "PrefetchCount": 0,
    "EnableAutoComplete": false
  }
}
```

---

## Azure Event Hubs

### Send Event

**Endpoint**: `POST /api/stream/eventhub/send`

```csharp
var eventData = new
{
    SensorId = "sensor-001",
    Temperature = 25.5,
    Humidity = 60.2,
    Timestamp = DateTime.UtcNow
};

var result = await _eventHubService.SendEventAsync(eventData, partitionKey: "sensor-001");
```

### Send Batch

**Endpoint**: `POST /api/stream/eventhub/send-batch`

```csharp
var events = new List<object>
{
    new { SensorId = "sensor-001", Temperature = 25.5 },
    new { SensorId = "sensor-002", Temperature = 26.1 },
    new { SensorId = "sensor-003", Temperature = 24.8 }
};

await _eventHubService.SendBatchAsync(events);
```

### Event Processor

```csharp
await _eventHubService.StartProcessorAsync(async (eventData) =>
{
    var sensorData = JsonConvert.DeserializeObject<SensorReading>(
        Encoding.UTF8.GetString(eventData.Body));
    
    await ProcessSensorData(sensorData);
    return true;
}, cancellationToken);
```

### Read Events from Partition

**Endpoint**: `GET /api/stream/eventhub/read/{partitionId}`

```csharp
var events = await _eventHubService.ReadEventsAsync("0", maxEvents: 100);
```

### Event Hubs Configuration

```json
{
  "EventHubs": {
    "ConnectionString": "Endpoint=sb://myeventhub.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=...",
    "EventHubName": "sensor-events",
    "ConsumerGroup": "$Default",
    "BlobStorageConnectionString": "DefaultEndpointsProtocol=https;AccountName=...",
    "BlobContainerName": "checkpoints",
    "MaxBatchSize": 100
  }
}
```

---

## RabbitMQ

### Publish Message

**Endpoint**: `POST /api/stream/rabbitmq/publish`

```csharp
var request = new RabbitMQPublishRequest
{
    Exchange = "order-exchange",
    RoutingKey = "order.created",
    Message = new { OrderId = 123, Total = 99.99 }
};

await _rabbitMQService.PublishAsync("order-exchange", "order.created", request.Message);
```

### Publish Batch

**Endpoint**: `POST /api/stream/rabbitmq/publish-batch`

```csharp
var messages = new List<object>
{
    new { OrderId = 1, Status = "Pending" },
    new { OrderId = 2, Status = "Processing" }
};

await _rabbitMQService.PublishBatchAsync("order-exchange", "order.created", messages);
```

### Consumer

```csharp
await _rabbitMQService.StartConsumerAsync("order-queue", async (message) =>
{
    var order = JsonConvert.DeserializeObject<Order>(
        Encoding.UTF8.GetString(message.Body));
    
    await ProcessOrder(order);
    return true;
}, cancellationToken);
```

### RabbitMQ Configuration

```json
{
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "VirtualHost": "/",
    "ExchangeName": "order-exchange",
    "QueueName": "order-queue",
    "RoutingKey": "order.created",
    "ExchangeType": "direct",
    "Durable": true,
    "PrefetchCount": 10
  }
}
```

---

## Redis Streams

### Add to Stream

**Endpoint**: `POST /api/stream/redis/add`

```csharp
var message = new { UserId = 123, Action = "ViewProduct", ProductId = 456 };
await _redisStreamService.AddToStreamAsync("user-activity", message);
```

### Add Batch

**Endpoint**: `POST /api/stream/redis/add-batch`

```csharp
var messages = new List<object>
{
    new { UserId = 1, Action = "Login" },
    new { UserId = 2, Action = "Logout" }
};

await _redisStreamService.AddBatchToStreamAsync("user-activity", messages);
```

### Consumer with Consumer Group

```csharp
await _redisStreamService.StartConsumerAsync(
    "user-activity",
    "analytics-group",
    async (message) =>
    {
        var activity = JsonConvert.DeserializeObject<UserActivity>(message.Values["data"]);
        await ProcessActivity(activity);
        return true;
    },
    cancellationToken);
```

### Read Stream

**Endpoint**: `GET /api/stream/redis/read/{streamKey}`

```csharp
var messages = await _redisStreamService.ReadStreamAsync("user-activity", count: 100);
```

### Redis Streams Configuration

```json
{
  "RedisStreams": {
    "ConnectionString": "localhost:6379",
    "StreamKey": "user-activity",
    "ConsumerGroup": "analytics-group",
    "MaxStreamLength": 10000,
    "BlockTimeMs": 5000,
    "BatchSize": 100
  }
}
```

---

## Stream Processing Patterns

### Windowing

Process events in time-based windows:

```csharp
var windowConfig = new WindowConfig
{
    WindowSize = TimeSpan.FromMinutes(5),
    SlideInterval = TimeSpan.FromMinutes(1),
    Type = WindowType.Sliding
};

var result = await _processingService.ProcessWindowAsync(
    messages,
    windowConfig,
    (events) => new Dictionary<string, object>
    {
        ["Count"] = events.Count,
        ["Average"] = events.Average(e => e.Value)
    });
```

### Batch Processing

```csharp
var results = await _processingService.ProcessBatchAsync(
    messages,
    async (batch) =>
    {
        await _database.BulkInsertAsync(batch);
        return true;
    });
```

### Dead Letter Queue

**Endpoint**: `GET /api/stream/dlq/messages`

```csharp
var deadLetterMessages = await _processingService.GetDeadLetterMessagesAsync(count: 100);

foreach (var dlqMessage in deadLetterMessages)
{
    Console.WriteLine($"Failed Message: {dlqMessage.OriginalMessageId}");
    Console.WriteLine($"Error: {dlqMessage.ErrorReason}");
    Console.WriteLine($"Failure Count: {dlqMessage.FailureCount}");
}
```

---

## Monitoring and Metrics

### Get Platform Metrics

**Kafka**: `GET /api/stream/kafka/metrics/{topic}`
**Service Bus**: `GET /api/stream/servicebus/metrics`
**Event Hubs**: `GET /api/stream/eventhub/metrics`
**RabbitMQ**: `GET /api/stream/rabbitmq/metrics`
**Redis**: `GET /api/stream/redis/metrics`

```csharp
var metrics = await _kafkaService.GetMetricsAsync("user-events");

Console.WriteLine($"Messages Processed: {metrics.MessagesProcessed}");
Console.WriteLine($"Messages Succeeded: {metrics.MessagesSucceeded}");
Console.WriteLine($"Messages Failed: {metrics.MessagesFailed}");
Console.WriteLine($"Average Processing Time: {metrics.AverageProcessingTimeMs}ms");
Console.WriteLine($"Messages Per Second: {metrics.MessagesPerSecond}");
Console.WriteLine($"Current Lag: {metrics.CurrentLag}");
```

### Prometheus Metrics

Metrics are exposed at `/metrics` endpoint for Prometheus scraping.

---

## Error Handling and Resilience

### Automatic Retry

```csharp
var config = new StreamProcessingConfig
{
    MaxRetries = 3,
    RetryDelay = TimeSpan.FromSeconds(5),
    EnableDeadLetterQueue = true
};
```

### Dead Letter Queue

Failed messages are automatically sent to the dead letter queue after max retries:

```csharp
var dlqMessages = await _processingService.GetDeadLetterMessagesAsync();

// Retry failed messages
foreach (var dlqMessage in dlqMessages)
{
    await RetryMessage(dlqMessage);
}
```

---

## Performance Tips

### Kafka
- Use batching for high throughput (`ProduceBatchAsync`)
- Enable compression (Snappy recommended)
- Configure `LingerMs` for optimal batch collection
- Use multiple consumer instances for parallel processing

### Service Bus
- Adjust `MaxConcurrentCalls` based on workload
- Use `PrefetchCount` for bulk processing
- Enable sessions for ordered processing
- Use partitioning for scalability

### Event Hubs
- Use partition keys for related events
- Configure appropriate `MaxBatchSize`
- Use Event Processor for automatic checkpointing
- Scale by adding partitions

### RabbitMQ
- Use persistent messages for durability
- Configure `PrefetchCount` for consumer optimization
- Use appropriate exchange types (direct, topic, fanout)
- Enable publisher confirms for reliability

### Redis Streams
- Set `MaxStreamLength` to prevent memory issues
- Use consumer groups for parallel processing
- Adjust `BlockTimeMs` for latency vs CPU trade-off
- Regularly acknowledge processed messages

---

## Testing

### Unit Tests

```csharp
[Fact]
public async Task ProduceToKafka_ValidMessage_ReturnsSuccess()
{
    // Arrange
    var config = new KafkaProducerConfig { BootstrapServers = "localhost:9092" };
    var kafkaService = new KafkaService(config, consumerConfig, logger);
    var message = new { Id = 1, Name = "Test" };

    // Act
    var result = await kafkaService.ProduceAsync("test-topic", message);

    // Assert
    Assert.True(result);
}
```

### Integration Tests

```csharp
[Fact]
public async Task EndToEnd_KafkaProducerConsumer_ProcessesMessages()
{
    // Arrange
    var producedMessages = new List<string>();
    var consumedMessages = new List<string>();

    // Act
    await _kafkaService.ProduceAsync("test-topic", "Test Message");
    
    await _kafkaService.StartConsumerAsync(
        new List<string> { "test-topic" },
        async (message) =>
        {
            consumedMessages.Add(message.Value);
            return true;
        },
        cts.Token);

    // Assert
    Assert.Equal(producedMessages.Count, consumedMessages.Count);
}
```

---

## Troubleshooting

### Issue: "Kafka broker not available"
**Solution**: Verify `BootstrapServers` is correct and Kafka is running

### Issue: "Service Bus authentication failed"
**Solution**: Check connection string and ensure proper permissions

### Issue: "Event Hub checkpoint errors"
**Solution**: Verify blob storage connection string and container exists

### Issue: "RabbitMQ connection refused"
**Solution**: Ensure RabbitMQ is running and credentials are correct

### Issue: "Redis connection timeout"
**Solution**: Check Redis server availability and connection string

---

## Dependencies

- **Confluent.Kafka** (2.3.0): Apache Kafka client
- **Azure.Messaging.ServiceBus** (7.17.5): Azure Service Bus integration
- **Azure.Messaging.EventHubs** (5.11.1): Azure Event Hubs integration
- **RabbitMQ.Client** (6.8.1): RabbitMQ client
- **StackExchange.Redis** (2.7.10): Redis client with streams support
- **Newtonsoft.Json** (13.0.3): JSON serialization
- **Serilog** (8.0.0): Structured logging
- **prometheus-net** (8.2.1): Prometheus metrics

---

## License

This module is part of the AI Learning Modules project.

---

## Support

For issues, questions, or contributions, please refer to the main project repository.
