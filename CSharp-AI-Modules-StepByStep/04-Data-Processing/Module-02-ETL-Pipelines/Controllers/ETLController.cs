using AI.ETL.Models;
using AI.ETL.Services;
using Microsoft.AspNetCore.Mvc;

namespace AI.ETL.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ETLController : ControllerBase
{
    private readonly IETLPipelineService _pipelineService;
    private readonly IValidationService _validationService;
    private readonly ILogger<ETLController> _logger;

    public ETLController(
        IETLPipelineService pipelineService,
        IValidationService validationService,
        ILogger<ETLController> logger)
    {
        _pipelineService = pipelineService;
        _validationService = validationService;
        _logger = logger;
    }

    [HttpPost("jobs")]
    public async Task<ActionResult<ApiResponseModel<ETLJobResponse>>> CreateJob([FromBody] ETLJobRequest request)
    {
        try
        {
            var job = await _pipelineService.CreateJobAsync(request);

            var response = new ETLJobResponse
            {
                JobId = job.JobId,
                Status = job.Status,
                Message = "ETL job created successfully",
                CreatedAt = job.CreatedAt
            };

            return Ok(ApiResponseModel<ETLJobResponse>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating ETL job");
            return BadRequest(ApiResponseModel<ETLJobResponse>.ErrorResponse(ex.Message));
        }
    }

    [HttpGet("jobs")]
    public async Task<ActionResult<ApiResponseModel<List<ETLJob>>>> GetAllJobs()
    {
        try
        {
            var jobs = await _pipelineService.GetAllJobsAsync();
            return Ok(ApiResponseModel<List<ETLJob>>.SuccessResponse(jobs));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving ETL jobs");
            return BadRequest(ApiResponseModel<List<ETLJob>>.ErrorResponse(ex.Message));
        }
    }

    [HttpGet("jobs/{jobId}")]
    public async Task<ActionResult<ApiResponseModel<ETLJob>>> GetJob(string jobId)
    {
        try
        {
            var job = await _pipelineService.GetJobAsync(jobId);
            return Ok(ApiResponseModel<ETLJob>.SuccessResponse(job));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponseModel<ETLJob>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving ETL job: {JobId}", jobId);
            return BadRequest(ApiResponseModel<ETLJob>.ErrorResponse(ex.Message));
        }
    }

    [HttpPost("jobs/{jobId}/execute")]
    public async Task<ActionResult<ApiResponseModel<ExecutePipelineResponse>>> ExecutePipeline(
        string jobId,
        [FromBody] ExecutePipelineRequest? request = null)
    {
        try
        {
            var job = await _pipelineService.GetJobAsync(jobId);

            if (request?.AsyncExecution ?? true)
            {
                // Execute asynchronously
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _pipelineService.ExecutePipelineAsync(job);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error in async pipeline execution for job: {JobId}", jobId);
                    }
                });

                var response = new ExecutePipelineResponse
                {
                    ExecutionId = Guid.NewGuid().ToString(),
                    Status = ETLJobStatus.Running,
                    Message = "ETL pipeline execution started asynchronously"
                };

                return Ok(ApiResponseModel<ExecutePipelineResponse>.SuccessResponse(response));
            }
            else
            {
                // Execute synchronously
                var execution = await _pipelineService.ExecutePipelineAsync(job);

                var response = new ExecutePipelineResponse
                {
                    ExecutionId = execution.ExecutionId,
                    Status = execution.Status,
                    Message = execution.Status == ETLJobStatus.Completed
                        ? "ETL pipeline completed successfully"
                        : "ETL pipeline execution failed",
                    Metrics = execution.Metrics
                };

                return Ok(ApiResponseModel<ExecutePipelineResponse>.SuccessResponse(response));
            }
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponseModel<ExecutePipelineResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing ETL pipeline: {JobId}", jobId);
            return BadRequest(ApiResponseModel<ExecutePipelineResponse>.ErrorResponse(ex.Message));
        }
    }

    [HttpPost("jobs/{jobId}/cancel")]
    public async Task<ActionResult<ApiResponseModel<bool>>> CancelJob(string jobId)
    {
        try
        {
            var result = await _pipelineService.CancelJobAsync(jobId);

            if (result)
            {
                return Ok(ApiResponseModel<bool>.SuccessResponse(true));
            }

            return BadRequest(ApiResponseModel<bool>.ErrorResponse("Job cannot be cancelled or not found"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling ETL job: {JobId}", jobId);
            return BadRequest(ApiResponseModel<bool>.ErrorResponse(ex.Message));
        }
    }

    [HttpPost("validate")]
    public async Task<ActionResult<ApiResponseModel<ValidationResult>>> ValidateData(
        [FromBody] ValidateDataRequest request)
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

    [HttpPost("jobs/{jobId}/quality-report")]
    public async Task<ActionResult<ApiResponseModel<DataQualityReport>>> GenerateQualityReport(
        string jobId,
        [FromBody] List<Dictionary<string, object>> data)
    {
        try
        {
            var report = await _validationService.GenerateQualityReportAsync(jobId, data);
            return Ok(ApiResponseModel<DataQualityReport>.SuccessResponse(report));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating quality report for job: {JobId}", jobId);
            return BadRequest(ApiResponseModel<DataQualityReport>.ErrorResponse(ex.Message));
        }
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Service = "ETL Pipeline Service"
        });
    }
}

// Additional request model
public class ValidateDataRequest
{
    public List<Dictionary<string, object>> Data { get; set; } = new();
    public List<ValidationRule> Rules { get; set; } = new();
}
