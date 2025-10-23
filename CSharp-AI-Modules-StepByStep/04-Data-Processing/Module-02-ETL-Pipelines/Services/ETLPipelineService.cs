using AI.ETL.Models;
using System.Diagnostics;

namespace AI.ETL.Services;

public interface IETLPipelineService
{
    Task<PipelineExecution> ExecutePipelineAsync(ETLJob job);
    Task<ETLJob> GetJobAsync(string jobId);
    Task<ETLJob> CreateJobAsync(ETLJobRequest request);
    Task<List<ETLJob>> GetAllJobsAsync();
    Task<bool> CancelJobAsync(string jobId);
}

public class ETLPipelineService : IETLPipelineService
{
    private readonly IExtractionService _extractionService;
    private readonly ITransformationService _transformationService;
    private readonly ILoadService _loadService;
    private readonly IValidationService _validationService;
    private readonly ILogger<ETLPipelineService> _logger;
    private static readonly Dictionary<string, ETLJob> _jobs = new();

    public ETLPipelineService(
        IExtractionService extractionService,
        ITransformationService transformationService,
        ILoadService loadService,
        IValidationService validationService,
        ILogger<ETLPipelineService> logger)
    {
        _extractionService = extractionService;
        _transformationService = transformationService;
        _loadService = loadService;
        _validationService = validationService;
        _logger = logger;
    }

    public async Task<PipelineExecution> ExecutePipelineAsync(ETLJob job)
    {
        var execution = new PipelineExecution
        {
            JobId = job.JobId,
            Status = ETLJobStatus.Running
        };

        var totalStopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("Starting ETL pipeline execution for job: {JobId}", job.JobId);

            job.Status = ETLJobStatus.Running;
            job.StartedAt = DateTime.UtcNow;

            // Stage 1: Extract
            var extractStage = new StageExecution
            {
                Name = "Extract",
                Status = ETLJobStatus.Running,
                StartTime = DateTime.UtcNow
            };
            execution.Stages.Add(extractStage);

            var extractStopwatch = Stopwatch.StartNew();
            var extractResult = await ExtractDataAsync(job.Configuration.Source);
            extractStopwatch.Stop();

            extractStage.EndTime = DateTime.UtcNow;
            extractStage.RecordsProcessed = extractResult.RecordsProcessed;
            extractStage.RecordsFailed = extractResult.RecordsFailed;
            extractStage.Status = extractResult.Success ? ETLJobStatus.Completed : ETLJobStatus.Failed;

            if (!extractResult.Success)
            {
                extractStage.ErrorMessage = string.Join("; ", extractResult.Errors.Select(e => e.ErrorMessage));
                throw new Exception($"Extraction failed: {extractStage.ErrorMessage}");
            }

            execution.Metrics.TotalRecordsExtracted = extractResult.RecordsProcessed;
            execution.Metrics.ExtractionTime = extractStopwatch.Elapsed;

            // Stage 2: Validate (if enabled)
            if (job.Configuration.Validation.Enabled)
            {
                var validateStage = new StageExecution
                {
                    Name = "Validate",
                    Status = ETLJobStatus.Running,
                    StartTime = DateTime.UtcNow
                };
                execution.Stages.Add(validateStage);

                var validationResult = await _validationService.ValidateDataAsync(
                    extractResult.Data,
                    job.Configuration.Validation.Rules);

                validateStage.EndTime = DateTime.UtcNow;
                validateStage.RecordsProcessed = validationResult.ValidRecords;
                validateStage.RecordsFailed = validationResult.InvalidRecords;
                validateStage.Status = validationResult.IsValid ? ETLJobStatus.Completed : ETLJobStatus.PartiallyCompleted;

                if (!validationResult.IsValid && job.Configuration.Validation.FailOnError)
                {
                    validateStage.ErrorMessage = $"Validation failed: {validationResult.Errors.Count} errors";
                    throw new Exception(validateStage.ErrorMessage);
                }

                // Filter out invalid records if configured
                if (job.Configuration.ErrorHandling.ContinueOnError)
                {
                    var invalidIndices = validationResult.Errors
                        .Select(e => e.RecordIndex)
                        .Distinct()
                        .OrderByDescending(i => i)
                        .ToList();

                    foreach (var index in invalidIndices)
                    {
                        if (index < extractResult.Data.Count)
                        {
                            extractResult.Data.RemoveAt(index);
                        }
                    }
                }
            }

            // Stage 3: Transform
            var transformStage = new StageExecution
            {
                Name = "Transform",
                Status = ETLJobStatus.Running,
                StartTime = DateTime.UtcNow
            };
            execution.Stages.Add(transformStage);

            var transformStopwatch = Stopwatch.StartNew();
            var transformResult = await _transformationService.ApplyTransformationsAsync(
                extractResult.Data,
                job.Configuration.Transformations);
            transformStopwatch.Stop();

            transformStage.EndTime = DateTime.UtcNow;
            transformStage.RecordsProcessed = transformResult.RecordsProcessed;
            transformStage.RecordsFailed = transformResult.RecordsFailed;
            transformStage.Status = transformResult.Success ? ETLJobStatus.Completed : ETLJobStatus.Failed;

            if (!transformResult.Success)
            {
                transformStage.ErrorMessage = string.Join("; ", transformResult.Errors.Select(e => e.ErrorMessage));
                throw new Exception($"Transformation failed: {transformStage.ErrorMessage}");
            }

            execution.Metrics.TotalRecordsTransformed = transformResult.RecordsProcessed;
            execution.Metrics.TransformationTime = transformStopwatch.Elapsed;

            // Stage 4: Load
            var loadStage = new StageExecution
            {
                Name = "Load",
                Status = ETLJobStatus.Running,
                StartTime = DateTime.UtcNow
            };
            execution.Stages.Add(loadStage);

            var loadStopwatch = Stopwatch.StartNew();
            var loadResult = await LoadDataAsync(transformResult.Data, job.Configuration.Destination);
            loadStopwatch.Stop();

            loadStage.EndTime = DateTime.UtcNow;
            loadStage.RecordsProcessed = loadResult.RecordsProcessed;
            loadStage.RecordsFailed = loadResult.RecordsFailed;
            loadStage.Status = loadResult.Success ? ETLJobStatus.Completed : ETLJobStatus.Failed;

            if (!loadResult.Success)
            {
                loadStage.ErrorMessage = string.Join("; ", loadResult.Errors.Select(e => e.ErrorMessage));
                throw new Exception($"Load failed: {loadStage.ErrorMessage}");
            }

            execution.Metrics.TotalRecordsLoaded = loadResult.RecordsProcessed;
            execution.Metrics.LoadTime = loadStopwatch.Elapsed;

            // Update job status
            job.Status = ETLJobStatus.Completed;
            job.CompletedAt = DateTime.UtcNow;
            job.ProcessedRecords = execution.Metrics.TotalRecordsLoaded;
            execution.Status = ETLJobStatus.Completed;

            _logger.LogInformation("ETL pipeline completed successfully for job: {JobId}", job.JobId);
        }
        catch (Exception ex)
        {
            job.Status = ETLJobStatus.Failed;
            job.ErrorMessage = ex.Message;
            job.CompletedAt = DateTime.UtcNow;
            execution.Status = ETLJobStatus.Failed;

            _logger.LogError(ex, "ETL pipeline failed for job: {JobId}", job.JobId);
        }
        finally
        {
            totalStopwatch.Stop();
            execution.EndTime = DateTime.UtcNow;
            execution.Duration = totalStopwatch.Elapsed;

            // Calculate metrics
            if (execution.Duration.TotalSeconds > 0)
            {
                execution.Metrics.RecordsPerSecond =
                    execution.Metrics.TotalRecordsLoaded / execution.Duration.TotalSeconds;
            }

            execution.Metrics.MemoryUsedMB = GC.GetTotalMemory(false) / (1024 * 1024);

            _jobs[job.JobId] = job;
        }

