using Module_04_Tool_Integration.Models;

namespace Module_04_Tool_Integration.Services;

public sealed class WebScrapingService
{
    public async Task<ToolExecutionOutcome> ExecuteAsync(WebScrapingToolDefinition tool, ToolExecutionRequest request, CancellationToken cancellationToken)
    {
        var startedAt = DateTimeOffset.UtcNow;
        await Task.Delay(Random.Shared.Next(200, 400), cancellationToken);

        var selector = request.Parameters.TryGetValue("selector", out var selected) ? selected : string.Empty;
        var samples = FilterSamples(tool, selector);
        var completedAt = DateTimeOffset.UtcNow;

        var outputs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["operation"] = "scrape",
            ["targetUrl"] = tool.TargetUrl,
            ["selector"] = string.IsNullOrWhiteSpace(selector) ? "*" : selector,
            ["sampleCount"] = samples.Count.ToString()
        };

        var result = new ToolExecutionResult
        {
            Success = true,
            Status = "completed",
            Summary = $"Retrieved {samples.Count} sample entries from {tool.Name}.",
            Outputs = outputs,
            Duration = completedAt - startedAt,
            CompletedAt = completedAt
        };

        var outcome = new ToolExecutionOutcome
        {
            Result = result
        };

        foreach (var sample in samples.Take(10))
        {
            outcome.Extracts.Add(new DataExtractRecord
            {
                RunId = string.Empty,
                ToolId = tool.ToolId,
                ToolType = ToolKinds.WebScraping,
                Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["selector"] = sample.Selector,
                    ["content"] = sample.Content,
                    ["capturedAt"] = sample.CapturedAt.ToString("O")
                },
                CapturedAt = completedAt
            });
        }

        return outcome;
    }

    private static List<ScrapedSampleRecord> FilterSamples(WebScrapingToolDefinition tool, string selector)
    {
        if (string.IsNullOrWhiteSpace(selector))
        {
            return tool.Samples.Select(sample => new ScrapedSampleRecord
            {
                Selector = sample.Selector,
                Content = sample.Content,
                CapturedAt = sample.CapturedAt
            }).ToList();
        }

        return tool.Samples
            .Where(sample => sample.Selector.Contains(selector, StringComparison.OrdinalIgnoreCase))
            .Select(sample => new ScrapedSampleRecord
            {
                Selector = sample.Selector,
                Content = sample.Content,
                CapturedAt = sample.CapturedAt
            })
            .ToList();
    }
}
