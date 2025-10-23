using Module_06_Monitoring_Alerts.Models;
using System.Collections.Concurrent;

namespace Module_06_Monitoring_Alerts.Services;

/// <summary>
/// Service for monitoring and tracking security events
/// </summary>
public class EventMonitoringService
{
    private readonly ConcurrentDictionary<string, SecurityEvent> _events = new();
    private readonly ConcurrentQueue<SecurityEvent> _eventStream = new();
    private const int MaxRetainedEvents = 10000;

    /// <summary>
    /// Log a security event
    /// </summary>
    public async Task<SecurityEvent> LogEventAsync(SecurityEvent securityEvent)
    {
        await Task.CompletedTask;

        // Assign correlation ID if not present
        securityEvent.CorrelationId ??= GenerateCorrelationId(securityEvent);

        // Store event
        _events[securityEvent.EventId] = securityEvent;
        _eventStream.Enqueue(securityEvent);

        // Maintain size limit
        if (_eventStream.Count > MaxRetainedEvents)
        {
            _eventStream.TryDequeue(out _);
        }

        return securityEvent;
    }

    /// <summary>
    /// Query events with filters
    /// </summary>
    public async Task<List<SecurityEvent>> QueryEventsAsync(EventQuery query)
    {
        await Task.CompletedTask;

        var events = _events.Values.AsEnumerable();

        // Apply filters
        if (query.StartTime.HasValue)
            events = events.Where(e => e.Timestamp >= query.StartTime.Value);

        if (query.EndTime.HasValue)
            events = events.Where(e => e.Timestamp <= query.EndTime.Value);

        if (query.Type.HasValue)
            events = events.Where(e => e.Type == query.Type.Value);

        if (query.MinSeverity.HasValue)
            events = events.Where(e => e.Severity >= query.MinSeverity.Value);

        if (!string.IsNullOrEmpty(query.UserId))
            events = events.Where(e => e.UserId == query.UserId);

        if (!string.IsNullOrEmpty(query.IpAddress))
            events = events.Where(e => e.IpAddress == query.IpAddress);

        if (query.AnomaliesOnly == true)
            events = events.Where(e => e.IsAnomaly);

        // Pagination
        var result = events
            .OrderByDescending(e => e.Timestamp)
            .Skip(query.PageNumber * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        return result;
    }

    /// <summary>
    /// Get a specific event by ID
    /// </summary>
    public async Task<SecurityEvent?> GetEventAsync(string eventId)
    {
        await Task.CompletedTask;
        _events.TryGetValue(eventId, out var securityEvent);
        return securityEvent;
    }

    /// <summary>
    /// Get correlated events (events with same correlation ID)
    /// </summary>
    public async Task<List<SecurityEvent>> GetCorrelatedEventsAsync(string correlationId)
    {
        await Task.CompletedTask;

        return _events.Values
            .Where(e => e.CorrelationId == correlationId)
            .OrderBy(e => e.Timestamp)
            .ToList();
    }

    /// <summary>
    /// Get event statistics
    /// </summary>
    public async Task<Dictionary<string, int>> GetEventStatisticsAsync(DateTime startTime, DateTime endTime)
    {
        await Task.CompletedTask;

        var events = _events.Values
            .Where(e => e.Timestamp >= startTime && e.Timestamp <= endTime);

        var statistics = new Dictionary<string, int>
        {
            ["TotalEvents"] = events.Count(),
            ["CriticalEvents"] = events.Count(e => e.Severity == EventSeverity.Critical),
            ["ErrorEvents"] = events.Count(e => e.Severity == EventSeverity.Error),
            ["WarningEvents"] = events.Count(e => e.Severity == EventSeverity.Warning),
            ["Anomalies"] = events.Count(e => e.IsAnomaly)
        };

        // Count by event type
        foreach (var type in Enum.GetValues<EventType>())
        {
            statistics[$"Type_{type}"] = events.Count(e => e.Type == type);
        }

        return statistics;
    }

    /// <summary>
    /// Generate audit trail for a user
    /// </summary>
    public async Task<List<SecurityEvent>> GenerateAuditTrailAsync(string userId, DateTime startTime, DateTime endTime)
    {
        await Task.CompletedTask;

        return _events.Values
            .Where(e => e.UserId == userId &&
                       e.Timestamp >= startTime &&
                       e.Timestamp <= endTime)
            .OrderBy(e => e.Timestamp)
            .ToList();
    }

    /// <summary>
    /// Get recent events stream
    /// </summary>
    public async Task<List<SecurityEvent>> GetRecentEventsAsync(int count = 100)
    {
        await Task.CompletedTask;
        return _eventStream.TakeLast(count).Reverse().ToList();
    }

    /// <summary>
    /// Search events by message content
    /// </summary>
    public async Task<List<SecurityEvent>> SearchEventsAsync(string searchTerm, int maxResults = 100)
    {
        await Task.CompletedTask;

        return _events.Values
            .Where(e => e.Message.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                       e.Source.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(e => e.Timestamp)
            .Take(maxResults)
            .ToList();
    }

    /// <summary>
    /// Get events by severity
    /// </summary>
    public async Task<List<SecurityEvent>> GetEventsBySeverityAsync(EventSeverity severity, int count = 50)
    {
        await Task.CompletedTask;

        return _events.Values
            .Where(e => e.Severity == severity)
            .OrderByDescending(e => e.Timestamp)
            .Take(count)
            .ToList();
    }

    /// <summary>
    /// Delete old events
    /// </summary>
    public async Task<int> DeleteOldEventsAsync(int retentionDays)
    {
        await Task.CompletedTask;

        var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);
        var eventsToDelete = _events.Values
            .Where(e => e.Timestamp < cutoffDate)
            .Select(e => e.EventId)
            .ToList();

        int deletedCount = 0;
        foreach (var eventId in eventsToDelete)
        {
            if (_events.TryRemove(eventId, out _))
                deletedCount++;
        }

        return deletedCount;
    }

    /// <summary>
    /// Export events to JSON
    /// </summary>
    public async Task<string> ExportEventsAsync(EventQuery query)
    {
        var events = await QueryEventsAsync(query);
        return System.Text.Json.JsonSerializer.Serialize(events, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    /// <summary>
    /// Get IP address statistics
    /// </summary>
    public async Task<Dictionary<string, int>> GetIpStatisticsAsync(int topCount = 10)
    {
        await Task.CompletedTask;

        return _events.Values
            .Where(e => !string.IsNullOrEmpty(e.IpAddress))
            .GroupBy(e => e.IpAddress!)
            .OrderByDescending(g => g.Count())
            .Take(topCount)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    // Helper methods

    private string GenerateCorrelationId(SecurityEvent securityEvent)
    {
        // Generate correlation ID based on user, IP, and time window
        var userId = securityEvent.UserId ?? "anonymous";
        var ipAddress = securityEvent.IpAddress ?? "unknown";
        var timeWindow = securityEvent.Timestamp.ToString("yyyyMMddHHmm");

        return $"{userId}_{ipAddress}_{timeWindow}".GetHashCode().ToString("X");
    }
}
