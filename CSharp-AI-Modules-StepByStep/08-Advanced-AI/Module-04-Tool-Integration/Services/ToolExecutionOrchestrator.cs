using Module_04_Tool_Integration.Models;

namespace Module_04_Tool_Integration.Services;

public sealed class ToolExecutionOrchestrator
{
    private readonly ToolCatalogService _catalog;
    private readonly ToolExecutionHistoryService _history;
    private readonly ApiGatewayService _apiGateway;
    private readonly DatabaseExecutorService _databaseExecutor;
    private readonly FileSystemAutomationService _fileSystemAutomation;
    private readonly WebScrapingService _webScraping;
    private readonly CustomToolService _customToolService;

    public ToolExecutionOrchestrator(
        ToolCatalogService catalog,
        ToolExecutionHistoryService history,
        ApiGatewayService apiGateway,
        DatabaseExecutorService databaseExecutor,
        FileSystemAutomationService fileSystemAutomation,
        WebScrapingService webScraping,
        CustomToolService customToolService)
    {
        _catalog = catalog;
        _history = history;
        _apiGateway = apiGateway;
        _databaseExecutor = databaseExecutor;
        _fileSystemAutomation = fileSystemAutomation;
        _webScraping = webScraping;
        _customToolService = customToolService;
    }

    public async Task ExecuteAsync(ToolExecutionWorkItem workItem, CancellationToken cancellationToken)
    {
        _history.MarkRunning(workItem.RunId);

        try
        {
            var request = workItem.Request;
            var toolType = request.ToolType;

            if (!ToolKinds.TryNormalize(toolType, out var normalizedType))
            {
                throw new InvalidOperationException($"Unsupported tool type '{toolType}'.");
            }

            ToolExecutionOutcome outcome = normalizedType switch
            {
                ToolKinds.Api => await ExecuteApiAsync(request, cancellationToken),
                ToolKinds.Database => await ExecuteDatabaseAsync(request, cancellationToken),
                ToolKinds.FileSystem => await ExecuteFileSystemAsync(request, cancellationToken),
                ToolKinds.WebScraping => await ExecuteWebScrapingAsync(request, cancellationToken),
                ToolKinds.Custom => await ExecuteCustomAsync(request, cancellationToken),
                _ => throw new InvalidOperationException($"Unknown tool type '{toolType}'.")
            };

            _history.CompleteSuccess(workItem.RunId, outcome);
        }
        catch (Exception ex)
        {
            _history.CompleteFailure(workItem.RunId, ex.Message);
        }
    }

    private async Task<ToolExecutionOutcome> ExecuteApiAsync(ToolExecutionRequest request, CancellationToken cancellationToken)
    {
        var tool = _catalog.GetApiTool(request.ToolId);
        if (tool is null)
        {
            throw new InvalidOperationException($"API tool '{request.ToolId}' not found.");
        }

        return await _apiGateway.ExecuteAsync(tool, request, cancellationToken);
    }

    private async Task<ToolExecutionOutcome> ExecuteDatabaseAsync(ToolExecutionRequest request, CancellationToken cancellationToken)
    {
        var tool = _catalog.GetDatabaseTool(request.ToolId);
        if (tool is null)
        {
            throw new InvalidOperationException($"Database tool '{request.ToolId}' not found.");
        }

        return await _databaseExecutor.ExecuteAsync(tool, request, cancellationToken);
    }

    private async Task<ToolExecutionOutcome> ExecuteFileSystemAsync(ToolExecutionRequest request, CancellationToken cancellationToken)
    {
        var tool = _catalog.GetFileSystemTool(request.ToolId);
        if (tool is null)
        {
            throw new InvalidOperationException($"File system tool '{request.ToolId}' not found.");
        }

        return await _fileSystemAutomation.ExecuteAsync(tool, request, cancellationToken);
    }

    private async Task<ToolExecutionOutcome> ExecuteWebScrapingAsync(ToolExecutionRequest request, CancellationToken cancellationToken)
    {
        var tool = _catalog.GetWebScrapingTool(request.ToolId);
        if (tool is null)
        {
            throw new InvalidOperationException($"Web scraping tool '{request.ToolId}' not found.");
        }

        return await _webScraping.ExecuteAsync(tool, request, cancellationToken);
    }

    private async Task<ToolExecutionOutcome> ExecuteCustomAsync(ToolExecutionRequest request, CancellationToken cancellationToken)
    {
        var tool = _catalog.GetCustomTool(request.ToolId);
        if (tool is null)
        {
            throw new InvalidOperationException($"Custom tool '{request.ToolId}' not found.");
        }

        return await _customToolService.ExecuteAsync(tool, request, cancellationToken);
    }
}
