using Module_05_Memory_Systems.Models;

namespace Module_05_Memory_Systems.Services;

public sealed class MemoryAnalyticsService
{
    private readonly MemoryStoreService _storeService;

    public MemoryAnalyticsService(MemoryStoreService storeService)
    {
        _storeService = storeService;
    }

    public MemoryAnalyticsSnapshot GetSnapshot(string profileId)
    {
        var summary = _storeService.GetProfileSummary(profileId);
        var insights = new List<MemoryInsight>();

        if (summary.TotalEntries == 0)
        {
            insights.Add(new MemoryInsight
            {
                Category = "status",
                Description = "No memories ingested yet for this profile.",
                Score = 0
            });

            return new MemoryAnalyticsSnapshot
            {
                Summary = summary,
                Insights = insights
            };
        }

        var busiestStore = summary.Stores.OrderByDescending(store => store.TotalEntries).FirstOrDefault();
        if (busiestStore is not null)
        {
            insights.Add(new MemoryInsight
            {
                Category = "density",
                Description = $"Store '{busiestStore.StoreId}' holds {busiestStore.TotalEntries} entries.",
                Score = Math.Min(1.0, busiestStore.TotalEntries / 500.0)
            });
        }

        var freshestStore = summary.Stores.OrderByDescending(store => store.LastUpdated).FirstOrDefault();
        if (freshestStore is not null)
        {
            var age = (DateTimeOffset.UtcNow - freshestStore.LastUpdated).TotalMinutes;
            insights.Add(new MemoryInsight
            {
                Category = "freshness",
                Description = $"Most recently updated store '{freshestStore.StoreId}' {age:F0} minutes ago.",
                Score = Math.Max(0, 1.0 - age / 60.0)
            });
        }

        var tagCloud = summary.Stores
            .SelectMany(store => store.TagFrequency)
            .GroupBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase)
            .Select(group => new
            {
                Tag = group.Key,
                Count = group.Sum(pair => pair.Value)
            })
            .OrderByDescending(item => item.Count)
            .Take(5)
            .ToList();

        if (tagCloud.Count > 0)
        {
            insights.Add(new MemoryInsight
            {
                Category = "tag-cloud",
                Description = $"Top tags: {string.Join(", ", tagCloud.Select(item => $"{item.Tag} ({item.Count})"))}",
                Score = Math.Min(1.0, tagCloud.Sum(item => item.Count) / 100.0)
            });
        }

        return new MemoryAnalyticsSnapshot
        {
            Summary = summary,
            Insights = insights
        };
    }
}
