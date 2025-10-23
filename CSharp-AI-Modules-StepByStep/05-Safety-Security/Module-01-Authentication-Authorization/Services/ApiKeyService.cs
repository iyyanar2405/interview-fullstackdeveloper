using Module_01_Authentication_Authorization.Models;
using System.Security.Cryptography;

namespace Module_01_Authentication_Authorization.Services;

public interface IApiKeyService
{
    Task<ApiKeyResponse> CreateApiKeyAsync(Guid userId, ApiKeyCreateRequest request);
    Task<ApiKey?> ValidateApiKeyAsync(string key);
    Task<bool> RevokeApiKeyAsync(Guid apiKeyId);
    Task<bool> RotateApiKeyAsync(Guid apiKeyId);
    Task<List<ApiKey>> GetUserApiKeysAsync(Guid userId);
    Task<bool> UpdateApiKeyAsync(Guid apiKeyId, ApiKeyCreateRequest request);
    Task<bool> CheckRateLimitAsync(Guid apiKeyId);
}

public class ApiKeyService : IApiKeyService
{
    private readonly ILogger<ApiKeyService> _logger;
    private readonly List<ApiKey> _apiKeys;
    private readonly Dictionary<Guid, RateLimitTracker> _rateLimitTrackers;

    public ApiKeyService(ILogger<ApiKeyService> logger)
    {
        _logger = logger;
        _apiKeys = new List<ApiKey>();
        _rateLimitTrackers = new Dictionary<Guid, RateLimitTracker>();
    }

