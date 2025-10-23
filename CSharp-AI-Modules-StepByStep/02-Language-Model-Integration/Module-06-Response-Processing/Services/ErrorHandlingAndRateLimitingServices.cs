using AI.ResponseProcessing.Models;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using System.Diagnostics;

namespace AI.ResponseProcessing.Services;

#region Error Handling Service

public interface IErrorHandlingService
{
    Task<ErrorHandlingResult<T>> ExecuteWithErrorHandlingAsync<T>(
        Func<Task<T>> operation,
        ErrorHandlingConfig config,
        string operationName);
    
    Task<RetryResult<T>> ExecuteWithRetryAsync<T>(
        Func<Task<T>> operation,
        RetryPolicy policy);
    
    Task LogErrorAsync(ErrorContext context);
    Task<List<ErrorContext>> GetRecentErrorsAsync(int count = 10);
}

public class ErrorHandlingService : IErrorHandlingService
{
    private readonly ILogger<ErrorHandlingService> _logger;
    private readonly List<ErrorContext> _errorHistory = new();
    private readonly SemaphoreSlim _errorHistoryLock = new(1, 1);

    public ErrorHandlingService(ILogger<ErrorHandlingService> logger)
    {
        _logger = logger;
    }

    public async Task<ErrorHandlingResult<T>> ExecuteWithErrorHandlingAsync<T>(
        Func<Task<T>> operation,
        ErrorHandlingConfig config,
        string operationName)
    {
        var attempts = 0;
        Exception? lastException = null;

        while (attempts < config.MaxRetries)
        {
            attempts++;

            try
            {
                var result = await operation();
                return new ErrorHandlingResult<T>
                {
                    Success = true,
                    Data = result,
                    AttemptsCount = attempts,
                    StrategyUsed = config.Strategy
                };
            }
            catch (Exception ex)
            {
                lastException = ex;
                
                await LogErrorAsync(new ErrorContext
                {
                    Exception = ex,
                    Operation = operationName,
                    AdditionalData = new Dictionary<string, object>
                    {
                        { "Attempt", attempts },
                        { "MaxRetries", config.MaxRetries }
                    }
                });

                // Check if exception is retryable
                if (!IsRetryable(ex, config))
                {
                    break;
                }

                // Apply delay before retry
                if (attempts < config.MaxRetries)
                {
                    await Task.Delay(config.RetryDelayMs * attempts);
                }
            }
        }

        // Handle failure based on strategy
        return await HandleFailureAsync<T>(lastException!, config, attempts);
    }

    public async Task<RetryResult<T>> ExecuteWithRetryAsync<T>(
        Func<Task<T>> operation,
        RetryPolicy policy)
    {
        var sw = Stopwatch.StartNew();
        var attempts = 0;
        Exception? lastException = null;

        while (attempts < policy.MaxAttempts)
        {
            attempts++;

            try
            {
                var result = await operation();
                sw.Stop();

                return new RetryResult<T>
                {
                    Success = true,
                    Result = result,
                    AttemptsMade = attempts,
                    TotalDuration = sw.Elapsed
                };
            }
            catch (Exception ex)
            {
                lastException = ex;

                if (policy.ShouldRetry != null && !policy.ShouldRetry(ex))
                {
                    break;
                }

                if (!IsRetryableException(ex, policy.RetryableExceptions))
                {
                    break;
                }

                if (attempts < policy.MaxAttempts)
                {
                    var delay = CalculateDelay(attempts, policy);
                    _logger.LogWarning("Retry attempt {Attempt} after {Delay}ms due to: {Error}",
                        attempts, delay.TotalMilliseconds, ex.Message);
                    await Task.Delay(delay);
                }
            }
        }

        sw.Stop();

        return new RetryResult<T>
        {
            Success = false,
            AttemptsMade = attempts,
            LastException = lastException,
            TotalDuration = sw.Elapsed
        };
    }

    public async Task LogErrorAsync(ErrorContext context)
    {
        _logger.LogError(context.Exception,
            "Error in operation: {Operation}. CorrelationId: {CorrelationId}",
            context.Operation, context.CorrelationId);

        await _errorHistoryLock.WaitAsync();
        try
        {
            _errorHistory.Add(context);
            
            // Keep only last 1000 errors
            if (_errorHistory.Count > 1000)
            {
                _errorHistory.RemoveAt(0);
            }
        }
        finally
        {
            _errorHistoryLock.Release();
        }
    }

    public async Task<List<ErrorContext>> GetRecentErrorsAsync(int count = 10)
    {
        await _errorHistoryLock.WaitAsync();
        try
        {
            return _errorHistory
                .OrderByDescending(e => e.Timestamp)
                .Take(count)
                .ToList();
        }
        finally
        {
            _errorHistoryLock.Release();
        }
    }

    #region Private Helper Methods

    private bool IsRetryable(Exception ex, ErrorHandlingConfig config)
    {
        if (!config.RetryableExceptions.Any())
            return true; // Retry all by default

        return config.RetryableExceptions.Any(type => type.IsInstanceOfType(ex));
    }

