using Microsoft.Extensions.Caching.Memory;
using Module_05_Rate_Limiting.Models;

namespace Module_05_Rate_Limiting.Services;

public interface IDDoSProtectionService
{
    Task<DDoSDetectionResult> AnalyzeRequestAsync(string ipAddress, string endpoint, string? userAgent = null);
    Task<bool> IsIpBlockedAsync(string ipAddress);
    Task<IpBlockRule> BlockIpAsync(IpBlockRequest request);
    Task<bool> UnblockIpAsync(string ipAddress);
    Task<TrafficAnalysis> GetTrafficAnalysisAsync(string ipAddress);
    Task<List<IpBlockRule>> GetBlockedIpsAsync();
}

public class DDoSProtectionService : IDDoSProtectionService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<DDoSProtectionService> _logger;
    private readonly Dictionary<string, IpBlockRule> _blockedIps;
    private readonly DDoSConfiguration _config;

    public DDoSProtectionService(
        IMemoryCache cache,
        ILogger<DDoSProtectionService> logger,
        DDoSConfiguration config)
    {
        _cache = cache;
        _logger = logger;
        _config = config;
        _blockedIps = new Dictionary<string, IpBlockRule>();
    }

    public async Task<DDoSDetectionResult> AnalyzeRequestAsync(string ipAddress, string endpoint, string? userAgent = null)
    {
        if (!_config.EnableDDoSProtection)
        {
            return new DDoSDetectionResult
            {
                IsSuspicious = false,
                ThreatLevel = ThreatLevel.None,
                IpAddress = ipAddress,
                RecommendedAction = DDoSAction.Allow
            };
        }

        // Track request
        await TrackRequestAsync(ipAddress, endpoint, userAgent);

        // Get traffic analysis
        var analysis = await GetTrafficAnalysisAsync(ipAddress);

        // Detect threats
        var detectionReasons = new List<string>();
        var threatLevel = ThreatLevel.None;
        var action = DDoSAction.Allow;

        // Check requests per second
        if (analysis.RequestsPerSecond > _config.RequestsPerSecondThreshold)
        {
            detectionReasons.Add($"High RPS: {analysis.RequestsPerSecond}");
            threatLevel = ThreatLevel.High;
            action = DDoSAction.Throttle;
        }

        // Check requests per minute
        if (analysis.RequestsPerMinute > _config.RequestsPerMinuteThreshold)
        {
            detectionReasons.Add($"High RPM: {analysis.RequestsPerMinute}");
            threatLevel = ThreatLevel.Critical;
            action = DDoSAction.Block;
        }

        // Check failure rate
        if (analysis.FailureRate > 0.5 && analysis.TotalRequests > 100)
        {
            detectionReasons.Add($"High failure rate: {analysis.FailureRate:P0}");
            if (threatLevel < ThreatLevel.Medium) threatLevel = ThreatLevel.Medium;
        }

        // Check for scanning behavior
        if (analysis.UniqueEndpoints > 50 && analysis.TotalRequests > 100)
        {
            detectionReasons.Add($"Scanning behavior: {analysis.UniqueEndpoints} unique endpoints");
            if (threatLevel < ThreatLevel.Medium) threatLevel = ThreatLevel.Medium;
            action = DDoSAction.Challenge;
        }

        // Check bot patterns
        if (analysis.IsBot)
        {
            detectionReasons.Add("Bot detected");
            if (threatLevel < ThreatLevel.Low) threatLevel = ThreatLevel.Low;
        }

        // Auto-block if configured
        if (_config.AutoBlockSuspiciousIps && threatLevel >= _config.AutoBlockThreshold)
        {
            await BlockIpAsync(new IpBlockRequest
            {
                IpAddress = ipAddress,
                Reason = BlockReason.DDoSDetected,
                IsPermanent = false,
                DurationMinutes = _config.AutoBlockDurationMinutes,
                Notes = string.Join(", ", detectionReasons)
            });
            action = DDoSAction.Block;
        }

        var result = new DDoSDetectionResult
        {
            IsSuspicious = detectionReasons.Count > 0,
            ThreatLevel = threatLevel,
            DetectionReasons = detectionReasons,
            IpAddress = ipAddress,
            RequestsPerSecond = analysis.RequestsPerSecond,
            TotalRequests = analysis.TotalRequests,
            ObservationPeriod = _config.AnalysisWindow,
            RecommendedAction = action
        };

        if (result.IsSuspicious)
        {
            _logger.LogWarning("DDoS threat detected from {IP}. Level: {Level}, Reasons: {Reasons}", 
                ipAddress, threatLevel, string.Join(", ", detectionReasons));
        }

        return result;
    }

    public async Task<bool> IsIpBlockedAsync(string ipAddress)
    {
        if (_blockedIps.TryGetValue(ipAddress, out var rule))
        {
            // Check expiration
            if (!rule.IsPermanent && rule.ExpiresAt.HasValue && rule.ExpiresAt.Value < DateTime.UtcNow)
            {
                _blockedIps.Remove(ipAddress);
                _logger.LogInformation("Block expired for IP: {IP}", ipAddress);
                return false;
            }
            return true;
        }
        return await Task.FromResult(false);
    }

    public async Task<IpBlockRule> BlockIpAsync(IpBlockRequest request)
    {
        var rule = new IpBlockRule
        {
            RuleId = Guid.NewGuid().ToString(),
            IpAddress = request.IpAddress,
            Reason = request.Reason,
            BlockedAt = DateTime.UtcNow,
            IsPermanent = request.IsPermanent,
            ExpiresAt = request.IsPermanent ? null : DateTime.UtcNow.AddMinutes(request.DurationMinutes ?? 60),
            Notes = request.Notes,
            ViolationCount = 1
        };

        _blockedIps[request.IpAddress] = rule;

        _logger.LogWarning("IP blocked: {IP}, Reason: {Reason}, Duration: {Duration}", 
            request.IpAddress, request.Reason, request.IsPermanent ? "Permanent" : $"{request.DurationMinutes} minutes");

        return await Task.FromResult(rule);
    }

    public async Task<bool> UnblockIpAsync(string ipAddress)
    {
        if (_blockedIps.Remove(ipAddress))
        {
            _logger.LogInformation("IP unblocked: {IP}", ipAddress);
            return await Task.FromResult(true);
        }
        return await Task.FromResult(false);
    }

    public async Task<TrafficAnalysis> GetTrafficAnalysisAsync(string ipAddress)
    {
        var key = $"traffic:{ipAddress}";
        var requests = _cache.Get<List<RequestRecord>>(key) ?? new List<RequestRecord>();

        var now = DateTime.UtcNow;
        var analysisStart = now - _config.AnalysisWindow;

        // Filter to analysis window
        requests = requests.Where(r => r.Timestamp >= analysisStart).ToList();

        var totalRequests = requests.Count;
        var failedRequests = requests.Count(r => r.StatusCode >= 400);
        var successfulRequests = totalRequests - failedRequests;
        var uniqueEndpoints = requests.Select(r => r.Endpoint).Distinct().Count();

        var endpointFrequency = requests
            .GroupBy(r => r.Endpoint)
            .ToDictionary(g => g.Key, g => g.Count());

        // Detect bot patterns
        var userAgents = requests.Select(r => r.UserAgent).Distinct().ToList();
        var isBot = userAgents.Any(ua => 
            ua?.ToLower().Contains("bot") == true || 
            ua?.ToLower().Contains("crawler") == true ||
            ua?.ToLower().Contains("spider") == true);

        // Calculate RPS and RPM
        var timespan = requests.Count > 1 ? (requests.Max(r => r.Timestamp) - requests.Min(r => r.Timestamp)).TotalSeconds : 1;
        var rps = (int)(totalRequests / Math.Max(1, timespan));
        var rpm = (int)(totalRequests / Math.Max(1, timespan / 60));

        return await Task.FromResult(new TrafficAnalysis
        {
            IpAddress = ipAddress,
            RequestsPerSecond = rps,
            RequestsPerMinute = rpm,
            TotalRequests = totalRequests,
            FailedRequests = failedRequests,
            SuccessfulRequests = successfulRequests,
            FailureRate = totalRequests > 0 ? (double)failedRequests / totalRequests : 0,
            AccessedEndpoints = endpointFrequency.Keys.ToList(),
            UniqueEndpoints = uniqueEndpoints,
            EndpointFrequency = endpointFrequency,
            AnalysisPeriodStart = analysisStart,
            AnalysisPeriodEnd = now,
            IsBot = isBot,
            UserAgent = userAgents.FirstOrDefault()
        });
    }

    public async Task<List<IpBlockRule>> GetBlockedIpsAsync()
    {
        // Remove expired blocks
        var now = DateTime.UtcNow;
        var expiredIps = _blockedIps.Where(kvp => 
            !kvp.Value.IsPermanent && 
            kvp.Value.ExpiresAt.HasValue && 
            kvp.Value.ExpiresAt.Value < now)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var ip in expiredIps)
        {
            _blockedIps.Remove(ip);
        }

        return await Task.FromResult(_blockedIps.Values.ToList());
    }

    private async Task TrackRequestAsync(string ipAddress, string endpoint, string? userAgent)
    {
        var key = $"traffic:{ipAddress}";
        var requests = _cache.Get<List<RequestRecord>>(key) ?? new List<RequestRecord>();

        requests.Add(new RequestRecord
        {
            Timestamp = DateTime.UtcNow,
            Endpoint = endpoint,
            UserAgent = userAgent,
            StatusCode = 200 // Will be updated by middleware
        });

        // Keep only recent requests
        var cutoff = DateTime.UtcNow - _config.AnalysisWindow;
        requests.RemoveAll(r => r.Timestamp < cutoff);

        _cache.Set(key, requests, _config.AnalysisWindow);

        await Task.CompletedTask;
    }

    private class RequestRecord
    {
        public DateTime Timestamp { get; set; }
        public string Endpoint { get; set; } = string.Empty;
        public string? UserAgent { get; set; }
        public int StatusCode { get; set; }
    }
}
