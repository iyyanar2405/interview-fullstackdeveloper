using StackExchange.Redis;
using Module_06_Storage_Solutions.Models;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Module_06_Storage_Solutions.Services;

public interface IRedisService
{
    Task<bool> SetAsync(string key, object value, TimeSpan? expiration = null);
    Task<T?> GetAsync<T>(string key);
    Task<bool> DeleteAsync(string key);
    Task<bool> ExistsAsync(string key);
    Task<TimeSpan?> GetTimeToLiveAsync(string key);
    Task<bool> SetExpirationAsync(string key, TimeSpan expiration);
    Task<long> IncrementAsync(string key, long value = 1);
    Task<long> DecrementAsync(string key, long value = 1);
    Task<bool> SetHashAsync(string key, Dictionary<string, string> hash);
    Task<Dictionary<string, string>?> GetHashAsync(string key);
    Task<bool> AddToListAsync(string key, string value);
    Task<List<string>> GetListAsync(string key);
    Task<bool> AddToSetAsync(string key, string value);
    Task<List<string>> GetSetMembersAsync(string key);
    Task<bool> PublishAsync(string channel, string message);
    Task SubscribeAsync(string channel, Action<string> handler);
}

public class RedisService : IRedisService
{
    private readonly ILogger<RedisService> _logger;
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _database;

    public RedisService(ILogger<RedisService> logger, IConfiguration configuration)
    {
        _logger = logger;
        var config = configuration.GetSection("Redis").Get<RedisConfig>()
            ?? throw new InvalidOperationException("Redis configuration not found");

        var options = ConfigurationOptions.Parse(config.ConnectionString);
        options.ConnectTimeout = (int)config.ConnectTimeout.TotalMilliseconds;
        options.SyncTimeout = (int)config.SyncTimeout.TotalMilliseconds;
        options.ConnectRetry = config.ConnectRetry;
        options.AbortOnConnectFail = config.AbortOnConnectFail;
        options.AllowAdmin = config.AllowAdmin;

        _redis = ConnectionMultiplexer.Connect(options);
        _database = _redis.GetDatabase(config.Database);
    }

    public async Task<bool> SetAsync(string key, object value, TimeSpan? expiration = null)
    {
        try
        {
            var json = JsonSerializer.Serialize(value);
            var success = await _database.StringSetAsync(key, json, expiration);

            _logger.LogInformation("Set cache key {Key} with expiration {Expiration}",
                key, expiration?.ToString() ?? "never");
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache key {Key}", key);
            return false;
        }
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var value = await _database.StringGetAsync(key);
            
            if (!value.HasValue)
            {
                _logger.LogDebug("Cache miss for key {Key}", key);
                return default;
            }

            var result = JsonSerializer.Deserialize<T>(value!);
            _logger.LogDebug("Cache hit for key {Key}", key);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache key {Key}", key);
            return default;
        }
    }

    public async Task<bool> DeleteAsync(string key)
    {
        try
        {
            var success = await _database.KeyDeleteAsync(key);
            _logger.LogInformation("Deleted cache key {Key}", key);
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting cache key {Key}", key);
            return false;
        }
    }

    public async Task<bool> ExistsAsync(string key)
    {
        try
        {
            return await _database.KeyExistsAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence of cache key {Key}", key);
            return false;
        }
    }

    public async Task<TimeSpan?> GetTimeToLiveAsync(string key)
    {
        try
        {
            return await _database.KeyTimeToLiveAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting TTL for cache key {Key}", key);
            return null;
        }
    }

    public async Task<bool> SetExpirationAsync(string key, TimeSpan expiration)
    {
        try
        {
            var success = await _database.KeyExpireAsync(key, expiration);
            _logger.LogInformation("Set expiration for cache key {Key} to {Expiration}",
                key, expiration);
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting expiration for cache key {Key}", key);
            return false;
        }
    }

    public async Task<long> IncrementAsync(string key, long value = 1)
    {
        try
        {
            var result = await _database.StringIncrementAsync(key, value);
            _logger.LogDebug("Incremented cache key {Key} by {Value}, new value: {Result}",
                key, value, result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error incrementing cache key {Key}", key);
            throw;
        }
    }

    public async Task<long> DecrementAsync(string key, long value = 1)
    {
        try
        {
            var result = await _database.StringDecrementAsync(key, value);
            _logger.LogDebug("Decremented cache key {Key} by {Value}, new value: {Result}",
                key, value, result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error decrementing cache key {Key}", key);
            throw;
        }
    }

    public async Task<bool> SetHashAsync(string key, Dictionary<string, string> hash)
    {
        try
        {
            var entries = hash.Select(kvp => new HashEntry(kvp.Key, kvp.Value)).ToArray();
            await _database.HashSetAsync(key, entries);

            _logger.LogInformation("Set hash {Key} with {Count} fields", key, hash.Count);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting hash {Key}", key);
            return false;
        }
    }

    public async Task<Dictionary<string, string>?> GetHashAsync(string key)
    {
        try
        {
            var entries = await _database.HashGetAllAsync(key);
            
            if (entries.Length == 0)
            {
                return null;
            }

            var result = entries.ToDictionary(
                entry => entry.Name.ToString(),
                entry => entry.Value.ToString());

            _logger.LogDebug("Retrieved hash {Key} with {Count} fields", key, result.Count);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting hash {Key}", key);
            return null;
        }
    }

    public async Task<bool> AddToListAsync(string key, string value)
    {
        try
        {
            await _database.ListRightPushAsync(key, value);
            _logger.LogDebug("Added value to list {Key}", key);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding to list {Key}", key);
            return false;
        }
    }

    public async Task<List<string>> GetListAsync(string key)
    {
        try
        {
            var values = await _database.ListRangeAsync(key);
            var result = values.Select(v => v.ToString()).ToList();

            _logger.LogDebug("Retrieved list {Key} with {Count} items", key, result.Count);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting list {Key}", key);
            return new List<string>();
        }
    }

    public async Task<bool> AddToSetAsync(string key, string value)
    {
        try
        {
            var added = await _database.SetAddAsync(key, value);
            _logger.LogDebug("Added value to set {Key}, was new: {New}", key, added);
            return added;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding to set {Key}", key);
            return false;
        }
    }

    public async Task<List<string>> GetSetMembersAsync(string key)
    {
        try
        {
            var values = await _database.SetMembersAsync(key);
            var result = values.Select(v => v.ToString()).ToList();

            _logger.LogDebug("Retrieved set {Key} with {Count} members", key, result.Count);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting set {Key}", key);
            return new List<string>();
        }
    }

    public async Task<bool> PublishAsync(string channel, string message)
    {
        try
        {
            var subscriber = _redis.GetSubscriber();
            var receivers = await subscriber.PublishAsync(channel, message);

            _logger.LogInformation("Published message to channel {Channel}, received by {Receivers}",
                channel, receivers);
            return receivers > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing to channel {Channel}", channel);
            return false;
        }
    }

    public async Task SubscribeAsync(string channel, Action<string> handler)
    {
        try
        {
            var subscriber = _redis.GetSubscriber();
            await subscriber.SubscribeAsync(channel, (ch, message) =>
            {
                _logger.LogDebug("Received message on channel {Channel}", channel);
                handler(message!);
            });

            _logger.LogInformation("Subscribed to channel {Channel}", channel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error subscribing to channel {Channel}", channel);
        }
    }
}