    private bool IsRetryableException(Exception ex, List<Type> retryableExceptions)
    {
        if (!retryableExceptions.Any())
            return true;

        return retryableExceptions.Any(type => type.IsInstanceOfType(ex));
    }

    private TimeSpan CalculateDelay(int attempt, RetryPolicy policy)
    {
        var delay = policy.InitialDelay.TotalMilliseconds * Math.Pow(policy.BackoffMultiplier, attempt - 1);
        delay = Math.Min(delay, policy.MaxDelay.TotalMilliseconds);
        return TimeSpan.FromMilliseconds(delay);
    }

    private async Task<ErrorHandlingResult<T>> HandleFailureAsync<T>(
        Exception exception,
        ErrorHandlingConfig config,
        int attempts)
    {
        return await Task.FromResult(config.Strategy switch
        {
            ErrorHandlingStrategy.Throw => throw exception,
            ErrorHandlingStrategy.ReturnDefault => new ErrorHandlingResult<T>
            {
                Success = false,
                Data = config.DefaultValue != null ? (T)config.DefaultValue : default,
                Error = exception,
                AttemptsCount = attempts,
                StrategyUsed = ErrorHandlingStrategy.ReturnDefault
            },
            ErrorHandlingStrategy.Log => new ErrorHandlingResult<T>
            {
                Success = false,
                Error = exception,
                AttemptsCount = attempts,
                StrategyUsed = ErrorHandlingStrategy.Log
            },
            _ => new ErrorHandlingResult<T>
            {
                Success = false,
                Error = exception,
                AttemptsCount = attempts,
                StrategyUsed = config.Strategy
            }
        });
    }

    #endregion
}

#endregion

#region Rate Limiting Service

public interface IRateLimitingService
{
    Task<RateLimitResult> CheckRateLimitAsync(RateLimitRequest request);
    Task<RateLimitInfo> GetRateLimitInfoAsync(string key);
    Task ResetRateLimitAsync(string key);
    Task<Dictionary<string, RateLimitInfo>> GetAllRateLimitsAsync();
}