    public Task<ApiKeyResponse> CreateApiKeyAsync(Guid userId, ApiKeyCreateRequest request)
    {
        try
        {
            // Generate secure API key
            var key = GenerateApiKey();
            var keyHash = HashApiKey(key);

            var apiKey = new ApiKey
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = request.Name,
                KeyHash = keyHash,
                KeyPrefix = key.Substring(0, 8), // Store prefix for identification
                Scopes = request.Scopes ?? new List<string>(),
                IpWhitelist = request.IpWhitelist ?? new List<string>(),
                RateLimitConfig = request.RateLimitConfig ?? new RateLimitConfig
                {
                    RequestsPerMinute = 60,
                    RequestsPerHour = 1000,
                    RequestsPerDay = 10000
                },
                ExpiresAt = request.ExpiresAt,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _apiKeys.Add(apiKey);

            _logger.LogInformation("API key created for user {UserId}: {ApiKeyId}", userId, apiKey.Id);

            return Task.FromResult(new ApiKeyResponse
            {
                Id = apiKey.Id,
                Key = key, // Return full key only once at creation
                Name = apiKey.Name,
                KeyPrefix = apiKey.KeyPrefix,
                Scopes = apiKey.Scopes,
                ExpiresAt = apiKey.ExpiresAt,
                CreatedAt = apiKey.CreatedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating API key for user {UserId}", userId);
            throw;
        }
    }

    public Task<ApiKey?> ValidateApiKeyAsync(string key)
    {
        try
        {
            var keyHash = HashApiKey(key);
            var keyPrefix = key.Substring(0, 8);

            var apiKey = _apiKeys.FirstOrDefault(k =>
                k.KeyHash == keyHash &&
                k.KeyPrefix == keyPrefix &&
                k.IsActive &&
                (!k.IsRevoked || k.IsRevoked == false) &&
                (k.ExpiresAt == null || k.ExpiresAt > DateTime.UtcNow));

            if (apiKey != null)
            {
                apiKey.LastUsedAt = DateTime.UtcNow;
                _logger.LogDebug("API key validated: {ApiKeyId}", apiKey.Id);
            }
            else
            {
                _logger.LogWarning("Invalid API key attempt with prefix: {Prefix}", keyPrefix);
            }

            return Task.FromResult(apiKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating API key");
            return Task.FromResult<ApiKey?>(null);
        }
    }

    public Task<bool> RevokeApiKeyAsync(Guid apiKeyId)
    {
        try
        {
            var apiKey = _apiKeys.FirstOrDefault(k => k.Id == apiKeyId);

            if (apiKey == null)
            {
                _logger.LogWarning("API key not found for revocation: {ApiKeyId}", apiKeyId);
                return Task.FromResult(false);
            }

            apiKey.IsRevoked = true;
            apiKey.RevokedAt = DateTime.UtcNow;

            _logger.LogInformation("API key revoked: {ApiKeyId}", apiKeyId);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking API key {ApiKeyId}", apiKeyId);
            return Task.FromResult(false);
        }
    }

    public async Task<bool> RotateApiKeyAsync(Guid apiKeyId)
    {
        try
        {
            var apiKey = _apiKeys.FirstOrDefault(k => k.Id == apiKeyId);

            if (apiKey == null)
            {
                return false;
            }

            // Generate new key
            var newKey = GenerateApiKey();
            var newKeyHash = HashApiKey(newKey);

            apiKey.KeyHash = newKeyHash;
            apiKey.KeyPrefix = newKey.Substring(0, 8);
            apiKey.UpdatedAt = DateTime.UtcNow;

            _logger.LogInformation("API key rotated: {ApiKeyId}", apiKeyId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rotating API key {ApiKeyId}", apiKeyId);
            return false;
        }
    }

    public Task<List<ApiKey>> GetUserApiKeysAsync(Guid userId)
    {
        try
        {
            var apiKeys = _apiKeys
                .Where(k => k.UserId == userId)
                .OrderByDescending(k => k.CreatedAt)
                .ToList();

            return Task.FromResult(apiKeys);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting API keys for user {UserId}", userId);
            return Task.FromResult(new List<ApiKey>());
        }
    }

    public Task<bool> UpdateApiKeyAsync(Guid apiKeyId, ApiKeyCreateRequest request)
    {
        try
        {
            var apiKey = _apiKeys.FirstOrDefault(k => k.Id == apiKeyId);

            if (apiKey == null)
            {
                return Task.FromResult(false);
            }

            apiKey.Name = request.Name;
            apiKey.Scopes = request.Scopes ?? apiKey.Scopes;
            apiKey.IpWhitelist = request.IpWhitelist ?? apiKey.IpWhitelist;
            apiKey.RateLimitConfig = request.RateLimitConfig ?? apiKey.RateLimitConfig;
            apiKey.ExpiresAt = request.ExpiresAt;
            apiKey.UpdatedAt = DateTime.UtcNow;

            _logger.LogInformation("API key updated: {ApiKeyId}", apiKeyId);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating API key {ApiKeyId}", apiKeyId);
            return Task.FromResult(false);
        }
    }

    public Task<bool> CheckRateLimitAsync(Guid apiKeyId)
    {
        try
        {
            var apiKey = _apiKeys.FirstOrDefault(k => k.Id == apiKeyId);

            if (apiKey == null || apiKey.RateLimitConfig == null)
            {
                return Task.FromResult(true);
            }

            if (!_rateLimitTrackers.TryGetValue(apiKeyId, out var tracker))
            {
                tracker = new RateLimitTracker();
                _rateLimitTrackers[apiKeyId] = tracker;
            }

            var now = DateTime.UtcNow;

            // Clean old entries
            tracker.Requests.RemoveAll(r => r < now.AddDays(-1));

            // Check rate limits
            var requestsLastMinute = tracker.Requests.Count(r => r > now.AddMinutes(-1));
            var requestsLastHour = tracker.Requests.Count(r => r > now.AddHours(-1));
            var requestsLastDay = tracker.Requests.Count(r => r > now.AddDays(-1));

            if (requestsLastMinute >= apiKey.RateLimitConfig.RequestsPerMinute)
            {
                _logger.LogWarning("Rate limit exceeded (per minute) for API key: {ApiKeyId}", apiKeyId);
                return Task.FromResult(false);
            }

            if (requestsLastHour >= apiKey.RateLimitConfig.RequestsPerHour)
            {
                _logger.LogWarning("Rate limit exceeded (per hour) for API key: {ApiKeyId}", apiKeyId);
                return Task.FromResult(false);
            }

            if (requestsLastDay >= apiKey.RateLimitConfig.RequestsPerDay)
            {
                _logger.LogWarning("Rate limit exceeded (per day) for API key: {ApiKeyId}", apiKeyId);
                return Task.FromResult(false);
            }

            // Add current request
            tracker.Requests.Add(now);

            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking rate limit for API key {ApiKeyId}", apiKeyId);
            return Task.FromResult(true); // Allow request on error
        }
    }

    private string GenerateApiKey()
    {
        // Generate a 32-byte random key
        var bytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);

        // Convert to base64 and make it URL-safe
        var key = Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");

        return $"sk_{key}";
    }

    private string HashApiKey(string key)
    {
        using var sha256 = SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(key);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    private class RateLimitTracker
    {
        public List<DateTime> Requests { get; set; } = new List<DateTime>();
    }
}
