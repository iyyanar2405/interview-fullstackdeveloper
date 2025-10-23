using System.Security.Cryptography;
using System.Text;
using Module_04_Data_Protection.Models;

namespace Module_04_Data_Protection.Services;

public interface IAuditLoggingService
{
    Task<AuditLogEntry> LogAsync(string eventType, string? userId, string? resourceId, 
        string action, AuditSeverity severity, Dictionary<string, object>? metadata = null);
    Task<AuditQueryResult> QueryLogsAsync(AuditQueryRequest request);
    Task<bool> VerifyLogIntegrityAsync(string logId);
    Task<List<AuditLogEntry>> ExportLogsAsync(DateTime startDate, DateTime endDate);
}

public class AuditLoggingService : IAuditLoggingService
{
    private readonly ILogger<AuditLoggingService> _logger;
    private readonly List<AuditLogEntry> _logs;
    private string? _lastLogHash;

    public AuditLoggingService(ILogger<AuditLoggingService> logger)
    {
        _logger = logger;
        _logs = new List<AuditLogEntry>();
        _lastLogHash = null;
    }

    public async Task<AuditLogEntry> LogAsync(
        string eventType, 
        string? userId, 
        string? resourceId, 
        string action,
        AuditSeverity severity,
        Dictionary<string, object>? metadata = null)
    {
        var logEntry = new AuditLogEntry
        {
            LogId = Guid.NewGuid().ToString(),
            EventType = eventType,
            UserId = userId,
            ResourceId = resourceId,
            Action = action,
            Details = $"{eventType}: {action}",
            Metadata = metadata ?? new Dictionary<string, object>(),
            Timestamp = DateTime.UtcNow,
            IpAddress = "127.0.0.1", // In production, get from HttpContext
            UserAgent = "API Client", // In production, get from HttpContext
            Severity = severity,
            Success = true
        };

        // Calculate hash chain for tamper-proofing
        logEntry.Hash = CalculateHash(logEntry, _lastLogHash);
        _lastLogHash = logEntry.Hash;

        _logs.Add(logEntry);

        _logger.LogInformation("Audit log created. Type: {Type}, User: {User}, Severity: {Severity}", 
            eventType, userId ?? "System", severity);

        return await Task.FromResult(logEntry);
    }

    public async Task<AuditQueryResult> QueryLogsAsync(AuditQueryRequest request)
    {
        _logger.LogInformation("Querying audit logs. Filter: {Filter}", 
            System.Text.Json.JsonSerializer.Serialize(request));

        var query = _logs.AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(request.UserId))
        {
            query = query.Where(l => l.UserId == request.UserId);
        }

        if (!string.IsNullOrEmpty(request.EventType))
        {
            query = query.Where(l => l.EventType == request.EventType);
        }

        if (request.StartDate.HasValue)
        {
            query = query.Where(l => l.Timestamp >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            query = query.Where(l => l.Timestamp <= request.EndDate.Value);
        }

        if (request.Severity.HasValue)
        {
            query = query.Where(l => l.Severity == request.Severity.Value);
        }

        var totalCount = query.Count();

        // Apply pagination
        var logs = query
            .OrderByDescending(l => l.Timestamp)
            .Skip(request.PageNumber * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return await Task.FromResult(new AuditQueryResult
        {
            Logs = logs,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        });
    }

    public async Task<bool> VerifyLogIntegrityAsync(string logId)
    {
        _logger.LogInformation("Verifying log integrity for: {LogId}", logId);

        var logIndex = _logs.FindIndex(l => l.LogId == logId);
        if (logIndex == -1)
        {
            return false;
        }

        var log = _logs[logIndex];
        var previousHash = logIndex > 0 ? _logs[logIndex - 1].Hash : null;

        var calculatedHash = CalculateHash(log, previousHash);

        var isValid = calculatedHash == log.Hash;

        if (!isValid)
        {
            _logger.LogCritical("SECURITY ALERT: Log tampering detected for log {LogId}", logId);
        }

        return await Task.FromResult(isValid);
    }

    public async Task<List<AuditLogEntry>> ExportLogsAsync(DateTime startDate, DateTime endDate)
    {
        _logger.LogInformation("Exporting audit logs. Period: {Start} to {End}", startDate, endDate);

        var logs = _logs
            .Where(l => l.Timestamp >= startDate && l.Timestamp <= endDate)
            .OrderBy(l => l.Timestamp)
            .ToList();

        // In production, verify integrity of all exported logs
        foreach (var log in logs)
        {
            await VerifyLogIntegrityAsync(log.LogId);
        }

        return logs;
    }

    private string CalculateHash(AuditLogEntry entry, string? previousHash)
    {
        // Create hash chain for tamper-proofing
        var dataToHash = $"{entry.LogId}|{entry.EventType}|{entry.UserId}|{entry.ResourceId}|" +
                         $"{entry.Action}|{entry.Timestamp:O}|{entry.Severity}|{previousHash}";

        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(dataToHash));
        return Convert.ToBase64String(hashBytes);
    }
}
