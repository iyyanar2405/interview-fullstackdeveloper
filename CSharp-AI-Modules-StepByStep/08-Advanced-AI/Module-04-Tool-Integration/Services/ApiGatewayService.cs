using Module_04_Tool_Integration.Models;

namespace Module_04_Tool_Integration.Services;

public sealed class ApiGatewayService
{
    public async Task<ToolExecutionOutcome> ExecuteAsync(ApiToolDefinition tool, ToolExecutionRequest request, CancellationToken cancellationToken)
    {
        var startedAt = DateTimeOffset.UtcNow;
        await Task.Delay(Random.Shared.Next(100, 250), cancellationToken);

        var operation = ResolveOperation(tool, request);
        var template = tool.ResponseTemplates.FirstOrDefault(t => string.Equals(t.Operation, operation, StringComparison.OrdinalIgnoreCase));

        var outputs = template != null
            ? new Dictionary<string, string>(template.Outputs, StringComparer.OrdinalIgnoreCase)
            : new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["operation"] = operation,
                ["status"] = "simulated"
            };

        var endpointPath = request.Parameters.TryGetValue("path", out var path) ? path : string.Empty;
        var query = request.Parameters.TryGetValue("query", out var queryString) ? queryString : string.Empty;
        var composedEndpoint = ComposeEndpoint(tool.BaseUrl, endpointPath, query);

        outputs["endpoint"] = composedEndpoint;
        outputs["method"] = tool.Method.ToUpperInvariant();
        outputs["requestedBy"] = request.RequestedBy;

        foreach (var parameter in request.Parameters)
        {
            outputs[$"param:{parameter.Key}"] = parameter.Value;
        }

        var summary = template?.Narrative ?? $"Simulated {tool.Name} {operation} call completed.";
        var completedAt = DateTimeOffset.UtcNow;

        var result = new ToolExecutionResult
        {
            Success = true,
            Status = "completed",
            Summary = summary,
            Outputs = outputs,
            Duration = completedAt - startedAt,
            CompletedAt = completedAt
        };

        var outcome = new ToolExecutionOutcome
        {
            Result = result
        };

        outcome.Extracts.Add(new DataExtractRecord
        {
            RunId = string.Empty,
            ToolId = tool.ToolId,
            ToolType = ToolKinds.Api,
            Data = new Dictionary<string, string>(outputs, StringComparer.OrdinalIgnoreCase)
            {
                ["response.summary"] = summary
            },
            CapturedAt = completedAt
        });

        return outcome;
    }

    private static string ResolveOperation(ApiToolDefinition tool, ToolExecutionRequest request)
    {
        if (request.Parameters.TryGetValue("operation", out var op) && !string.IsNullOrWhiteSpace(op))
        {
            return op;
        }

        if (tool.SupportedOperations.Count > 0)
        {
            return tool.SupportedOperations[0];
        }

        return "default";
    }

    private static string ComposeEndpoint(string baseUrl, string path, string query)
    {
        var trimmedBase = baseUrl.TrimEnd('/');
        var trimmedPath = path.TrimStart('/');
        var endpoint = string.IsNullOrWhiteSpace(trimmedPath) ? trimmedBase : $"{trimmedBase}/{trimmedPath}";
        if (string.IsNullOrWhiteSpace(query))
        {
            return endpoint;
        }

        if (query.Contains("?", StringComparison.Ordinal))
        {
            var parts = query.Split('?', 2, StringSplitOptions.RemoveEmptyEntries);
            var querySegment = parts.Length == 2 ? parts[1] : query;
            return $"{endpoint}?{querySegment}";
        }

        return $"{endpoint}?{query}";
    }
}
