using System.Collections.Concurrent;
using Module_05_Alerting_Systems.Models;
using Microsoft.Extensions.Options;

namespace Module_05_Alerting_Systems.Services;

public sealed class AlertHistoryService
{
    private readonly ConcurrentQueue<AlertNotification> _history = new();
    private readonly int _maxSize;

    public AlertHistoryService(IOptions<AlertingOptions> options)
    {
        _maxSize = Math.Max(500, options.Value.HistorySize);
    }

    public void Record(AlertNotification notification)
    {
        _history.Enqueue(notification);
        while (_history.Count > _maxSize && _history.TryDequeue(out _))
        {
        }
    }

    public IReadOnlyCollection<AlertNotification> GetRecent(int count)
    {
        return _history.Reverse().Take(count).ToArray();
    }

    public AlertSummary GetSummary()
    {
        var now = DateTimeOffset.UtcNow;
        var lastHour = _history.Count(a => (now - a.TriggeredAt) <= TimeSpan.FromHours(1));
        var last24 = _history.Count(a => (now - a.TriggeredAt) <= TimeSpan.FromHours(24));

        return new AlertSummary
        {
            AlertsLastHour = lastHour,
            AlertsLast24Hours = last24,
            GeneratedAt = now
        };
    }
}
