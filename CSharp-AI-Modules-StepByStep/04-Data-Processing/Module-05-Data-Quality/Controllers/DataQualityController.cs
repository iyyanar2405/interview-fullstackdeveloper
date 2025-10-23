using AI.DataQuality.Models;
using AI.DataQuality.Services;
using Microsoft.AspNetCore.Mvc;

namespace AI.DataQuality.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DataQualityController : ControllerBase
{
    private readonly IDataValidationService _validationService;
    private readonly IDataProfilingService _profilingService;
    private readonly IAnomalyDetectionService _anomalyService;
    private readonly IDataCleansingService _cleansingService;
    private readonly IDataGovernanceService _governanceService;
    private readonly ILogger<DataQualityController> _logger;

    public DataQualityController(
        IDataValidationService validationService,
        IDataProfilingService profilingService,
        IAnomalyDetectionService anomalyService,
        IDataCleansingService cleansingService,
        IDataGovernanceService governanceService,
        ILogger<DataQualityController> logger)
    {
        _validationService = validationService;
        _profilingService = profilingService;
        _anomalyService = anomalyService;
        _cleansingService = cleansingService;
        _governanceService = governanceService;
        _logger = logger;
    }

    // Validation Endpoints
    [HttpPost("validate")]
    public async Task<ActionResult<ApiResponseModel<ValidationResult>>> ValidateData(
        [FromBody] DataValidationRequest request)
    {
        try
        {
            var result = await _validationService.ValidateDataAsync(request.Data, request.Rules);
            return Ok(ApiResponseModel<ValidationResult>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating data");
            return BadRequest(ApiResponseModel<ValidationResult>.ErrorResponse(ex.Message));
        }
    }

    [HttpPost("validate/schema")]
    public async Task<ActionResult<ApiResponseModel<SchemaValidationResult>>> ValidateSchema(
        [FromBody] SchemaValidationRequest request)
    {
        try
        {
            var result = await _validationService.ValidateSchemaAsync(request.Data, request.Schema);
            return Ok(ApiResponseModel<SchemaValidationResult>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating schema");
            return BadRequest(ApiResponseModel<SchemaValidationResult>.ErrorResponse(ex.Message));
        }
    }

    [HttpGet("validate/rules/{datasetType}")]
    public async Task<ActionResult<ApiResponseModel<List<DataQualityRule>>>> GetStandardRules(string datasetType)
    {
        try
        {
            var rules = await _validationService.GetStandardRulesAsync(datasetType);
            return Ok(ApiResponseModel<List<DataQualityRule>>.SuccessResponse(rules));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting standard rules");
            return BadRequest(ApiResponseModel<List<DataQualityRule>>.ErrorResponse(ex.Message));
        }
    }

    // Profiling Endpoints
    [HttpPost("profile")]
    public async Task<ActionResult<ApiResponseModel<DataProfile>>> ProfileDataset(
        [FromBody] DataProfilingRequest request)
    {
        try
        {
            var profile = await _profilingService.ProfileDatasetAsync(request.Data, request.DatasetName);
            return Ok(ApiResponseModel<DataProfile>.SuccessResponse(profile));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error profiling dataset");
            return BadRequest(ApiResponseModel<DataProfile>.ErrorResponse(ex.Message));
        }
    }

    [HttpPost("metrics")]
    public async Task<ActionResult<ApiResponseModel<DataQualityMetrics>>> CalculateMetrics(
        [FromBody] DataProfilingRequest request)
    {
        try
        {
            var metrics = await _profilingService.CalculateQualityMetricsAsync(request.Data, request.DatasetName);
            return Ok(ApiResponseModel<DataQualityMetrics>.SuccessResponse(metrics));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating metrics");
            return BadRequest(ApiResponseModel<DataQualityMetrics>.ErrorResponse(ex.Message));
        }
    }

    // Anomaly Detection Endpoints
    [HttpPost("anomalies/detect")]
    public async Task<ActionResult<ApiResponseModel<AnomalyDetectionResult>>> DetectAnomalies(
        [FromBody] AnomalyDetectionRequest request)
    {
        try
        {
            var result = await _anomalyService.DetectAnomaliesAsync(request);
            return Ok(ApiResponseModel<AnomalyDetectionResult>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error detecting anomalies");
            return BadRequest(ApiResponseModel<AnomalyDetectionResult>.ErrorResponse(ex.Message));
        }
    }

    [HttpPost("anomalies/outliers")]
    public async Task<ActionResult<ApiResponseModel<List<Anomaly>>>> DetectOutliers(
        [FromBody] OutlierDetectionRequest request)
    {
        try
        {
            var anomalies = await _anomalyService.DetectOutliersAsync(
                request.Values,
                request.ColumnName,
                request.Threshold);
            return Ok(ApiResponseModel<List<Anomaly>>.SuccessResponse(anomalies));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error detecting outliers");
            return BadRequest(ApiResponseModel<List<Anomaly>>.ErrorResponse(ex.Message));
        }
    }

    [HttpPost("anomalies/duplicates")]
    public async Task<ActionResult<ApiResponseModel<List<Anomaly>>>> DetectDuplicates(
        [FromBody] List<Dictionary<string, object>> data)
    {
        try
        {
            var anomalies = await _anomalyService.DetectDuplicatesAsync(data);
            return Ok(ApiResponseModel<List<Anomaly>>.SuccessResponse(anomalies));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error detecting duplicates");
            return BadRequest(ApiResponseModel<List<Anomaly>>.ErrorResponse(ex.Message));
        }
    }

    // Data Cleansing Endpoints
    [HttpPost("cleanse")]
    public async Task<ActionResult<ApiResponseModel<DataCleansingResult>>> CleanseData(
        [FromBody] DataCleansingRequest request)
    {
        try
        {
            var result = await _cleansingService.CleanseDataAsync(request);
            return Ok(ApiResponseModel<DataCleansingResult>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleansing data");
            return BadRequest(ApiResponseModel<DataCleansingResult>.ErrorResponse(ex.Message));
        }
    }

    [HttpPost("cleanse/duplicates")]
    public async Task<ActionResult<ApiResponseModel<List<Dictionary<string, object>>>>> RemoveDuplicates(
        [FromBody] List<Dictionary<string, object>> data)
    {
        try
        {
            var result = await _cleansingService.RemoveDuplicatesAsync(data);
            return Ok(ApiResponseModel<List<Dictionary<string, object>>>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing duplicates");
            return BadRequest(ApiResponseModel<List<Dictionary<string, object>>>.ErrorResponse(ex.Message));
        }
    }

    // Governance Endpoints
    [HttpPost("governance/check-compliance")]
    public async Task<ActionResult<ApiResponseModel<ComplianceReport>>> CheckCompliance(
        [FromBody] ComplianceCheckRequest request)
    {
        try
        {
            var report = await _governanceService.CheckComplianceAsync(request.Data, request.Policy);
            return Ok(ApiResponseModel<ComplianceReport>.SuccessResponse(report));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking compliance");
            return BadRequest(ApiResponseModel<ComplianceReport>.ErrorResponse(ex.Message));
        }
    }

    [HttpPost("governance/policies")]
    public async Task<ActionResult<ApiResponseModel<DataGovernancePolicy>>> CreatePolicy(
        [FromBody] CreatePolicyRequest request)
    {
        try
        {
            var policy = await _governanceService.CreatePolicyAsync(
                request.PolicyName,
                request.Description,
                request.Rules);
            return Ok(ApiResponseModel<DataGovernancePolicy>.SuccessResponse(policy));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating policy");
            return BadRequest(ApiResponseModel<DataGovernancePolicy>.ErrorResponse(ex.Message));
        }
    }

    [HttpGet("governance/policies")]
    public async Task<ActionResult<ApiResponseModel<List<DataGovernancePolicy>>>> GetPolicies()
    {
        try
        {
            var policies = await _governanceService.GetActivePoliciesAsync();
            return Ok(ApiResponseModel<List<DataGovernancePolicy>>.SuccessResponse(policies));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting policies");
            return BadRequest(ApiResponseModel<List<DataGovernancePolicy>>.ErrorResponse(ex.Message));
        }
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Service = "Data Quality Service"
        });
    }
}

// Request Models
public class DataValidationRequest
{
    public List<Dictionary<string, object>> Data { get; set; } = new();
    public List<DataQualityRule> Rules { get; set; } = new();
}

public class SchemaValidationRequest
{
    public List<Dictionary<string, object>> Data { get; set; } = new();
    public SchemaDefinition Schema { get; set; } = new();
}

public class DataProfilingRequest
{
    public string DatasetName { get; set; } = string.Empty;
    public List<Dictionary<string, object>> Data { get; set; } = new();
}

public class OutlierDetectionRequest
{
    public List<double> Values { get; set; } = new();
    public string ColumnName { get; set; } = string.Empty;
    public double Threshold { get; set; } = 3.0;
}

public class ComplianceCheckRequest
{
    public List<Dictionary<string, object>> Data { get; set; } = new();
    public DataGovernancePolicy Policy { get; set; } = new();
}

public class CreatePolicyRequest
{
    public string PolicyName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<DataQualityRule> Rules { get; set; } = new();
}
