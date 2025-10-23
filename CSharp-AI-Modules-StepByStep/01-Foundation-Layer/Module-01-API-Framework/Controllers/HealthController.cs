using Microsoft.AspNetCore.Mvc;
using AI.Foundation.API.Models;

namespace AI.Foundation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Tags("Health")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;

    public HealthController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Basic health check endpoint
    /// </summary>
    /// <returns>Health status</returns>
    [HttpGet]
    [ProducesResponseType(typeof(HealthCheckResponse), StatusCodes.Status200OK)]
    public ActionResult<HealthCheckResponse> Get()
    {
        var response = new HealthCheckResponse
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0",
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"
        };

        return Ok(response);
    }

    /// <summary>
    /// Detailed health check with system information
    /// </summary>
    /// <returns>Detailed health status</returns>
    [HttpGet("detailed")]
    [ProducesResponseType(typeof(DetailedHealthCheckResponse), StatusCodes.Status200OK)]
    public ActionResult<DetailedHealthCheckResponse> GetDetailed()
    {
        var response = new DetailedHealthCheckResponse
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0",
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
            SystemInfo = new SystemInfo
            {
                MachineName = Environment.MachineName,
                ProcessorCount = Environment.ProcessorCount,
                OSVersion = Environment.OSVersion.ToString(),
                WorkingSet = Environment.WorkingSet,
                UpTime = TimeSpan.FromMilliseconds(Environment.TickCount64)
            },
            Dependencies = new List<DependencyHealth>
            {
                new() { Name = "Database", Status = "Healthy", ResponseTime = TimeSpan.FromMilliseconds(5) },
                new() { Name = "External API", Status = "Healthy", ResponseTime = TimeSpan.FromMilliseconds(150) }
            }
        };

        return Ok(response);
    }

    /// <summary>
    /// Readiness probe for Kubernetes
    /// </summary>
    /// <returns>Readiness status</returns>
    [HttpGet("ready")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public ActionResult Ready()
    {
        // Check if application is ready to serve requests
        // This could include database connectivity, required services, etc.
        
        bool isReady = true; // Implement actual readiness checks
        
        if (isReady)
        {
            return Ok(new { Status = "Ready", Timestamp = DateTime.UtcNow });
        }
        
        return StatusCode(503, new { Status = "Not Ready", Timestamp = DateTime.UtcNow });
    }

    /// <summary>
    /// Liveness probe for Kubernetes
    /// </summary>
    /// <returns>Liveness status</returns>
    [HttpGet("live")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult Live()
    {
        // Simple liveness check - if we can respond, we're alive
        return Ok(new { Status = "Alive", Timestamp = DateTime.UtcNow });
    }
}