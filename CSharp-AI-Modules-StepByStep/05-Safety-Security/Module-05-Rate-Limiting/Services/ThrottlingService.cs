using System.Threading.Channels;
using Module_05_Rate_Limiting.Models;

namespace Module_05_Rate_Limiting.Services;

public interface IThrottlingService
{
    Task<ThrottleResult> CheckThrottleAsync(string clientId, string resourceId, PriorityLevel priority = PriorityLevel.Normal);
    Task ReleaseResourceAsync(string clientId, string resourceId);
    Task<ResourceThrottle> GetResourceStatusAsync(string resourceId);
}

public class ThrottlingService : IThrottlingService
{
    private readonly ILogger<ThrottlingService> _logger;
    private readonly Dictionary<string, ResourceThrottle> _resources;
    private readonly Dictionary<string, Channel<ThrottleRequest>> _requestQueues;
    private readonly Dictionary<string, ThrottlePolicy> _policies;

    public ThrottlingService(ILogger<ThrottlingService> logger)
    {
        _logger = logger;
        _resources = new Dictionary<string, ResourceThrottle>();
        _requestQueues = new Dictionary<string, Channel<ThrottleRequest>>();
        _policies = InitializePolicies();
        InitializeResources();
    }

    private Dictionary<string, ThrottlePolicy> InitializePolicies()
    {
        return new Dictionary<string, ThrottlePolicy>
        {
            ["default"] = new ThrottlePolicy
            {
                PolicyId = "default",
                Name = "Default Throttle Policy",
                Strategy = ThrottleStrategy.Adaptive,
                MaxConcurrentRequests = 100,
                RequestTimeout = TimeSpan.FromSeconds(30),
                BackoffStrategy = BackoffStrategy.Exponential,
                MaxRetries = 3,
                DefaultPriority = PriorityLevel.Normal,
                EnablePriorityQueue = true
            },
            ["high_load"] = new ThrottlePolicy
            {
                PolicyId = "high_load",
                Name = "High Load Policy",
                Strategy = ThrottleStrategy.PriorityBased,
                MaxConcurrentRequests = 50,
                RequestTimeout = TimeSpan.FromSeconds(20),
                BackoffStrategy = BackoffStrategy.Exponential,
                MaxRetries = 2,
                DefaultPriority = PriorityLevel.Normal,
                EnablePriorityQueue = true
            }
        };
    }

    private void InitializeResources()
    {
        _resources["api"] = new ResourceThrottle
        {
            ResourceId = "api",
            ResourceType = "API",
            CurrentLoad = 0,
            MaxCapacity = 100,
            UtilizationPercentage = 0,
            IsOverloaded = false,
            QueuedRequests = 0,
            LastUpdated = DateTime.UtcNow
        };
    }

    public async Task<ThrottleResult> CheckThrottleAsync(string clientId, string resourceId, PriorityLevel priority = PriorityLevel.Normal)
    {
        var resource = GetOrCreateResource(resourceId);
        var policy = _policies["default"];

        // Update resource status
        UpdateResourceStatus(resource);

        // Check if resource is available
        if (resource.CurrentLoad < resource.MaxCapacity)
        {
            resource.CurrentLoad++;
            resource.UtilizationPercentage = (double)resource.CurrentLoad / resource.MaxCapacity * 100;
            resource.LastUpdated = DateTime.UtcNow;

            _logger.LogDebug("Request allowed for {Client} on {Resource}. Load: {Load}/{Capacity}",
                clientId, resourceId, resource.CurrentLoad, resource.MaxCapacity);

            return new ThrottleResult
            {
                IsThrottled = false,
                Priority = priority,
                QueuePosition = 0
            };
        }

        // Resource at capacity - apply throttling
        if (!policy.EnablePriorityQueue)
        {
            var waitTime = CalculateBackoffTime(policy.BackoffStrategy, 0);
            return new ThrottleResult
            {
                IsThrottled = true,
                Reason = "Resource at capacity",
                WaitTime = waitTime,
                Priority = priority,
                RetryAt = DateTime.UtcNow.Add(waitTime)
            };
        }

        // Queue request with priority
        var queue = GetOrCreateQueue(resourceId);
        var request = new ThrottleRequest
        {
            ClientId = clientId,
            ResourceId = resourceId,
            Priority = priority,
            RequestedAt = DateTime.UtcNow
        };

        await queue.Writer.WriteAsync(request);
        resource.QueuedRequests++;

        var queuePosition = EstimateQueuePosition(resourceId, priority);
        var estimatedWait = queuePosition * 2; // Estimate 2 seconds per queued request

        _logger.LogInformation("Request throttled for {Client}. Queue position: {Position}, Estimated wait: {Wait}s",
            clientId, queuePosition, estimatedWait);

        return new ThrottleResult
        {
            IsThrottled = true,
            Reason = "Resource capacity exceeded - request queued",
            QueuePosition = queuePosition,
            EstimatedWaitSeconds = estimatedWait,
            Priority = priority,
            RetryAt = DateTime.UtcNow.AddSeconds(estimatedWait)
        };
    }

