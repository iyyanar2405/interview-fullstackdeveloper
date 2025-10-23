using Module_04_Tool_Integration.Models;

namespace Module_04_Tool_Integration.Services;

public sealed class DatabaseExecutorService
{
    public async Task<ToolExecutionOutcome> ExecuteAsync(DatabaseToolDefinition tool, ToolExecutionRequest request, CancellationToken cancellationToken)
    {
        var startedAt = DateTimeOffset.UtcNow;
        await Task.Delay(Random.Shared.Next(150, 350), cancellationToken);

        var tableName = ResolveTable(tool, request);
        var table = tool.Tables.FirstOrDefault(t => string.Equals(t.TableName, tableName, StringComparison.OrdinalIgnoreCase));
        if (table is null)
        {
            throw new InvalidOperationException($"Table '{tableName}' not defined for tool '{tool.ToolId}'.");
        }

        var operation = request.Parameters.TryGetValue("operation", out var op) ? op : "select";
        var filterKey = request.Parameters.TryGetValue("filterKey", out var key) ? key : string.Empty;
        var filterValue = request.Parameters.TryGetValue("filterValue", out var value) ? value : string.Empty;

        var filteredRows = ApplyFilter(table.Rows, filterKey, filterValue);
        var rowCount = filteredRows.Count;

        var outputs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["operation"] = operation,
            ["table"] = table.TableName,
            ["rowCount"] = rowCount.ToString(),
            ["requestedBy"] = request.RequestedBy
        };

        if (!string.IsNullOrWhiteSpace(filterKey))
        {
            outputs["filterKey"] = filterKey;
            outputs["filterValue"] = filterValue;
        }

        var completedAt = DateTimeOffset.UtcNow;
        var result = new ToolExecutionResult
        {
            Success = true,
            Status = "completed",
            Summary = $"Retrieved {rowCount} rows from {table.TableName}.",
            Outputs = outputs,
            Duration = completedAt - startedAt,
            CompletedAt = completedAt
        };

        var outcome = new ToolExecutionOutcome
        {
            Result = result
        };

        foreach (var row in filteredRows.Take(5))
        {
            outcome.Extracts.Add(new DataExtractRecord
            {
                RunId = string.Empty,
                ToolId = tool.ToolId,
                ToolType = ToolKinds.Database,
                Data = new Dictionary<string, string>(row, StringComparer.OrdinalIgnoreCase),
                CapturedAt = completedAt
            });
        }

        return outcome;
    }

    private static string ResolveTable(DatabaseToolDefinition tool, ToolExecutionRequest request)
    {
        if (request.Parameters.TryGetValue("table", out var table) && !string.IsNullOrWhiteSpace(table))
        {
            return table;
        }

        return tool.Tables.Count > 0 ? tool.Tables[0].TableName : "unknown";
    }

    private static List<Dictionary<string, string>> ApplyFilter(IEnumerable<Dictionary<string, string>> rows, string filterKey, string filterValue)
    {
        if (string.IsNullOrWhiteSpace(filterKey) || string.IsNullOrWhiteSpace(filterValue))
        {
            return rows.Select(row => new Dictionary<string, string>(row, StringComparer.OrdinalIgnoreCase)).ToList();
        }

        return rows
            .Where(row => row.TryGetValue(filterKey, out var value) && value.Contains(filterValue, StringComparison.OrdinalIgnoreCase))
            .Select(row => new Dictionary<string, string>(row, StringComparer.OrdinalIgnoreCase))
            .ToList();
    }
}
