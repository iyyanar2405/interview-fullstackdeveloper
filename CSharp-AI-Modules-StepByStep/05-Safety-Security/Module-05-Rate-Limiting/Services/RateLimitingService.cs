using Microsoft.Extensions.Caching.Memory;
using Module_05_Rate_Limiting.Models;

namespace Module_05_Rate_Limiting.Services;

public interface IRateLimitingService
{
    Task<RateLimitResult> CheckRateLimitAsync(string clientId, string endpoint, string? ipAddress = null);
    Task<RateLimitStatus> GetStatusAsync(string clientId, string endpoint);
    Task ResetLimitAsync(string clientId, string endpoint);
    Task<bool> AddRuleAsync(RateLimitRule rule);
}

public class RateLimitingService : IRateLimitingService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<RateLimitingService> _logger;
    private readonly Dictionary<string, RateLimitRule> _rules;
    private readonly RateLimitConfiguration _config;

    public RateLimitingService(
        IMemoryCache cache,
        ILogger<RateLimitingService> logger,
        RateLimitConfiguration config)
    {
        _cache = cache;
        _logger = logger;
        _config = config;
        _rules = new Dictionary<string, RateLimitRule>();
        InitializeDefaultRules();
    }

    private void InitializeDefaultRules()
    {
        // Global rate limit
        _rules["global"] = new RateLimitRule
        {
            RuleId = "global",
            Endpoint = "*",
            Algorithm = _config.DefaultAlgorithm,
            Limit = _config.DefaultLimit,
            Period = TimeSpan.FromSeconds(_config.DefaultPeriodSeconds),
            Priority = 0
        };
    }

    public async Task<RateLimitResult> CheckRateLimitAsync(string clientId, string endpoint, string? ipAddress = null)
    {
        // Check whitelist
        if (_config.WhitelistedClients.Contains(clientId) || 
            (ipAddress != null && _config.WhitelistedIps.Contains(ipAddress)))
        {
            return new RateLimitResult { IsAllowed = true, RemainingRequests = int.MaxValue };
        }

        // Find applicable rule
        var rule = FindApplicableRule(endpoint, clientId, ipAddress);
        if (rule == null || !rule.Enabled)
        {
            return new RateLimitResult { IsAllowed = true, RemainingRequests = int.MaxValue };
        }

        // Apply rate limiting algorithm
        return rule.Algorithm switch
        {
            RateLimitAlgorithm.TokenBucket => await ApplyTokenBucketAsync(clientId, endpoint, rule),
            RateLimitAlgorithm.SlidingWindow => await ApplySlidingWindowAsync(clientId, endpoint, rule),
            RateLimitAlgorithm.FixedWindow => await ApplyFixedWindowAsync(clientId, endpoint, rule),
            RateLimitAlgorithm.LeakyBucket => await ApplyLeakyBucketAsync(clientId, endpoint, rule),
            RateLimitAlgorithm.ConcurrentRequests => await ApplyConcurrentLimitAsync(clientId, endpoint, rule),
            _ => new RateLimitResult { IsAllowed = true, RemainingRequests = rule.Limit }
        };
    }

    private async Task<RateLimitResult> ApplyTokenBucketAsync(string clientId, string endpoint, RateLimitRule rule)
    {
        var key = $"token_bucket:{clientId}:{endpoint}";
        var bucket = _cache.Get<TokenBucket>(key) ?? new TokenBucket
        {
            Capacity = rule.Limit,
            Tokens = rule.Limit,
            RefillRate = rule.Limit / rule.Period.TotalSeconds,
            LastRefill = DateTime.UtcNow
        };

        // Refill tokens
        var now = DateTime.UtcNow;
        var timeSinceLastRefill = (now - bucket.LastRefill).TotalSeconds;
        var tokensToAdd = timeSinceLastRefill * bucket.RefillRate;
        bucket.Tokens = Math.Min(bucket.Capacity, bucket.Tokens + tokensToAdd);
        bucket.LastRefill = now;

        // Consume token
        if (bucket.Tokens >= 1)
        {
            bucket.Tokens--;
            _cache.Set(key, bucket, TimeSpan.FromHours(1));

            return await Task.FromResult(new RateLimitResult
            {
                IsAllowed = true,
                RemainingRequests = (int)bucket.Tokens,
                ResetTime = now.Add(rule.Period)
            });
        }

        // Calculate wait time
        var waitTime = TimeSpan.FromSeconds((1 - bucket.Tokens) / bucket.RefillRate);

        return new RateLimitResult
        {
            IsAllowed = false,
            ReasonDenied = "Token bucket exhausted",
            RemainingRequests = 0,
            RetryAfter = waitTime,
            ResetTime = now.Add(waitTime)
        };
    }

    private async Task<RateLimitResult> ApplySlidingWindowAsync(string clientId, string endpoint, RateLimitRule rule)
    {
        var key = $"sliding_window:{clientId}:{endpoint}";
        var now = DateTime.UtcNow;
        var windowStart = now - rule.Period;

        var requests = _cache.Get<List<DateTime>>(key) ?? new List<DateTime>();
        
        // Remove old requests outside window
        requests.RemoveAll(r => r < windowStart);

        if (requests.Count < rule.Limit)
        {
            requests.Add(now);
            _cache.Set(key, requests, rule.Period);

            return await Task.FromResult(new RateLimitResult
            {
                IsAllowed = true,
                RemainingRequests = rule.Limit - requests.Count,
                ResetTime = requests.Count > 0 ? requests[0].Add(rule.Period) : now.Add(rule.Period)
            });
        }

        var oldestRequest = requests.Min();
        var resetTime = oldestRequest.Add(rule.Period);

        return new RateLimitResult
        {
            IsAllowed = false,
            ReasonDenied = "Sliding window limit exceeded",
            RemainingRequests = 0,
            RetryAfter = resetTime - now,
            ResetTime = resetTime
        };
    }

    private async Task<RateLimitResult> ApplyFixedWindowAsync(string clientId, string endpoint, RateLimitRule rule)
    {
        var now = DateTime.UtcNow;
        var windowStart = new DateTime(
            now.Year, now.Month, now.Day, now.Hour, now.Minute, 
            (now.Second / (int)rule.Period.TotalSeconds) * (int)rule.Period.TotalSeconds);
        var windowEnd = windowStart.Add(rule.Period);

        var key = $"fixed_window:{clientId}:{endpoint}:{windowStart:yyyyMMddHHmmss}";
        var count = _cache.Get<int>(key);

        if (count < rule.Limit)
        {
            count++;
            _cache.Set(key, count, windowEnd - now);

            return await Task.FromResult(new RateLimitResult
            {
                IsAllowed = true,
                RemainingRequests = rule.Limit - count,
                ResetTime = windowEnd
            });
        }

        return new RateLimitResult
        {
            IsAllowed = false,
            ReasonDenied = "Fixed window limit exceeded",
            RemainingRequests = 0,
            RetryAfter = windowEnd - now,
            ResetTime = windowEnd
        };
    }

    private async Task<RateLimitResult> ApplyLeakyBucketAsync(string clientId, string endpoint, RateLimitRule rule)
    {
        var key = $"leaky_bucket:{clientId}:{endpoint}";
        var bucket = _cache.Get<LeakyBucket>(key) ?? new LeakyBucket
        {
            Capacity = rule.Limit,
            CurrentLevel = 0,
            LeakRate = rule.Limit / rule.Period.TotalSeconds,
            LastLeak = DateTime.UtcNow
        };

        var now = DateTime.UtcNow;
        var timeSinceLastLeak = (now - bucket.LastLeak).TotalSeconds;
        var leaked = timeSinceLastLeak * bucket.LeakRate;
        bucket.CurrentLevel = Math.Max(0, bucket.CurrentLevel - leaked);
        bucket.LastLeak = now;

        if (bucket.CurrentLevel < bucket.Capacity)
        {
            bucket.CurrentLevel++;
            _cache.Set(key, bucket, TimeSpan.FromHours(1));

            return await Task.FromResult(new RateLimitResult
            {
                IsAllowed = true,
                RemainingRequests = (int)(bucket.Capacity - bucket.CurrentLevel),
                ResetTime = now.Add(rule.Period)
            });
        }

        var waitTime = TimeSpan.FromSeconds((bucket.CurrentLevel - bucket.Capacity + 1) / bucket.LeakRate);

        return new RateLimitResult
        {
            IsAllowed = false,
            ReasonDenied = "Leaky bucket full",
            RemainingRequests = 0,
            RetryAfter = waitTime,
            ResetTime = now.Add(waitTime)
        };
    }

    private async Task<RateLimitResult> ApplyConcurrentLimitAsync(string clientId, string endpoint, RateLimitRule rule)
    {
        var key = $"concurrent:{clientId}:{endpoint}";
        var count = _cache.Get<int>(key);

        if (count < rule.Limit)
        {
            _cache.Set(key, count + 1, TimeSpan.FromMinutes(5));

            return await Task.FromResult(new RateLimitResult
            {
                IsAllowed = true,
                RemainingRequests = rule.Limit - count - 1
            });
        }

        return new RateLimitResult
        {
            IsAllowed = false,
            ReasonDenied = "Concurrent request limit exceeded",
            RemainingRequests = 0,
            RetryAfter = TimeSpan.FromSeconds(5)
        };
    }

    public async Task<RateLimitStatus> GetStatusAsync(string clientId, string endpoint)
    {
        var rule = FindApplicableRule(endpoint, clientId, null) ?? _rules["global"];
        var key = $"{rule.Algorithm.ToString().ToLower()}:{clientId}:{endpoint}";

        var now = DateTime.UtcNow;
        var windowStart = now - rule.Period;

        return await Task.FromResult(new RateLimitStatus
        {
            ClientId = clientId,
            Endpoint = endpoint,
            Limit = rule.Limit,
            Algorithm = rule.Algorithm,
            WindowStart = windowStart,
            WindowEnd = now,
            NextReset = now.Add(rule.Period)
        });
    }

    public async Task ResetLimitAsync(string clientId, string endpoint)
    {
        _logger.LogInformation("Resetting rate limit for client: {Client}, endpoint: {Endpoint}", clientId, endpoint);
        
        var patterns = new[] { "token_bucket", "sliding_window", "fixed_window", "leaky_bucket", "concurrent" };
        foreach (var pattern in patterns)
        {
            var key = $"{pattern}:{clientId}:{endpoint}";
            _cache.Remove(key);
        }

        await Task.CompletedTask;
    }

    public async Task<bool> AddRuleAsync(RateLimitRule rule)
    {
        _rules[rule.RuleId] = rule;
        _logger.LogInformation("Added rate limit rule: {RuleId}", rule.RuleId);
        return await Task.FromResult(true);
    }

    private RateLimitRule? FindApplicableRule(string endpoint, string? clientId, string? ipAddress)
    {
        // Priority: Specific client+endpoint > Endpoint > Global
        var key = $"{clientId}:{endpoint}";
        if (_rules.ContainsKey(key)) return _rules[key];

        if (_rules.ContainsKey(endpoint)) return _rules[endpoint];

        return _rules["global"];
    }

    private class TokenBucket
    {
        public double Capacity { get; set; }
        public double Tokens { get; set; }
        public double RefillRate { get; set; }
        public DateTime LastRefill { get; set; }
    }

    private class LeakyBucket
    {
        public double Capacity { get; set; }
        public double CurrentLevel { get; set; }
        public double LeakRate { get; set; }
        public DateTime LastLeak { get; set; }
    }
}
