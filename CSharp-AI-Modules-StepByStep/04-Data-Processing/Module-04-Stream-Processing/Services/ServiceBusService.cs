using AI.StreamProcessing.Models;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

namespace AI.StreamProcessing.Services;

public interface IServiceBusService
{
    Task<bool> SendMessageAsync<T>(string queueOrTopic, T message);
    Task<bool> SendBatchAsync<T>(string queueOrTopic, List<T> messages);
    Task StartQueueProcessorAsync(string queueName, Func<ServiceBusMessage, Task<bool>> messageHandler, CancellationToken cancellationToken);
    Task StartTopicProcessorAsync(string topicName, string subscriptionName, Func<ServiceBusMessage, Task<bool>> messageHandler, CancellationToken cancellationToken);
    Task<StreamMetrics> GetMetricsAsync();
}

public class ServiceBusService : IServiceBusService
{
    private readonly ServiceBusConfig _config;
    private readonly ILogger<ServiceBusService> _logger;
    private readonly StreamMetrics _metrics;
    private ServiceBusClient? _client;

    public ServiceBusService(ServiceBusConfig config, ILogger<ServiceBusService> logger)
    {
        _config = config;
        _logger = logger;
        _metrics = new StreamMetrics { StreamName = "Azure Service Bus" };
    }

    private ServiceBusClient GetClient()
    {
        if (_client == null)
        {
            _client = new ServiceBusClient(_config.ConnectionString);
        }
        return _client;
    }

    public async Task<bool> SendMessageAsync<T>(string queueOrTopic, T message)
    {
        try
        {
            var client = GetClient();
            var sender = client.CreateSender(queueOrTopic);

            var serializedMessage = JsonConvert.SerializeObject(message);
            var serviceBusMessage = new Azure.Messaging.ServiceBus.ServiceBusMessage(serializedMessage)
            {
                MessageId = Guid.NewGuid().ToString(),
                ContentType = "application/json"
            };

            await sender.SendMessageAsync(serviceBusMessage);
            await sender.DisposeAsync();

            _logger.LogInformation($"Message sent to {queueOrTopic}");
            _metrics.MessagesProcessed++;
            _metrics.MessagesSucceeded++;

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending message to {queueOrTopic}");
            _metrics.MessagesFailed++;
            return false;
        }
    }

    public async Task<bool> SendBatchAsync<T>(string queueOrTopic, List<T> messages)
    {
        try
        {
            var client = GetClient();
            var sender = client.CreateSender(queueOrTopic);

            using var messageBatch = await sender.CreateMessageBatchAsync();

            foreach (var message in messages)
            {
                var serialized = JsonConvert.SerializeObject(message);
                var serviceBusMessage = new Azure.Messaging.ServiceBus.ServiceBusMessage(serialized)
                {
                    MessageId = Guid.NewGuid().ToString()
                };

                if (!messageBatch.TryAddMessage(serviceBusMessage))
                {
                    _logger.LogWarning("Message too large for batch, sending separately");
                    await sender.SendMessageAsync(serviceBusMessage);
                }
            }

            await sender.SendMessagesAsync(messageBatch);
            await sender.DisposeAsync();

            _logger.LogInformation($"Batch of {messages.Count} messages sent to {queueOrTopic}");
            _metrics.MessagesProcessed += messages.Count;
            _metrics.MessagesSucceeded += messages.Count;

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending batch to {queueOrTopic}");
            _metrics.MessagesFailed += messages.Count;
            return false;
        }
    }

    public async Task StartQueueProcessorAsync(
        string queueName,
        Func<ServiceBusMessage, Task<bool>> messageHandler,
        CancellationToken cancellationToken)
    {
        var client = GetClient();
        var processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions
        {
            MaxConcurrentCalls = _config.MaxConcurrentCalls,
            PrefetchCount = _config.PrefetchCount,
            AutoCompleteMessages = _config.EnableAutoComplete
        });

