using System.Collections.Concurrent;
using System.IO;
using Module_04_Tool_Integration.Models;
using Microsoft.Extensions.Options;

namespace Module_04_Tool_Integration.Services;

public sealed class FileSystemAutomationService
{
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> _fileStores = new(StringComparer.OrdinalIgnoreCase);

    public FileSystemAutomationService(IOptions<ToolIntegrationOptions> options)
    {
        foreach (var tool in options.Value.FileSystemTools)
        {
            var store = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var seed in tool.SeedFiles)
            {
                store[NormalizePath(seed.Path)] = seed.Content;
            }

            _fileStores[tool.ToolId] = store;
        }
    }

    public async Task<ToolExecutionOutcome> ExecuteAsync(FileSystemToolDefinition tool, ToolExecutionRequest request, CancellationToken cancellationToken)
    {
        var store = _fileStores.GetOrAdd(tool.ToolId, _ => new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase));
        var operation = request.Parameters.TryGetValue("operation", out var op) ? op.ToLowerInvariant() : "list";
        var startedAt = DateTimeOffset.UtcNow;

        await Task.Delay(Random.Shared.Next(50, 150), cancellationToken);

        return operation switch
        {
            "read" => HandleRead(tool, request, store, startedAt),
            "write" => HandleWrite(tool, request, store, startedAt),
            "delete" => HandleDelete(tool, request, store, startedAt),
            _ => HandleList(tool, request, store, startedAt)
        };
    }

    private static ToolExecutionOutcome HandleList(FileSystemToolDefinition tool, ToolExecutionRequest request, ConcurrentDictionary<string, string> store, DateTimeOffset startedAt)
    {
        var files = store.Keys.OrderBy(path => path).ToArray();
        var completedAt = DateTimeOffset.UtcNow;

        var outputs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["operation"] = "list",
            ["rootPath"] = tool.RootPath,
            ["fileCount"] = files.Length.ToString()
        };

        for (var i = 0; i < files.Length && i < 5; i++)
        {
            outputs[$"file[{i}]"] = files[i];
        }

        var result = new ToolExecutionResult
        {
            Success = true,
            Status = "completed",
            Summary = $"Listed {files.Length} files under {tool.RootPath}.",
            Outputs = outputs,
            Duration = completedAt - startedAt,
            CompletedAt = completedAt
        };

        return new ToolExecutionOutcome
        {
            Result = result
        };
    }

    private static ToolExecutionOutcome HandleRead(FileSystemToolDefinition tool, ToolExecutionRequest request, ConcurrentDictionary<string, string> store, DateTimeOffset startedAt)
    {
        if (!request.Parameters.TryGetValue("filePath", out var filePath))
        {
            throw new InvalidOperationException("File path is required for read operation.");
        }

        var normalizedPath = NormalizePath(filePath);
        if (!store.TryGetValue(normalizedPath, out var content))
        {
            throw new FileNotFoundException($"File '{normalizedPath}' not found in virtual store.");
        }

        var completedAt = DateTimeOffset.UtcNow;
        var result = new ToolExecutionResult
        {
            Success = true,
            Status = "completed",
            Summary = $"Read file {normalizedPath}.",
            Outputs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["operation"] = "read",
                ["filePath"] = normalizedPath,
                ["length"] = content.Length.ToString()
            },
            Duration = completedAt - startedAt,
            CompletedAt = completedAt
        };

        var outcome = new ToolExecutionOutcome
        {
            Result = result
        };

        outcome.Artifacts.Add(new FileArtifactRecord
        {
            RunId = string.Empty,
            ToolId = tool.ToolId,
            ToolType = ToolKinds.FileSystem,
            FilePath = normalizedPath,
            Content = content,
            Operation = "read",
            CapturedAt = completedAt
        });

        return outcome;
    }

    private static ToolExecutionOutcome HandleWrite(FileSystemToolDefinition tool, ToolExecutionRequest request, ConcurrentDictionary<string, string> store, DateTimeOffset startedAt)
    {
        if (!request.Parameters.TryGetValue("filePath", out var filePath))
        {
            throw new InvalidOperationException("File path is required for write operation.");
        }

        var normalizedPath = NormalizePath(filePath);
        var content = request.Parameters.TryGetValue("content", out var providedContent) ? providedContent : string.Empty;
        store[normalizedPath] = content;

        var completedAt = DateTimeOffset.UtcNow;
        var outputs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["operation"] = "write",
            ["filePath"] = normalizedPath,
            ["length"] = content.Length.ToString()
        };

        var result = new ToolExecutionResult
        {
            Success = true,
            Status = "completed",
            Summary = $"Wrote {content.Length} characters to {normalizedPath}.",
            Outputs = outputs,
            Duration = completedAt - startedAt,
            CompletedAt = completedAt
        };

        var outcome = new ToolExecutionOutcome
        {
            Result = result
        };

        outcome.Artifacts.Add(new FileArtifactRecord
        {
            RunId = string.Empty,
            ToolId = tool.ToolId,
            ToolType = ToolKinds.FileSystem,
            FilePath = normalizedPath,
            Content = content,
            Operation = "write",
            CapturedAt = completedAt
        });

        return outcome;
    }

    private static ToolExecutionOutcome HandleDelete(FileSystemToolDefinition tool, ToolExecutionRequest request, ConcurrentDictionary<string, string> store, DateTimeOffset startedAt)
    {
        if (!request.Parameters.TryGetValue("filePath", out var filePath))
        {
            throw new InvalidOperationException("File path is required for delete operation.");
        }

        var normalizedPath = NormalizePath(filePath);
        store.TryRemove(normalizedPath, out _);

        var completedAt = DateTimeOffset.UtcNow;
        var outputs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["operation"] = "delete",
            ["filePath"] = normalizedPath
        };

        var result = new ToolExecutionResult
        {
            Success = true,
            Status = "completed",
            Summary = $"Deleted file {normalizedPath} if it existed.",
            Outputs = outputs,
            Duration = completedAt - startedAt,
            CompletedAt = completedAt
        };

        return new ToolExecutionOutcome
        {
            Result = result
        };
    }

    private static string NormalizePath(string path)
    {
        return path.Replace('\\', '/').Trim();
    }
}
