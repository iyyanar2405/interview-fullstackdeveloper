using AI.ModelManagement.Models;
using AI.ModelManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace AI.ModelManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ModelManagementController : ControllerBase
{
    private readonly IModelSelectionService _modelSelection;
    private readonly IFallbackService _fallback;
    private readonly ICostOptimizationService _costOptimization;
    private readonly IPerformanceMonitoringService _performanceMonitoring;
    private readonly ILogger<ModelManagementController> _logger;

    public ModelManagementController(
        IModelSelectionService modelSelection,
        IFallbackService fallback,
        ICostOptimizationService costOptimization,
        IPerformanceMonitoringService performanceMonitoring,
        ILogger<ModelManagementController> logger)
    {
        _modelSelection = modelSelection;
        _fallback = fallback;
        _costOptimization = costOptimization;
        _performanceMonitoring = performanceMonitoring;
        _logger = logger;
    }

    #region Model Selection Endpoints

    /// <summary>
    /// Select the best model based on requirements and strategy
    /// </summary>
    [HttpPost("select")]
    [ProducesResponseType(typeof(ModelSelectionResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> SelectModel([FromBody] ModelSelectionRequest request)
    {
        try
        {
            var response = await _modelSelection.SelectModelAsync(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error selecting model");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get all available models
    /// </summary>
    [HttpGet("models")]
    [ProducesResponseType(typeof(List<ModelConfiguration>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailableModels()
    {
        var models = await _modelSelection.GetAvailableModelsAsync();
        return Ok(models);
    }

    /// <summary>
    /// Get model by ID
    /// </summary>
    [HttpGet("models/{modelId}")]
    [ProducesResponseType(typeof(ModelConfiguration), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetModelById(string modelId)
    {
        var model = await _modelSelection.GetModelByIdAsync(modelId);
        if (model == null)
            return NotFound(new { error = $"Model {modelId} not found" });

        return Ok(model);
    }

    /// <summary>
    /// Update model status
    /// </summary>
    [HttpPut("models/{modelId}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateModelStatus(string modelId, [FromBody] ModelStatus status)
    {
        await _modelSelection.UpdateModelStatusAsync(modelId, status);
        return Ok(new { message = $"Model {modelId} status updated to {status}" });
    }

    #endregion

    #region Fallback Endpoints

    /// <summary>
    /// Get fallback model for a failed request
    /// </summary>
    [HttpPost("fallback")]
    [ProducesResponseType(typeof(FallbackResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFallbackModel([FromBody] FallbackRequest request)
    {
        try
        {
            var response = await _fallback.GetFallbackModelAsync(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting fallback model");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Record a model failure
    /// </summary>
    [HttpPost("fallback/record-failure")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RecordFailure(
        [FromQuery] string modelId,
        [FromQuery] string errorType)
    {
        await _fallback.RecordFailureAsync(modelId, errorType);
        return Ok(new { message = "Failure recorded" });
    }

    /// <summary>
    /// Check if fallback should be triggered
    /// </summary>
    [HttpGet("fallback/should-trigger")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> ShouldTriggerFallback(
        [FromQuery] string modelId,
        [FromQuery] string errorType)
    {
        var shouldTrigger = await _fallback.ShouldTriggerFallbackAsync(modelId, errorType);
        return Ok(new { shouldTrigger, modelId, errorType });
    }

    #endregion

    #region Cost Optimization Endpoints

    /// <summary>
    /// Track cost for a request
    /// </summary>
    [HttpPost("cost/track")]
    [ProducesResponseType(typeof(CostEntry), StatusCodes.Status200OK)]
    public async Task<IActionResult> TrackCost([FromBody] CostTrackingRequest request)
    {
        try
        {
            var entry = await _costOptimization.TrackCostAsync(request);
            return Ok(entry);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking cost");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get cost report for a date range
    /// </summary>
    [HttpGet("cost/report")]
    [ProducesResponseType(typeof(CostReport), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCostReport(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var report = await _costOptimization.GetCostReportAsync(startDate, endDate);
        return Ok(report);
    }

    /// <summary>
    /// Check if request is within budget
    /// </summary>
    [HttpGet("cost/check-budget")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckBudget([FromQuery] decimal estimatedCost)
    {
        var withinBudget = await _costOptimization.CheckBudgetAsync(estimatedCost);
        return Ok(new { withinBudget, estimatedCost });
    }

    /// <summary>
    /// Get active cost alerts
    /// </summary>
    [HttpGet("cost/alerts")]
    [ProducesResponseType(typeof(List<CostAlert>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveAlerts()
    {
        var alerts = await _costOptimization.GetActiveAlertsAsync();
        return Ok(alerts);
    }

    /// <summary>
    /// Get remaining budget for a period
    /// </summary>
    [HttpGet("cost/remaining-budget")]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRemainingBudget([FromQuery] string period = "daily")
    {
        var remaining = await _costOptimization.GetRemainingBudgetAsync(period);
        return Ok(new { period, remainingBudget = remaining });
    }

    #endregion

    #region Performance Monitoring Endpoints

    /// <summary>
    /// Record performance metrics
    /// </summary>
    [HttpPost("performance/record")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RecordMetrics([FromBody] PerformanceMetrics metrics)
    {
        await _performanceMonitoring.RecordMetricsAsync(metrics);
        return Ok(new { message = "Metrics recorded" });
    }

    /// <summary>
    /// Check health of a specific model
    /// </summary>
    [HttpGet("performance/health/{modelId}")]
    [ProducesResponseType(typeof(ModelHealthCheck), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckModelHealth(string modelId)
    {
        var health = await _performanceMonitoring.CheckModelHealthAsync(modelId);
        return Ok(health);
    }

    /// <summary>
    /// Get performance report for a date range
    /// </summary>
    [HttpGet("performance/report")]
    [ProducesResponseType(typeof(PerformanceReport), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPerformanceReport(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var report = await _performanceMonitoring.GetPerformanceReportAsync(startDate, endDate);
        return Ok(report);
    }

    /// <summary>
    /// Get health status of all models
    /// </summary>
    [HttpGet("performance/health")]
    [ProducesResponseType(typeof(Dictionary<string, HealthStatus>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllModelsHealth()
    {
        var health = await _performanceMonitoring.GetAllModelsHealthAsync();
        return Ok(health);
    }

    /// <summary>
    /// Get optimization recommendations
    /// </summary>
    [HttpGet("performance/recommendations")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecommendations()
    {
        var recommendations = await _performanceMonitoring.GetRecommendationsAsync();
        return Ok(recommendations);
    }

    #endregion

    #region Dashboard Endpoints

    /// <summary>
    /// Get comprehensive dashboard data
    /// </summary>
    [HttpGet("dashboard")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboard()
    {
        var endDate = DateTime.UtcNow;
        var startDate = endDate.AddDays(-7);

        var models = await _modelSelection.GetAvailableModelsAsync();
        var health = await _performanceMonitoring.GetAllModelsHealthAsync();
        var performanceReport = await _performanceMonitoring.GetPerformanceReportAsync(startDate, endDate);
        var costReport = await _costOptimization.GetCostReportAsync(startDate, endDate);
        var alerts = await _costOptimization.GetActiveAlertsAsync();
        var recommendations = await _performanceMonitoring.GetRecommendationsAsync();

        var dashboard = new
        {
            models = models.Select(m => new
            {
                m.ModelId,
                m.Name,
                m.Provider,
                m.Status,
                health = health.ContainsKey(m.ModelId) ? health[m.ModelId] : HealthStatus.Unknown
            }),
            performance = new
            {
                summary = performanceReport.Summary,
                topModels = performanceReport.ModelPerformance
                    .OrderByDescending(kv => kv.Value.SuccessRate)
                    .Take(5)
                    .ToDictionary(kv => kv.Key, kv => kv.Value)
            },
            cost = new
            {
                totalCost = costReport.TotalCost,
                trend = costReport.Trend,
                costByProvider = costReport.CostByProvider,
                topExpenses = costReport.TopExpenses.Take(5)
            },
            alerts,
            recommendations
        };

        return Ok(dashboard);
    }

    /// <summary>
    /// Get quick stats
    /// </summary>
    [HttpGet("stats")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetQuickStats()
    {
        var models = await _modelSelection.GetAvailableModelsAsync();
        var health = await _performanceMonitoring.GetAllModelsHealthAsync();
        var dailyBudget = await _costOptimization.GetRemainingBudgetAsync("daily");
        var monthlyBudget = await _costOptimization.GetRemainingBudgetAsync("monthly");
        var alerts = await _costOptimization.GetActiveAlertsAsync();

        var stats = new
        {
            totalModels = models.Count,
            healthyModels = health.Count(h => h.Value == HealthStatus.Healthy),
            degradedModels = health.Count(h => h.Value == HealthStatus.Degraded),
            unhealthyModels = health.Count(h => h.Value == HealthStatus.Unhealthy),
            remainingDailyBudget = dailyBudget,
            remainingMonthlyBudget = monthlyBudget,
            activeAlerts = alerts.Count
        };

        return Ok(stats);
    }

    #endregion
}
