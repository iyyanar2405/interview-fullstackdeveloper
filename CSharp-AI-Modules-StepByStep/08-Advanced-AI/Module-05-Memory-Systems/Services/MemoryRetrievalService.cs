using Module_05_Memory_Systems.Models;

namespace Module_05_Memory_Systems.Services;

public sealed class MemoryRetrievalService
{
    private readonly MemoryStoreService _storeService;
    private readonly MemoryProfileCatalogService _catalog;

    public MemoryRetrievalService(MemoryStoreService storeService, MemoryProfileCatalogService catalog)
    {
        _storeService = storeService;
        _catalog = catalog;
    }

    public MemoryRetrievalResponse RetrieveMemories(MemoryRetrievalRequest request)
    {
        var entries = _storeService.QueryMemories(request);
        if (entries.Count == 0)
        {
            return new MemoryRetrievalResponse
            {
                ProfileId = request.ProfileId
            };
        }

        var ranked = entries
            .Select(entry => new
            {
                Entry = entry,
                Score = ScoreEntry(entry, request)
            })
            .OrderByDescending(item => item.Score)
            .ThenByDescending(item => item.Entry.Importance)
            .ThenByDescending(item => item.Entry.CreatedAt)
            .Take(Math.Max(1, request.Limit))
            .ToList();

        foreach (var item in ranked)
        {
            _storeService.RegisterRecall(item.Entry.EntryId);
        }

        var response = new MemoryRetrievalResponse
        {
            ProfileId = request.ProfileId,
            Entries = ranked.Select(item => item.Entry).ToList(),
            Insights = BuildInsights(ranked.Select(item => item.Entry).ToList(), request)
        };

        return response;
    }

    private static double ScoreEntry(MemoryEntry entry, MemoryRetrievalRequest request)
    {
        double score = entry.Importance;

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            if (entry.Content.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase) ||
                entry.Title.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase))
            {
                score += 0.6;
            }
        }

        if (request.Tags.Count > 0)
        {
            var matches = entry.Tags.Intersect(request.Tags, StringComparer.OrdinalIgnoreCase).Count();
            score += matches * 0.25;
        }

        var recencyBoost = Math.Max(0, 1.0 - (DateTimeOffset.UtcNow - entry.CreatedAt).TotalHours / 24.0);
        score += recencyBoost * 0.2;

        score += entry.RecallCount * 0.05;
        return score;
    }

    private List<MemoryInsight> BuildInsights(IReadOnlyCollection<MemoryEntry> entries, MemoryRetrievalRequest request)
    {
        var insights = new List<MemoryInsight>();
        if (entries.Count == 0)
        {
            return insights;
        }

        var averageImportance = entries.Average(entry => entry.Importance);
        insights.Add(new MemoryInsight
        {
            Category = "relevance",
            Description = $"Average importance score for this retrieval: {averageImportance:F2}",
            Score = averageImportance
        });

        var tagMatches = entries.SelectMany(entry => entry.Tags).Intersect(request.Tags, StringComparer.OrdinalIgnoreCase).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        if (tagMatches.Count > 0)
        {
            insights.Add(new MemoryInsight
            {
                Category = "tag-match",
                Description = $"Matched tags: {string.Join(", ", tagMatches)}",
                Score = Math.Min(1.0, tagMatches.Count * 0.2)
            });
        }

        var profile = _catalog.GetProfile(request.ProfileId);
        if (profile is not null)
        {
            insights.Add(new MemoryInsight
            {
                Category = "strategy",
                Description = $"Applied retrieval strategies: {string.Join(", ", profile.RetrievalStrategies)}",
                Score = profile.RetrievalStrategies.Count * 0.1
            });
        }

        return insights;
    }
}
