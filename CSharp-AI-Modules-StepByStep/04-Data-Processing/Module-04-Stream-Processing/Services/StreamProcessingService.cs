using AI.StreamProcessing.Models;

namespace AI.StreamProcessing.Services;

public interface IStreamProcessingService
{
    Task<ProcessingResult> ProcessMessageAsync<T>(StreamMessage<T> message, Func<T, Task<bool>> processor);
    Task<List<ProcessingResult>> ProcessBatchAsync<T>(List<StreamMessage<T>> messages, Func<List<T>, Task<bool>> batchProcessor);
    Task<WindowedResult<T>> ProcessWindowAsync<T>(List<StreamMessage<T>> messages, WindowConfig windowConfig, Func<List<T>, Dictionary<string, object>> aggregator);
    Task<bool> SendToDeadLetterQueueAsync(DeadLetterMessage message);
    Task<List<DeadLetterMessage>> GetDeadLetterMessagesAsync(int count = 100);
}

public class StreamProcessingService : IStreamProcessingService
{
    private readonly ILogger<StreamProcessingService> _logger;
    private readonly List<DeadLetterMessage> _deadLetterQueue = new();

    public StreamProcessingService(ILogger<StreamProcessingService> logger)
    {
        _logger = logger;
    }

    public async Task<ProcessingResult> ProcessMessageAsync<T>(
        StreamMessage<T> message,
        Func<T, Task<bool>> processor)
    {
        var startTime = DateTime.UtcNow;
        var result = new ProcessingResult
        {
            MessageId = message.MessageId
        };

        try
        {
            if (message.Payload == null)
            {
                result.Success = false;
                result.Error = "Message payload is null";
                return result;
            }

            var success = await processor(message.Payload);

            result.Success = success;
            result.ProcessingTime = DateTime.UtcNow - startTime;

            if (success)
            {
                _logger.LogInformation($"Message {message.MessageId} processed successfully");
            }
            else
            {
                _logger.LogWarning($"Message {message.MessageId} processing returned false");
                
                // Send to dead letter queue after retries
                if (message.RetryCount >= 3)
                {
                    await SendToDeadLetterQueueAsync(new DeadLetterMessage
                    {
                        OriginalMessageId = message.MessageId,
                        ErrorReason = "Max retries exceeded",
                        FailureCount = message.RetryCount,
                        FirstFailureTime = DateTime.UtcNow.AddMinutes(-message.RetryCount),
                        LastFailureTime = DateTime.UtcNow
                    });
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing message {message.MessageId}");
            result.Success = false;
            result.Error = ex.Message;
            result.ProcessingTime = DateTime.UtcNow - startTime;

            // Send to dead letter queue
            await SendToDeadLetterQueueAsync(new DeadLetterMessage
            {
                OriginalMessageId = message.MessageId,
                ErrorReason = ex.Message,
                StackTrace = ex.StackTrace ?? string.Empty,
                FailureCount = message.RetryCount + 1,
                FirstFailureTime = DateTime.UtcNow,
                LastFailureTime = DateTime.UtcNow
            });
        }

        return result;
    }

    public async Task<List<ProcessingResult>> ProcessBatchAsync<T>(
        List<StreamMessage<T>> messages,
        Func<List<T>, Task<bool>> batchProcessor)
    {
        var results = new List<ProcessingResult>();
        var startTime = DateTime.UtcNow;

        try
        {
            var payloads = messages
                .Where(m => m.Payload != null)
                .Select(m => m.Payload!)
                .ToList();

            if (payloads.Count == 0)
            {
                _logger.LogWarning("No valid payloads in batch");
                return results;
            }

            var success = await batchProcessor(payloads);
            var processingTime = DateTime.UtcNow - startTime;

            foreach (var message in messages)
            {
                results.Add(new ProcessingResult
                {
                    Success = success,
                    MessageId = message.MessageId,
                    ProcessingTime = processingTime
                });
            }

            if (success)
            {
                _logger.LogInformation($"Batch of {messages.Count} messages processed successfully");
            }
            else
            {
                _logger.LogWarning($"Batch processing failed for {messages.Count} messages");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing batch");

            foreach (var message in messages)
            {
                results.Add(new ProcessingResult
                {
                    Success = false,
                    MessageId = message.MessageId,
                    Error = ex.Message,
                    ProcessingTime = DateTime.UtcNow - startTime
                });

                await SendToDeadLetterQueueAsync(new DeadLetterMessage
                {
                    OriginalMessageId = message.MessageId,
                    ErrorReason = ex.Message,
                    StackTrace = ex.StackTrace ?? string.Empty,
                    FailureCount = 1,
                    FirstFailureTime = DateTime.UtcNow,
                    LastFailureTime = DateTime.UtcNow
                });
            }
        }

        return results;
    }

    public async Task<WindowedResult<T>> ProcessWindowAsync<T>(
        List<StreamMessage<T>> messages,
        WindowConfig windowConfig,
        Func<List<T>, Dictionary<string, object>> aggregator)
    {
        var result = new WindowedResult<T>();

        try
        {
            // Sort messages by timestamp
            var sortedMessages = messages.OrderBy(m => m.Timestamp).ToList();

            if (sortedMessages.Count == 0)
            {
                return result;
            }

            // Determine window boundaries
            var windowStart = sortedMessages.First().Timestamp;
            var windowEnd = windowConfig.Type switch
            {
                WindowType.Tumbling => windowStart.Add(windowConfig.WindowSize),
                WindowType.Sliding => windowStart.Add(windowConfig.WindowSize),
                WindowType.Session => sortedMessages.Last().Timestamp,
                WindowType.Count => sortedMessages.Last().Timestamp,
                _ => windowStart.Add(windowConfig.WindowSize)
            };

            // Filter messages in window
            var windowMessages = windowConfig.Type switch
            {
                WindowType.Tumbling => sortedMessages.Where(m => 
                    m.Timestamp >= windowStart && m.Timestamp < windowEnd).ToList(),
                WindowType.Sliding => sortedMessages.Where(m => 
                    m.Timestamp >= windowStart && m.Timestamp < windowEnd).ToList(),
                WindowType.Count => sortedMessages.Take(windowConfig.MaxEventsPerWindow).ToList(),
                _ => sortedMessages.ToList()
            };

            var payloads = windowMessages
                .Where(m => m.Payload != null)
                .Select(m => m.Payload!)
                .ToList();

            // Apply aggregation function
            var aggregates = await Task.Run(() => aggregator(payloads));

            result.WindowStart = windowStart;
            result.WindowEnd = windowEnd;
            result.Events = payloads;
            result.EventCount = payloads.Count;
            result.Aggregates = aggregates;

            _logger.LogInformation($"Window processed: {result.EventCount} events from {windowStart} to {windowEnd}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing window");
        }

        return result;
    }

    public Task<bool> SendToDeadLetterQueueAsync(DeadLetterMessage message)
    {
        try
        {
            _deadLetterQueue.Add(message);
            _logger.LogWarning($"Message {message.OriginalMessageId} sent to dead letter queue: {message.ErrorReason}");
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message to dead letter queue");
            return Task.FromResult(false);
        }
    }

    public Task<List<DeadLetterMessage>> GetDeadLetterMessagesAsync(int count = 100)
    {
        return Task.FromResult(_deadLetterQueue.Take(count).ToList());
    }
}
