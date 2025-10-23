using Module_04_Tool_Integration.Models;

namespace Module_04_Tool_Integration.Services;

public sealed class ToolExecutionService
{
    private readonly ToolCatalogService _catalog;
    private readonly ToolExecutionQueueService _queue;
    private readonly ToolExecutionHistoryService _history;

    public ToolExecutionService(
        ToolCatalogService catalog,
        ToolExecutionQueueService queue,
        ToolExecutionHistoryService history)
    {
        _catalog = catalog;
        _queue = queue;
        _history = history;
    }

    public IReadOnlyCollection<ToolDescriptor> GetTools()
    {
        return _catalog.GetTools();
    }

    public ToolDescriptor? GetTool(string toolType, string toolId)
    {
        return _catalog.GetToolDescriptor(toolType, toolId);
    }

    public string SubmitExecution(ToolExecutionRequest request)
    {
        if (!ToolKinds.TryNormalize(request.ToolType, out _))
        {
            throw new InvalidOperationException($"Unsupported tool type '{request.ToolType}'.");
        }

        return _queue.Enqueue(request);
    }

    public IReadOnlyCollection<ToolRunLog> GetRuns()
    {
        return _history.GetRuns();
    }

    public ToolRunLog? GetRun(string runId)
    {
        return _history.GetRun(runId);
    }

    public IReadOnlyCollection<DataExtractRecord> GetExtracts()
    {
        return _history.GetExtracts();
    }

    public IReadOnlyCollection<FileArtifactRecord> GetArtifacts()
    {
        return _history.GetArtifacts();
    }
}
