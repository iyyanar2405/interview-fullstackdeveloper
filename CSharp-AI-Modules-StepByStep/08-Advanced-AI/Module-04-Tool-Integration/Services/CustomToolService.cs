using Module_04_Tool_Integration.Models;

namespace Module_04_Tool_Integration.Services;

public sealed class CustomToolService
{
    public async Task<ToolExecutionOutcome> ExecuteAsync(CustomToolDefinition tool, ToolExecutionRequest request, CancellationToken cancellationToken)
    {
        var startedAt = DateTimeOffset.UtcNow;
        await Task.Delay(Random.Shared.Next(120, 220), cancellationToken);

        var behaviorKey = request.Parameters.TryGetValue("behavior", out var behavior) ? behavior : string.Empty;
        var behaviorScript = ResolveBehavior(tool, behaviorKey);

        var outputs = new Dictionary<string, string>(tool.DefaultOutputs, StringComparer.OrdinalIgnoreCase)
        {
            ["behavior"] = behaviorScript.Key,
            ["summary"] = behaviorScript.Value
        };

        foreach (var parameter in request.Parameters)
        {
            outputs[$"param:{parameter.Key}"] = parameter.Value;
        }

        var completedAt = DateTimeOffset.UtcNow;
        var result = new ToolExecutionResult
        {
            Success = true,
            Status = "completed",
            Summary = $"Executed behavior '{behaviorScript.Key}' for {tool.Name}.",
            Outputs = outputs,
            Duration = completedAt - startedAt,
            CompletedAt = completedAt
        };

        return new ToolExecutionOutcome
        {
            Result = result
        };
    }

    private static KeyValuePair<string, string> ResolveBehavior(CustomToolDefinition tool, string behaviorKey)
    {
        if (!string.IsNullOrWhiteSpace(behaviorKey) && tool.Behaviors.TryGetValue(behaviorKey, out var script))
        {
            return new KeyValuePair<string, string>(behaviorKey, script);
        }

        if (tool.Behaviors.Count > 0)
        {
            var firstBehavior = tool.Behaviors.First();
            return new KeyValuePair<string, string>(firstBehavior.Key, firstBehavior.Value);
        }

        return new KeyValuePair<string, string>("default", "No behavior defined.");
    }
}
