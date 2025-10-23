using System.Collections.Concurrent;
using Module_05_Alerting_Systems.Models;

namespace Module_05_Alerting_Systems.Services;

public sealed class MetricRepository
{
    private readonly ConcurrentDictionary<string, ConcurrentQueue<MetricSample>> _metrics = new(StringComparer.OrdinalIgnoreCase);

    public void AddSample(MetricSample sample)
    {
        var queue = _metrics.GetOrAdd(sample.Metric, _ => new ConcurrentQueue<MetricSample>());
        queue.Enqueue(sample);
    }

    public IReadOnlyCollection<MetricSample> GetSamples(string metric, TimeSpan window)
    {
        if (!_metrics.TryGetValue(metric, out var queue))
        {
            return Array.Empty<MetricSample>();
        }

        var cutoff = DateTimeOffset.UtcNow - window;
        return queue.Where(s => s.Timestamp >= cutoff).ToArray();
    }

    public void Prune(TimeSpan retention)
    {
        var cutoff = DateTimeOffset.UtcNow - retention;
        foreach (var queue in _metrics.Values)
        {
            while (queue.TryPeek(out var sample) && sample.Timestamp < cutoff)
            {
                queue.TryDequeue(out _);
            }
        }
    }
}