        return execution;
    }

    public async Task<ETLJob> GetJobAsync(string jobId)
    {
        await Task.CompletedTask;

        if (_jobs.TryGetValue(jobId, out var job))
        {
            return job;
        }

        throw new KeyNotFoundException($"Job with ID '{jobId}' not found");
    }

    public async Task<ETLJob> CreateJobAsync(ETLJobRequest request)
    {
        await Task.CompletedTask;

        var job = new ETLJob
        {
            Name = request.Name,
            Description = request.Description,
            Configuration = request.Configuration,
            Status = ETLJobStatus.Pending
        };

        _jobs[job.JobId] = job;

        _logger.LogInformation("Created ETL job: {JobId} - {Name}", job.JobId, job.Name);

        return job;
    }

    public async Task<List<ETLJob>> GetAllJobsAsync()
    {
        await Task.CompletedTask;
        return _jobs.Values.OrderByDescending(j => j.CreatedAt).ToList();
    }

    public async Task<bool> CancelJobAsync(string jobId)
    {
        await Task.CompletedTask;

        if (_jobs.TryGetValue(jobId, out var job))
        {
            if (job.Status == ETLJobStatus.Running)
            {
                job.Status = ETLJobStatus.Cancelled;
                job.CompletedAt = DateTime.UtcNow;
                _logger.LogInformation("Cancelled ETL job: {JobId}", jobId);
                return true;
            }
        }

        return false;
    }

    // Private helper methods
    private async Task<TransformationResult> ExtractDataAsync(DataSourceConfig config)
    {
        return config.Type switch
        {
            DataSourceType.SqlServer => await _extractionService.ExtractFromSqlServerAsync(config),
            DataSourceType.PostgreSQL => await _extractionService.ExtractFromPostgreSqlAsync(config),
            DataSourceType.MongoDB => await _extractionService.ExtractFromMongoDbAsync(config),
            DataSourceType.CsvFile => await _extractionService.ExtractFromCsvFileAsync(config.FilePath),
            DataSourceType.JsonFile => await _extractionService.ExtractFromJsonFileAsync(config.FilePath),
            _ => throw new NotSupportedException($"Data source type '{config.Type}' is not supported for extraction")
        };
    }

    private async Task<TransformationResult> LoadDataAsync(
        List<Dictionary<string, object>> data,
        DataSourceConfig config)
    {
        return config.Type switch
        {
            DataSourceType.SqlServer => await _loadService.LoadToSqlServerAsync(data, config),
            DataSourceType.PostgreSQL => await _loadService.LoadToPostgreSqlAsync(data, config),
            DataSourceType.MongoDB => await _loadService.LoadToMongoDbAsync(data, config),
            DataSourceType.CsvFile => await _loadService.LoadToCsvFileAsync(data, config.FilePath),
            DataSourceType.JsonFile => await _loadService.LoadToJsonFileAsync(data, config.FilePath),
            _ => throw new NotSupportedException($"Data source type '{config.Type}' is not supported for loading")
        };
    }
}
