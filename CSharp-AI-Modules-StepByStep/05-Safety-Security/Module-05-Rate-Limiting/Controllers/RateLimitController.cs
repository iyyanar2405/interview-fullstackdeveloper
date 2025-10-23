using Microsoft.AspNetCore.Mvc;
using Module_05_Rate_Limiting.Models;
using Module_05_Rate_Limiting.Services;

namespace Module_05_Rate_Limiting.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RateLimitController : ControllerBase
{
    private readonly IRateLimitingService _rateLimitingService;
    private readonly IQuotaManagementService _quotaService;
    private readonly IDDoSProtectionService _ddosService;
    private readonly IThrottlingService _throttlingService;
    private readonly ILogger<RateLimitController> _logger;

    public RateLimitController(
        IRateLimitingService rateLimitingService,
        IQuotaManagementService quotaService,
        IDDoSProtectionService ddosService,
        IThrottlingService throttlingService,
        ILogger<RateLimitController> logger)
    {
        _rateLimitingService = rateLimitingService;
        _quotaService = quotaService;
        _ddosService = ddosService;
        _throttlingService = throttlingService;
        _logger = logger;
    }

    // ===== Rate Limiting Endpoints =====

    [HttpGet("status")]
    public async Task<ActionResult<RateLimitStatus>> GetRateLimitStatus(
        [FromQuery] string clientId,
        [FromQuery] string endpoint = "/")
    {
        try
        {
            var status = await _rateLimitingService.GetStatusAsync(clientId, endpoint);
            return Ok(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get rate limit status");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("rules")]
    public async Task<ActionResult<RateLimitRule>> AddRateLimitRule([FromBody] RateLimitRule rule)
    {
        try
        {
            var success = await _rateLimitingService.AddRuleAsync(rule);
            return success ? Ok(rule) : BadRequest(new { error = "Failed to add rule" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add rate limit rule");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("reset")]
    public async Task<ActionResult> ResetRateLimit(
        [FromQuery] string clientId,
        [FromQuery] string endpoint = "/")
    {
        try
        {
            await _rateLimitingService.ResetLimitAsync(clientId, endpoint);
            return Ok(new { message = "Rate limit reset successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reset rate limit");
            return BadRequest(new { error = ex.Message });
        }
    }

    // ===== Quota Management Endpoints =====

    [HttpGet("quota")]
    public async Task<ActionResult<QuotaUsage>> GetQuotaUsage([FromQuery] string clientId)
    {
        try
        {
            var usage = await _quotaService.GetUsageAsync(clientId);
            return Ok(usage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get quota usage");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("quota/allocation")]
    public async Task<ActionResult<QuotaAllocation>> GetQuotaAllocation([FromQuery] string clientId)
    {
        try
        {
            var allocation = await _quotaService.GetAllocationAsync(clientId);
            return Ok(allocation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get quota allocation");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("quota/check")]
    public async Task<ActionResult<QuotaCheckResponse>> CheckQuota([FromBody] QuotaCheckRequest request)
    {
        try
        {
            var result = await _quotaService.CheckQuotaAsync(request.ClientId, request.Endpoint, request.RequestCount);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check quota");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("quota/update")]
    public async Task<ActionResult> UpdateQuota([FromBody] PolicyUpdateRequest request)
    {
        try
        {
            var success = await _quotaService.UpdateQuotaAsync(
                request.ClientId,
                request.NewTier,
                request.CustomDailyLimit,
                request.CustomMonthlyLimit);

            return success 
                ? Ok(new { message = "Quota updated successfully" })
                : BadRequest(new { error = "Failed to update quota" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update quota");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("quota/reset")]
    public async Task<ActionResult> ResetQuota([FromQuery] string clientId)
    {
        try
        {
            await _quotaService.ResetQuotaAsync(clientId);
            return Ok(new { message = "Quota reset successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reset quota");
            return BadRequest(new { error = ex.Message });
        }
    }

    // ===== DDoS Protection Endpoints =====

    [HttpGet("ddos/traffic")]
    public async Task<ActionResult<TrafficAnalysis>> GetTrafficAnalysis([FromQuery] string ipAddress)
    {
        try
        {
            var analysis = await _ddosService.GetTrafficAnalysisAsync(ipAddress);
            return Ok(analysis);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get traffic analysis");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("ddos/block")]
    public async Task<ActionResult<IpBlockRule>> BlockIp([FromBody] IpBlockRequest request)
    {
        try
        {
            var rule = await _ddosService.BlockIpAsync(request);
            return Ok(rule);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to block IP");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("ddos/unblock")]
    public async Task<ActionResult> UnblockIp([FromQuery] string ipAddress)
    {
        try
        {
            var success = await _ddosService.UnblockIpAsync(ipAddress);
            return success 
                ? Ok(new { message = "IP unblocked successfully" })
                : NotFound(new { error = "IP not found in block list" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to unblock IP");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("ddos/blocked")]
    public async Task<ActionResult<List<IpBlockRule>>> GetBlockedIps()
    {
        try
        {
            var blockedIps = await _ddosService.GetBlockedIpsAsync();
            return Ok(blockedIps);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get blocked IPs");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("ddos/check")]
    public async Task<ActionResult<bool>> IsIpBlocked([FromQuery] string ipAddress)
    {
        try
        {
            var isBlocked = await _ddosService.IsIpBlockedAsync(ipAddress);
            return Ok(new { ipAddress, isBlocked });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check IP block status");
            return BadRequest(new { error = ex.Message });
        }
    }

    // ===== Throttling Endpoints =====

    [HttpGet("throttle/status")]
    public async Task<ActionResult<ResourceThrottle>> GetThrottleStatus([FromQuery] string resourceId = "api")
    {
        try
        {
            var status = await _throttlingService.GetResourceStatusAsync(resourceId);
            return Ok(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get throttle status");
            return BadRequest(new { error = ex.Message });
        }
    }

    // ===== Testing & Demo Endpoints =====

    [HttpGet("test/simple")]
    public ActionResult TestSimpleEndpoint()
    {
        return Ok(new
        {
            message = "Simple test endpoint",
            timestamp = DateTime.UtcNow,
            headers = new
            {
                rate_limit = Response.Headers["X-RateLimit-Remaining"].FirstOrDefault(),
                quota_daily = Response.Headers["X-Quota-Remaining-Daily"].FirstOrDefault(),
                quota_monthly = Response.Headers["X-Quota-Remaining-Monthly"].FirstOrDefault()
            }
        });
    }

    [HttpPost("test/load")]
    public async Task<ActionResult> TestLoadEndpoint()
    {
        // Simulate some processing
        await Task.Delay(100);
        
        return Ok(new
        {
            message = "Load test endpoint - 100ms delay",
            timestamp = DateTime.UtcNow
        });
    }

    [HttpGet("test/heavy")]
    public async Task<ActionResult> TestHeavyEndpoint()
    {
        // Simulate heavy processing
        await Task.Delay(1000);
        
        return Ok(new
        {
            message = "Heavy endpoint - 1s delay",
            timestamp = DateTime.UtcNow
        });
    }

    // ===== Health & Status =====

    [HttpGet("health")]
    public ActionResult GetHealth()
    {
        return Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            services = new
            {
                rateLimiting = "operational",
                quotaManagement = "operational",
                ddosProtection = "operational",
                throttling = "operational"
            }
        });
    }

    [HttpGet("stats")]
    public ActionResult GetStatistics()
    {
        return Ok(new
        {
            rate_limits = new
            {
                algorithms = new[] { "TokenBucket", "SlidingWindow", "FixedWindow", "LeakyBucket", "ConcurrentRequests" },
                active_rules = 1 // This would come from the service
            },
            quota_tiers = new[]
            {
                new { tier = "Free", daily = 1000, monthly = 10000 },
                new { tier = "Basic", daily = 10000, monthly = 250000 },
                new { tier = "Professional", daily = 100000, monthly = 2500000 },
                new { tier = "Enterprise", daily = 1000000, monthly = 25000000 }
            },
            ddos_protection = new
            {
                enabled = true,
                threshold_rps = 100,
                threshold_rpm = 1000
            }
        });
    }
}
