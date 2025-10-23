using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Module_01_Agentic_AI.Models;

namespace Module_01_Agentic_AI.Services;

public sealed class ToolCatalogService
{
    private readonly ConcurrentDictionary<string, ToolDefinition> _tools = new(StringComparer.OrdinalIgnoreCase);
    private readonly SimulationSettings _settings;
    private readonly ILogger<ToolCatalogService> _logger;
    private readonly Random _random = new();

    public ToolCatalogService(IOptions<AgenticOptions> options, ILogger<ToolCatalogService> logger)
    {
        _logger = logger;
        _settings = options.Value.Simulation;
        foreach (var tool in options.Value.Tools)
        {
            _tools[tool.Name] = Clone(tool);
        }
    }

    public IReadOnlyCollection<ToolDefinition> GetTools()
    {
        return _tools.Values.Select(Clone).ToArray();
    }

    public ToolDefinition? GetTool(string name)
    {
        return _tools.TryGetValue(name, out var tool) ? Clone(tool) : null;
    }

    public async Task<ToolInvocationResult> ExecuteAsync(string toolName, string input, CancellationToken cancellationToken)
    {
        if (!_tools.TryGetValue(toolName, out var tool))
        {
            _logger.LogWarning("Tool {Tool} not found", toolName);
            return new ToolInvocationResult { Success = false, Output = "Tool not available" };
        }

        await Task.Delay(_settings.ToolDelayMilliseconds, cancellationToken);
        var success = _random.NextDouble() > 0.05; // optimistic default
        var output = success
            ? $"Executed {toolName} with input length {input.Length}"
            : $"{toolName} encountered a transient error";

        var artifacts = new List<ExecutionArtifact>
        {
            new ExecutionArtifact
            {
                Type = "log",
                Description = $"{toolName} invocation trace",
                Data = $"{{\"timestamp\":\"{DateTimeOffset.UtcNow:o}\",\"status\":\"{(success ? "success" : "error")}\"}}"
            }
        };

        return new ToolInvocationResult
        {
            Success = success,
            Output = output,
            Artifacts = artifacts,
            Reasoning = success ? "Tool executed using simulated pipeline" : "Simulated failure for resilience testing"
        };
    }

    private static ToolDefinition Clone(ToolDefinition tool)
    {
        return new ToolDefinition
        {
            Name = tool.Name,
            Description = tool.Description,
            InputSchema = tool.InputSchema,
            OutputSchema = tool.OutputSchema,
            SafetyGuidelines = tool.SafetyGuidelines.ToList(),
            Tags = tool.Tags.ToList()
        };
    }
}
