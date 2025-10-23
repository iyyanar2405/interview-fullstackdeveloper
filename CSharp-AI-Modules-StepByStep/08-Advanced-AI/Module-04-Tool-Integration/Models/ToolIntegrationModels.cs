namespace Module_04_Tool_Integration.Models;

public static class ToolKinds
{
    public const string Api = "api";
    public const string Database = "database";
    public const string FileSystem = "filesystem";
    public const string WebScraping = "webscraping";
    public const string Custom = "custom";

    public static bool TryNormalize(string? value, out string normalized)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            normalized = string.Empty;
            return false;
        }

        normalized = value.Trim().ToLowerInvariant();
        return normalized is Api or Database or FileSystem or WebScraping or Custom;
    }
}

public sealed class ToolIntegrationOptions
{
    public List<ApiToolDefinition> ApiTools { get; set; } = new();
    public List<DatabaseToolDefinition> DatabaseTools { get; set; } = new();
    public List<FileSystemToolDefinition> FileSystemTools { get; set; } = new();
    public List<WebScrapingToolDefinition> WebScrapingTools { get; set; } = new();
    public List<CustomToolDefinition> CustomTools { get; set; } = new();
    public ToolExecutionSimulation Simulation { get; set; } = new();
}

public sealed class ToolExecutionSimulation
{
    public int WorkerIntervalMilliseconds { get; set; } = 500;
    public double FailureProbability { get; set; } = 0.05;
    public int MaxLogEntries { get; set; } = 250;
    public int MaxDataRecords { get; set; } = 250;
}

public sealed class ApiToolDefinition
{
    public string ToolId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public string Method { get; set; } = "GET";
    public List<string> SupportedOperations { get; set; } = new();
    public Dictionary<string, string> DefaultHeaders { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> SampleParameters { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public List<ApiResponseTemplate> ResponseTemplates { get; set; } = new();
}

public sealed class ApiResponseTemplate
{
    public string Operation { get; set; } = string.Empty;
    public Dictionary<string, string> Outputs { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public string Narrative { get; set; } = string.Empty;
}

public sealed class DatabaseToolDefinition
{
    public string ToolId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Engine { get; set; } = string.Empty;
    public List<string> SupportedOperations { get; set; } = new();
    public List<DatabaseTableDefinition> Tables { get; set; } = new();
}

public sealed class DatabaseTableDefinition
{
    public string TableName { get; set; } = string.Empty;
    public List<Dictionary<string, string>> Rows { get; set; } = new();
}

public sealed class FileSystemToolDefinition
{
    public string ToolId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string RootPath { get; set; } = string.Empty;
    public List<string> Capabilities { get; set; } = new();
    public List<FileSeedDefinition> SeedFiles { get; set; } = new();
}

public sealed class FileSeedDefinition
{
    public string Path { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

public sealed class WebScrapingToolDefinition
{
    public string ToolId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TargetUrl { get; set; } = string.Empty;
    public Dictionary<string, string> Selectors { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public List<ScrapedSampleRecord> Samples { get; set; } = new();
}

public sealed class ScrapedSampleRecord
{
    public string Selector { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTimeOffset CapturedAt { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class CustomToolDefinition
{
    public string ToolId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Parameters { get; set; } = new();
    public Dictionary<string, string> Behaviors { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> DefaultOutputs { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class ToolDescriptor
{
    public string ToolId { get; set; } = string.Empty;
    public string ToolType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public IReadOnlyCollection<string> Capabilities { get; set; } = Array.Empty<string>();
    public Dictionary<string, string> Metadata { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class ToolExecutionRequest
{
    public string ToolId { get; set; } = string.Empty;
    public string ToolType { get; set; } = string.Empty;
    public string RequestedBy { get; set; } = "system";
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString("N");
    public DateTimeOffset RequestedAt { get; set; } = DateTimeOffset.UtcNow;
    public Dictionary<string, string> Parameters { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public ToolExecutionRequest Clone()
    {
        return new ToolExecutionRequest
        {
            ToolId = ToolId,
            ToolType = ToolType,
            RequestedBy = RequestedBy,
            CorrelationId = CorrelationId,
            RequestedAt = RequestedAt,
            Parameters = new Dictionary<string, string>(Parameters, StringComparer.OrdinalIgnoreCase)
        };
    }
}

public sealed class ToolExecutionResult
{
    public bool Success { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public Dictionary<string, string> Outputs { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public TimeSpan Duration { get; set; } = TimeSpan.Zero;
    public DateTimeOffset CompletedAt { get; set; } = DateTimeOffset.UtcNow;

    public ToolExecutionResult Clone()
    {
        return new ToolExecutionResult
        {
            Success = Success,
            Status = Status,
            Summary = Summary,
            Outputs = new Dictionary<string, string>(Outputs, StringComparer.OrdinalIgnoreCase),
            Duration = Duration,
            CompletedAt = CompletedAt
        };
    }
}

public enum ToolRunStatus
{
    Pending,
    Running,
    Completed,
    Failed
}

public sealed class ToolRunLog
{
    public string RunId { get; set; } = string.Empty;
    public string ToolId { get; set; } = string.Empty;
    public string ToolType { get; set; } = string.Empty;
    public ToolExecutionRequest Request { get; set; } = new();
    public ToolExecutionResult? Result { get; set; }
    public ToolRunStatus Status { get; set; } = ToolRunStatus.Pending;
    public string Notes { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }

    public ToolRunLog Clone()
    {
        return new ToolRunLog
        {
            RunId = RunId,
            ToolId = ToolId,
            ToolType = ToolType,
            Request = Request.Clone(),
            Result = Result?.Clone(),
            Status = Status,
            Notes = Notes,
            CreatedAt = CreatedAt,
            StartedAt = StartedAt,
            CompletedAt = CompletedAt
        };
    }
}

public sealed class DataExtractRecord
{
    public string RunId { get; set; } = string.Empty;
    public string ToolId { get; set; } = string.Empty;
    public string ToolType { get; set; } = string.Empty;
    public Dictionary<string, string> Data { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public DateTimeOffset CapturedAt { get; set; } = DateTimeOffset.UtcNow;

    public DataExtractRecord Clone()
    {
        return new DataExtractRecord
        {
            RunId = RunId,
            ToolId = ToolId,
            ToolType = ToolType,
            Data = new Dictionary<string, string>(Data, StringComparer.OrdinalIgnoreCase),
            CapturedAt = CapturedAt
        };
    }
}

public sealed class FileArtifactRecord
{
    public string RunId { get; set; } = string.Empty;
    public string ToolId { get; set; } = string.Empty;
    public string ToolType { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public DateTimeOffset CapturedAt { get; set; } = DateTimeOffset.UtcNow;

    public FileArtifactRecord Clone()
    {
        return new FileArtifactRecord
        {
            RunId = RunId,
            ToolId = ToolId,
            ToolType = ToolType,
            FilePath = FilePath,
            Content = Content,
            Operation = Operation,
            CapturedAt = CapturedAt
        };
    }
}

public sealed class ToolExecutionOutcome
{
    public ToolExecutionResult Result { get; set; } = new();
    public List<DataExtractRecord> Extracts { get; set; } = new();
    public List<FileArtifactRecord> Artifacts { get; set; } = new();
}

public sealed class ToolExecutionWorkItem
{
    public ToolExecutionWorkItem(string runId, ToolExecutionRequest request)
    {
        RunId = runId;
        Request = request;
    }

    public string RunId { get; }
    public ToolExecutionRequest Request { get; }
}
