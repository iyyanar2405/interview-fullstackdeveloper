using AI.StreamProcessing.Models;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Messaging.EventHubs.Producer;
using Azure.Storage.Blobs;
using Newtonsoft.Json;
using System.Text;

namespace AI.StreamProcessing.Services;

public interface IEventHubService
{
    Task<bool> SendEventAsync<T>(T eventData, string? partitionKey = null);
    Task<bool> SendBatchAsync<T>(List<T> events);
    Task StartProcessorAsync(Func<EventData, Task<bool>> eventHandler, CancellationToken cancellationToken);
    Task<List<EventData>> ReadEventsAsync(string partitionId, int maxEvents = 100);
    Task<StreamMetrics> GetMetricsAsync();
}

public class EventHubService : IEventHubService
{
    private readonly EventHubConfig _config;
    private readonly ILogger<EventHubService> _logger;
    private readonly StreamMetrics _metrics;

    public EventHubService(EventHubConfig config, ILogger<EventHubService> logger)
    {
        _config = config;
        _logger = logger;
        _metrics = new StreamMetrics { StreamName = "Azure Event Hubs" };
    }

    public async Task<bool> SendEventAsync<T>(T eventData, string? partitionKey = null)
    {
        try
        {
            await using var producer = new EventHubProducerClient(_config.ConnectionString, _config.EventHubName);

            var serialized = JsonConvert.SerializeObject(eventData);
            var eventDataBatch = await producer.CreateBatchAsync();

            var eventHubData = new Azure.Messaging.EventHubs.EventData(Encoding.UTF8.GetBytes(serialized));
            
            if (!string.IsNullOrEmpty(partitionKey))
            {
                eventDataBatch = await producer.CreateBatchAsync(new CreateBatchOptions
                {
                    PartitionKey = partitionKey
                });
            }

            eventDataBatch.TryAdd(eventHubData);
            await producer.SendAsync(eventDataBatch);

            _logger.LogInformation($"Event sent to Event Hub: {_config.EventHubName}");
            _metrics.MessagesProcessed++;
            _metrics.MessagesSucceeded++;

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending event to Event Hub");
            _metrics.MessagesFailed++;
            return false;
        }
    }

    public async Task<bool> SendBatchAsync<T>(List<T> events)
    {
        try
        {
            await using var producer = new EventHubProducerClient(_config.ConnectionString, _config.EventHubName);

            var eventDataBatch = await producer.CreateBatchAsync();

            foreach (var eventData in events)
            {
                var serialized = JsonConvert.SerializeObject(eventData);
                var eventHubData = new Azure.Messaging.EventHubs.EventData(Encoding.UTF8.GetBytes(serialized));

                if (!eventDataBatch.TryAdd(eventHubData))
                {
                    await producer.SendAsync(eventDataBatch);
                    eventDataBatch.Dispose();
                    eventDataBatch = await producer.CreateBatchAsync();
                    eventDataBatch.TryAdd(eventHubData);
                }
            }

            if (eventDataBatch.Count > 0)
            {
                await producer.SendAsync(eventDataBatch);
            }

            _logger.LogInformation($"Batch of {events.Count} events sent to Event Hub");
            _metrics.MessagesProcessed += events.Count;
            _metrics.MessagesSucceeded += events.Count;

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending batch to Event Hub");
            _metrics.MessagesFailed += events.Count;
            return false;
        }
    }

    public async Task StartProcessorAsync(
        Func<EventData, Task<bool>> eventHandler,
        CancellationToken cancellationToken)
    {
        var storageClient = new BlobContainerClient(
            _config.BlobStorageConnectionString,
            _config.BlobContainerName);

        var processor = new EventProcessorClient(
            storageClient,
            _config.ConsumerGroup,
            _config.ConnectionString,
            _config.EventHubName);

        processor.ProcessEventAsync += async args =>
        {
            if (args.Data == null || !args.Data.Body.IsEmpty)
            {
                var startTime = DateTime.UtcNow;

                try
                {
                    var eventData = new EventData
                    {
                        Body = args.Data.Body.ToArray(),
                        SequenceNumber = args.Data.SequenceNumber,
                        Offset = args.Data.Offset,
                        PartitionKey = args.Data.PartitionKey ?? string.Empty,
                        EnqueuedTime = args.Data.EnqueuedTime.UtcDateTime
                    };

                    foreach (var prop in args.Data.Properties)
                    {
                        eventData.Properties[prop.Key] = prop.Value;
                    }

                    var success = await eventHandler(eventData);

                    if (success)
                    {
                        await args.UpdateCheckpointAsync(cancellationToken);
                        _metrics.MessagesSucceeded++;
                        _logger.LogDebug($"Event processed from partition {args.Partition.PartitionId}");
                    }
                    else
                    {
                        _metrics.MessagesFailed++;
                        _logger.LogWarning($"Event processing failed for partition {args.Partition.PartitionId}");
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
                    _logger.LogError(ex, "Error processing event");
                    _metrics.MessagesFailed++;
                }
            }
        };

        processor.ProcessErrorAsync += args =>
        {
            _logger.LogError(args.Exception, $"Event processor error on partition {args.PartitionId}");
            return Task.CompletedTask;
        };

        await processor.StartProcessingAsync(cancellationToken);
        _logger.LogInformation($"Event Hub processor started for {_config.EventHubName}");

        try
        {
            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Event Hub processor cancelled");
        }
        finally
        {
            await processor.StopProcessingAsync();
        }
    }

    public async Task<List<EventData>> ReadEventsAsync(string partitionId, int maxEvents = 100)
    {
        var events = new List<EventData>();

        try
        {
            await using var consumer = new EventHubConsumerClient(
                _config.ConsumerGroup,
                _config.ConnectionString,
                _config.EventHubName);

            var partition = await consumer.GetPartitionPropertiesAsync(partitionId);
            var startingPosition = EventPosition.FromSequenceNumber(partition.LastEnqueuedSequenceNumber - maxEvents);

            await foreach (var partitionEvent in consumer.ReadEventsFromPartitionAsync(
                partitionId,
                startingPosition,
                new ReadEventOptions { MaximumWaitTime = TimeSpan.FromSeconds(5) }))
            {
                if (partitionEvent.Data != null)
                {
                    events.Add(new EventData
                    {
                        Body = partitionEvent.Data.Body.ToArray(),
                        SequenceNumber = partitionEvent.Data.SequenceNumber,
                        Offset = partitionEvent.Data.Offset,
                        EnqueuedTime = partitionEvent.Data.EnqueuedTime.UtcDateTime
                    });

                    if (events.Count >= maxEvents)
                        break;
                }
            }

            _logger.LogInformation($"Read {events.Count} events from partition {partitionId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error reading events from partition {partitionId}");
        }

        return events;
    }

    public Task<StreamMetrics> GetMetricsAsync()
    {
        return Task.FromResult(_metrics);
    }
}