public class RateLimitingService : IRateLimitingService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<RateLimitingService> _logger;
    private readonly RateLimitConfig _config;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public RateLimitingService(
        IMemoryCache cache,
        ILogger<RateLimitingService> logger,
        Microsoft.Extensions.Options.IOptions<ResponseProcessingSettings> settings)
    {
        _cache = cache;
        _logger = logger;
        _config = settings.Value.RateLimiting;
    }

    public async Task<RateLimitResult> CheckRateLimitAsync(RateLimitRequest request)
    {
        var key = BuildRateLimitKey(request);

        await _lock.WaitAsync();
        try
        {
            return _config.Strategy switch
            {
                RateLimitStrategy.FixedWindow => await CheckFixedWindowAsync(key, request),
                RateLimitStrategy.SlidingWindow => await CheckSlidingWindowAsync(key, request),
                RateLimitStrategy.TokenBucket => await CheckTokenBucketAsync(key, request),
                RateLimitStrategy.LeakyBucket => await CheckLeakyBucketAsync(key, request),
                RateLimitStrategy.Adaptive => await CheckAdaptiveAsync(key, request),
                _ => await CheckFixedWindowAsync(key, request)
            };
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<RateLimitInfo> GetRateLimitInfoAsync(string key)
    {
        var rateLimitKey = $"ratelimit_{key}";
        var data = _cache.Get<RateLimitData>(rateLimitKey);

        if (data == null)
        {
            return await Task.FromResult(new RateLimitInfo
            {
                Limit = _config.MaxRequests,
                Remaining = _config.MaxRequests,
                ResetTime = DateTime.UtcNow.Add(_config.TimeWindow),
                WindowDuration = _config.TimeWindow
            });
        }

        return await Task.FromResult(new RateLimitInfo
        {
            Limit = _config.MaxRequests,
            Remaining = Math.Max(0, _config.MaxRequests - data.RequestCount),
            ResetTime = data.WindowStart.Add(_config.TimeWindow),
            WindowDuration = _config.TimeWindow
        });
    }

    public async Task ResetRateLimitAsync(string key)
    {
        var rateLimitKey = $"ratelimit_{key}";
        _cache.Remove(rateLimitKey);
        _logger.LogInformation("Rate limit reset for key: {Key}", key);
        await Task.CompletedTask;
    }

    public async Task<Dictionary<string, RateLimitInfo>> GetAllRateLimitsAsync()
    {
        // Note: This is a simplified implementation
        // In production, use a distributed cache with key enumeration support
        var result = new Dictionary<string, RateLimitInfo>();
        return await Task.FromResult(result);
    }

    #region Private Helper Methods

    private string BuildRateLimitKey(RateLimitRequest request)
    {
        var parts = new List<string>();

        if (_config.EnablePerUser && !string.IsNullOrEmpty(request.UserId))
        {
            parts.Add($"user_{request.UserId}");
        }

        if (_config.EnablePerEndpoint && !string.IsNullOrEmpty(request.Endpoint))
        {
            parts.Add($"endpoint_{request.Endpoint}");
        }

        return parts.Any() ? string.Join("_", parts) : "global";
    }

    private async Task<RateLimitResult> CheckFixedWindowAsync(string key, RateLimitRequest request)
    {
        var rateLimitKey = $"ratelimit_{key}";
        var data = _cache.GetOrCreate(rateLimitKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = _config.TimeWindow;
            return new RateLimitData
            {
                RequestCount = 0,
                WindowStart = DateTime.UtcNow
            };
        });

        if (data == null)
        {
            data = new RateLimitData
            {
                RequestCount = 0,
                WindowStart = DateTime.UtcNow
            };
        }

        data.RequestCount += request.Weight;

        var allowed = data.RequestCount <= _config.MaxRequests;
        var resetTime = data.WindowStart.Add(_config.TimeWindow);

        if (allowed)
        {
            _cache.Set(rateLimitKey, data, resetTime);
        }

        _logger.LogDebug("Rate limit check for {Key}: {Count}/{Max}, Allowed: {Allowed}",
            key, data.RequestCount, _config.MaxRequests, allowed);

        return await Task.FromResult(new RateLimitResult
        {
            Allowed = allowed,
            RemainingRequests = Math.Max(0, _config.MaxRequests - data.RequestCount),
            ResetTime = resetTime,
            RetryAfter = allowed ? TimeSpan.Zero : resetTime - DateTime.UtcNow,
            RateLimitKey = key
        });
    }

    private async Task<RateLimitResult> CheckSlidingWindowAsync(string key, RateLimitRequest request)
    {
        var rateLimitKey = $"ratelimit_sliding_{key}";
        var timestamps = _cache.GetOrCreate(rateLimitKey, entry =>
        {
            entry.SlidingExpiration = _config.TimeWindow;
            return new List<DateTime>();
        }) ?? new List<DateTime>();

        var now = DateTime.UtcNow;
        var windowStart = now - _config.TimeWindow;

        // Remove old timestamps
        timestamps.RemoveAll(t => t < windowStart);

        var allowed = timestamps.Count < _config.MaxRequests;

        if (allowed)
        {
            timestamps.Add(now);
            _cache.Set(rateLimitKey, timestamps, new MemoryCacheEntryOptions
            {
                SlidingExpiration = _config.TimeWindow
            });
        }

        var oldestTimestamp = timestamps.Any() ? timestamps.Min() : now;
        var resetTime = oldestTimestamp.Add(_config.TimeWindow);

        return await Task.FromResult(new RateLimitResult
        {
            Allowed = allowed,
            RemainingRequests = Math.Max(0, _config.MaxRequests - timestamps.Count),
            ResetTime = resetTime,
            RetryAfter = allowed ? TimeSpan.Zero : resetTime - now,
            RateLimitKey = key
        });
    }

    private async Task<RateLimitResult> CheckTokenBucketAsync(string key, RateLimitRequest request)
    {
        var rateLimitKey = $"ratelimit_bucket_{key}";
        var bucket = _cache.GetOrCreate(rateLimitKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
            return new TokenBucket
            {
                Tokens = _config.MaxRequests,
                LastRefill = DateTime.UtcNow,
                Capacity = _config.MaxRequests
            };
        }) ?? new TokenBucket
        {
            Tokens = _config.MaxRequests,
            LastRefill = DateTime.UtcNow,
            Capacity = _config.MaxRequests
        };

        // Refill tokens based on time elapsed
        var now = DateTime.UtcNow;
        var elapsed = now - bucket.LastRefill;
        var refillRate = _config.MaxRequests / _config.TimeWindow.TotalSeconds;
        var tokensToAdd = (int)(elapsed.TotalSeconds * refillRate);

        if (tokensToAdd > 0)
        {
            bucket.Tokens = Math.Min(bucket.Capacity, bucket.Tokens + tokensToAdd);
            bucket.LastRefill = now;
        }

        var allowed = bucket.Tokens >= request.Weight;

        if (allowed)
        {
            bucket.Tokens -= request.Weight;
            _cache.Set(rateLimitKey, bucket);
        }

        return await Task.FromResult(new RateLimitResult
        {
            Allowed = allowed,
            RemainingRequests = bucket.Tokens,
            ResetTime = now.Add(_config.TimeWindow),
            RetryAfter = allowed ? TimeSpan.Zero : TimeSpan.FromSeconds(request.Weight / refillRate),
            RateLimitKey = key
        });
    }

    private async Task<RateLimitResult> CheckLeakyBucketAsync(string key, RateLimitRequest request)
    {
        // Simplified leaky bucket implementation
        return await CheckTokenBucketAsync(key, request);
    }

    private async Task<RateLimitResult> CheckAdaptiveAsync(string key, RateLimitRequest request)
    {
        // Adaptive rate limiting based on current load
        // For now, use sliding window
        return await CheckSlidingWindowAsync(key, request);
    }

    #endregion

    #region Helper Classes

    private class RateLimitData
    {
        public int RequestCount { get; set; }
        public DateTime WindowStart { get; set; }
    }

    private class TokenBucket
    {
        public int Tokens { get; set; }
        public DateTime LastRefill { get; set; }
        public int Capacity { get; set; }
    }

    #endregion
}

#endregion
