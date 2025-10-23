using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Module_06_Storage_Solutions.Models;
using System.Text.Json;

namespace Module_06_Storage_Solutions.Services;

public interface ICachingService
{
    Task<T?> GetAsync<T>(string key);
    Task<bool> SetAsync<T>(string key, T value, CacheEntry entry);
    Task<bool> RemoveAsync(string key);
    Task<bool> ExistsAsync(string key);
    Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);
    Task<bool> RefreshAsync(string key);
    Task<bool> ClearAsync(string? pattern = null);
    Task<Dictionary<string, long>> GetCacheStatsAsync();
}

public class CachingService : ICachingService
{
    private readonly ILogger<CachingService> _logger;
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;
    private readonly CacheOptions _options;
    private long _hits;
    private long _misses;
    private long _sets;
    private long _removes;

    public CachingService(
        ILogger<CachingService> logger,
        IMemoryCache memoryCache,
        IDistributedCache distributedCache,
        IConfiguration configuration)
    {
        _logger = logger;
        _memoryCache = memoryCache;
        _distributedCache = distributedCache;
        _options = configuration.GetSection("CacheOptions").Get<CacheOptions>() ?? new CacheOptions();
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            // Try memory cache first (L1)
            if (_memoryCache.TryGetValue(key, out T? cachedValue))
            {
                Interlocked.Increment(ref _hits);
                _logger.LogDebug("Memory cache hit for key {Key}", key);
                return cachedValue;
            }

            // Try distributed cache (L2)
            var distributedValue = await _distributedCache.GetStringAsync(key);
            if (distributedValue != null)
            {
                Interlocked.Increment(ref _hits);
                var value = JsonSerializer.Deserialize<T>(distributedValue);
                
                // Populate memory cache
                _memoryCache.Set(key, value, TimeSpan.FromMinutes(5));
                
                _logger.LogDebug("Distributed cache hit for key {Key}", key);
                return value;
            }

            Interlocked.Increment(ref _misses);
            _logger.LogDebug("Cache miss for key {Key}", key);
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache key {Key}", key);
            Interlocked.Increment(ref _misses);
            return default;
        }
    }

    public async Task<bool> SetAsync<T>(string key, T value, CacheEntry entry)
    {
        try
        {
            var expiration = entry.Expiration ?? _options.DefaultExpiration;

            // Set in memory cache
            var memoryCacheOptions = new MemoryCacheEntryOptions();
            
            if (entry.ExpirationMode == CacheExpirationMode.Absolute)
            {
                memoryCacheOptions.SetAbsoluteExpiration(expiration);
            }
            else
            {
                memoryCacheOptions.SetSlidingExpiration(expiration);
            }

            _memoryCache.Set(key, value, memoryCacheOptions);

            // Set in distributed cache
            var json = JsonSerializer.Serialize(value);
            var distributedOptions = new DistributedCacheEntryOptions();
            
            if (entry.ExpirationMode == CacheExpirationMode.Absolute)
            {
                distributedOptions.SetAbsoluteExpiration(expiration);
            }
            else
            {
                distributedOptions.SetSlidingExpiration(expiration);
            }

            await _distributedCache.SetStringAsync(key, json, distributedOptions);

            Interlocked.Increment(ref _sets);
            _logger.LogDebug("Set cache key {Key} with expiration {Expiration}", key, expiration);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache key {Key}", key);
            return false;
        }
    }

    public async Task<bool> RemoveAsync(string key)
    {
        try
        {
            _memoryCache.Remove(key);
            await _distributedCache.RemoveAsync(key);

            Interlocked.Increment(ref _removes);
            _logger.LogDebug("Removed cache key {Key}", key);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache key {Key}", key);
            return false;
        }
    }

    public async Task<bool> ExistsAsync(string key)
    {
        try
        {
            if (_memoryCache.TryGetValue(key, out _))
            {
                return true;
            }

            var distributedValue = await _distributedCache.GetStringAsync(key);
            return distributedValue != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking cache key existence {Key}", key);
            return false;
        }
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
    {
        try
        {
            // Try to get from cache
            var cachedValue = await GetAsync<T>(key);
            if (cachedValue != null)
            {
                return cachedValue;
            }

            // Execute factory to get value
            var value = await factory();

            // Set in cache
            var entry = new CacheEntry
            {
                Key = key,
                Value = value,
                Expiration = expiration ?? _options.DefaultExpiration,
                ExpirationMode = CacheExpirationMode.Absolute
            };

            await SetAsync(key, value, entry);

            _logger.LogInformation("Cache key {Key} set from factory method", key);
            return value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetOrSetAsync for key {Key}", key);
            throw;
        }
    }

    public async Task<bool> RefreshAsync(string key)
    {
        try
        {
            await _distributedCache.RefreshAsync(key);
            _logger.LogDebug("Refreshed cache key {Key}", key);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing cache key {Key}", key);
            return false;
        }
    }

    public Task<bool> ClearAsync(string? pattern = null)
    {
        try
        {
            // Note: Memory cache doesn't support pattern-based clearing
            // This is a simplified implementation
            if (pattern == null)
            {
                // Clear all memory cache
                if (_memoryCache is MemoryCache mc)
                {
                    mc.Compact(1.0); // Remove all entries
                }

                _logger.LogInformation("Cleared all cache entries");
            }
            else
            {
                _logger.LogWarning("Pattern-based cache clearing not fully supported in memory cache");
            }

            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cache");
            return Task.FromResult(false);
        }
    }

    public Task<Dictionary<string, long>> GetCacheStatsAsync()
    {
        var stats = new Dictionary<string, long>
        {
            ["hits"] = _hits,
            ["misses"] = _misses,
            ["sets"] = _sets,
            ["removes"] = _removes,
            ["hit_rate"] = _hits + _misses > 0 ? (_hits * 100) / (_hits + _misses) : 0
        };

        return Task.FromResult(stats);
    }
}

public class CacheKeyBuilder
{
    private readonly List<string> _parts = new();

    public CacheKeyBuilder WithPrefix(string prefix)
    {
        _parts.Add(prefix);
        return this;
    }

    public CacheKeyBuilder WithEntity(string entity)
    {
        _parts.Add(entity);
        return this;
    }

    public CacheKeyBuilder WithId(string id)
    {
        _parts.Add(id);
        return this;
    }

    public CacheKeyBuilder WithSuffix(string suffix)
    {
        _parts.Add(suffix);
        return this;
    }

    public string Build()
    {
        return string.Join(":", _parts);
    }

    public static string Build(params string[] parts)
    {
        return string.Join(":", parts);
    }
}