    public async Task ReleaseResourceAsync(string clientId, string resourceId)
    {
        if (_resources.TryGetValue(resourceId, out var resource))
        {
            resource.CurrentLoad = Math.Max(0, resource.CurrentLoad - 1);
            resource.UtilizationPercentage = (double)resource.CurrentLoad / resource.MaxCapacity * 100;
            resource.LastUpdated = DateTime.UtcNow;

            _logger.LogDebug("Resource released by {Client} on {Resource}. Load: {Load}/{Capacity}",
                clientId, resourceId, resource.CurrentLoad, resource.MaxCapacity);

            // Process next request from queue if exists
            if (_requestQueues.TryGetValue(resourceId, out var queue))
            {
                if (queue.Reader.TryRead(out var nextRequest))
                {
                    resource.QueuedRequests--;
                    resource.CurrentLoad++;
                    _logger.LogInformation("Processing queued request for {Client}", nextRequest.ClientId);
                }
            }
        }

        await Task.CompletedTask;
    }

    public async Task<ResourceThrottle> GetResourceStatusAsync(string resourceId)
    {
        var resource = GetOrCreateResource(resourceId);
        UpdateResourceStatus(resource);
        return await Task.FromResult(resource);
    }

    private ResourceThrottle GetOrCreateResource(string resourceId)
    {
        if (!_resources.ContainsKey(resourceId))
        {
            _resources[resourceId] = new ResourceThrottle
            {
                ResourceId = resourceId,
                ResourceType = "Generic",
                CurrentLoad = 0,
                MaxCapacity = 100,
                UtilizationPercentage = 0,
                IsOverloaded = false,
                QueuedRequests = 0,
                LastUpdated = DateTime.UtcNow
            };
        }
        return _resources[resourceId];
    }

    private Channel<ThrottleRequest> GetOrCreateQueue(string resourceId)
    {
        if (!_requestQueues.ContainsKey(resourceId))
        {
            _requestQueues[resourceId] = Channel.CreateUnbounded<ThrottleRequest>(
                new UnboundedChannelOptions
                {
                    SingleReader = true,
                    SingleWriter = false
                });
        }
        return _requestQueues[resourceId];
    }

    private void UpdateResourceStatus(ResourceThrottle resource)
    {
        resource.UtilizationPercentage = (double)resource.CurrentLoad / resource.MaxCapacity * 100;
        resource.IsOverloaded = resource.UtilizationPercentage > 90;
        resource.LastUpdated = DateTime.UtcNow;
    }

    private int EstimateQueuePosition(string resourceId, PriorityLevel priority)
    {
        // Simplified queue position estimation
        if (_resources.TryGetValue(resourceId, out var resource))
        {
            return resource.QueuedRequests + 1;
        }
        return 1;
    }

    private TimeSpan CalculateBackoffTime(BackoffStrategy strategy, int retryCount)
    {
        return strategy switch
        {
            BackoffStrategy.Fixed => TimeSpan.FromSeconds(5),
            BackoffStrategy.Linear => TimeSpan.FromSeconds(5 * (retryCount + 1)),
            BackoffStrategy.Exponential => TimeSpan.FromSeconds(Math.Pow(2, retryCount)),
            BackoffStrategy.Fibonacci => TimeSpan.FromSeconds(Fibonacci(retryCount + 1)),
            BackoffStrategy.Decorrelated => TimeSpan.FromSeconds(Random.Shared.Next(1, (int)Math.Pow(2, retryCount + 1))),
            _ => TimeSpan.FromSeconds(5)
        };
    }

    private int Fibonacci(int n)
    {
        if (n <= 1) return 1;
        int a = 1, b = 1;
        for (int i = 2; i <= n; i++)
        {
            int temp = a + b;
            a = b;
            b = temp;
        }
        return b;
    }

    private class ThrottleRequest
    {
        public string ClientId { get; set; } = string.Empty;
        public string ResourceId { get; set; } = string.Empty;
        public PriorityLevel Priority { get; set; }
        public DateTime RequestedAt { get; set; }
    }
}