        processor.ProcessMessageAsync += async args =>
        {
            var startTime = DateTime.UtcNow;

            try
            {
                var message = new ServiceBusMessage
                {
                    MessageId = args.Message.MessageId,
                    CorrelationId = args.Message.CorrelationId ?? string.Empty,
                    SessionId = args.Message.SessionId ?? string.Empty,
                    Body = args.Message.Body.ToString(),
                    Subject = args.Message.Subject ?? string.Empty
                };

                foreach (var prop in args.Message.ApplicationProperties)
                {
                    message.ApplicationProperties[prop.Key] = prop.Value;
                }

                var success = await messageHandler(message);

                if (success)
                {
                    await args.CompleteMessageAsync(args.Message);
                    _metrics.MessagesSucceeded++;
                    _logger.LogDebug($"Message {args.Message.MessageId} processed successfully");
                }
                else
                {
                    await args.AbandonMessageAsync(args.Message);
                    _metrics.MessagesFailed++;
                    _logger.LogWarning($"Message {args.Message.MessageId} abandoned");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing message {args.Message.MessageId}");
                await args.DeadLetterMessageAsync(args.Message, "ProcessingError", ex.Message);
                _metrics.MessagesFailed++;
            }

            var processingTime = (DateTime.UtcNow - startTime).TotalMilliseconds;
            _metrics.AverageProcessingTimeMs = 
                (_metrics.AverageProcessingTimeMs * _metrics.MessagesProcessed + processingTime) 
                / (_metrics.MessagesProcessed + 1);
            _metrics.MessagesProcessed++;
            _metrics.LastProcessedTime = DateTime.UtcNow;
        };

        processor.ProcessErrorAsync += args =>
        {
            _logger.LogError(args.Exception, $"Service Bus processor error: {args.ErrorSource}");
            return Task.CompletedTask;
        };

        await processor.StartProcessingAsync(cancellationToken);
        _logger.LogInformation($"Service Bus processor started for queue: {queueName}");

        try
        {
            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Service Bus processor cancelled");
        }
        finally
        {
            await processor.StopProcessingAsync();
            await processor.DisposeAsync();
        }
    }

    public async Task StartTopicProcessorAsync(
        string topicName,
        string subscriptionName,
        Func<ServiceBusMessage, Task<bool>> messageHandler,
        CancellationToken cancellationToken)
    {
        var client = GetClient();
        var processor = client.CreateProcessor(topicName, subscriptionName, new ServiceBusProcessorOptions
        {
            MaxConcurrentCalls = _config.MaxConcurrentCalls,
            PrefetchCount = _config.PrefetchCount,
            AutoCompleteMessages = _config.EnableAutoComplete
        });

        processor.ProcessMessageAsync += async args =>
        {
            var startTime = DateTime.UtcNow;

            try
            {
                var message = new ServiceBusMessage
                {
                    MessageId = args.Message.MessageId,
                    Body = args.Message.Body.ToString()
                };

                var success = await messageHandler(message);

                if (success)
                {
                    await args.CompleteMessageAsync(args.Message);
                    _metrics.MessagesSucceeded++;
                }
                else
                {
                    await args.AbandonMessageAsync(args.Message);
                    _metrics.MessagesFailed++;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing topic message");
                await args.DeadLetterMessageAsync(args.Message, "ProcessingError", ex.Message);
                _metrics.MessagesFailed++;
            }

            _metrics.MessagesProcessed++;
            _metrics.LastProcessedTime = DateTime.UtcNow;
        };

        processor.ProcessErrorAsync += args =>
        {
            _logger.LogError(args.Exception, "Topic processor error");
            return Task.CompletedTask;
        };

        await processor.StartProcessingAsync(cancellationToken);
        _logger.LogInformation($"Topic processor started: {topicName}/{subscriptionName}");

        try
        {
            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Topic processor cancelled");
        }
        finally
        {
            await processor.StopProcessingAsync();
            await processor.DisposeAsync();
        }
    }

    public Task<StreamMetrics> GetMetricsAsync()
    {
        return Task.FromResult(_metrics);
    }
}
