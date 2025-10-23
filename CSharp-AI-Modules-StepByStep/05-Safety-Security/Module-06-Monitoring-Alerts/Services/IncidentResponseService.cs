using Module_06_Monitoring_Alerts.Models;
using System.Collections.Concurrent;

namespace Module_06_Monitoring_Alerts.Services;

/// <summary>
/// Service for managing security incidents and response workflows
/// </summary>
public class IncidentResponseService
{
    private readonly ConcurrentDictionary<string, Incident> _incidents = new();
    private readonly ConcurrentDictionary<string, List<IncidentAction>> _incidentActions = new();

    /// <summary>
    /// Create a new incident
    /// </summary>
    public Task<Incident> CreateIncidentAsync(Incident incident)
    {
        incident.IncidentId = incident.IncidentId ?? Guid.NewGuid().ToString();
        incident.CreatedAt = DateTime.UtcNow;
        incident.Status = IncidentStatus.Open;

        _incidents[incident.IncidentId] = incident;
        _incidentActions[incident.IncidentId] = new List<IncidentAction>();

        return Task.FromResult(incident);
    }

    /// <summary>
    /// Get incident by ID
    /// </summary>
    public Task<Incident?> GetIncidentAsync(string incidentId)
    {
        _incidents.TryGetValue(incidentId, out var incident);
        return Task.FromResult(incident);
    }

    /// <summary>
    /// Get all incidents
    /// </summary>
    public Task<List<Incident>> GetIncidentsAsync(IncidentStatus? status = null)
    {
        var incidents = _incidents.Values.AsEnumerable();

        if (status.HasValue)
            incidents = incidents.Where(i => i.Status == status.Value);

        return Task.FromResult(incidents.OrderByDescending(i => i.CreatedAt).ToList());
    }

    /// <summary>
    /// Update incident details
    /// </summary>
    public Task<Incident?> UpdateIncidentAsync(string incidentId, IncidentUpdate update)
    {
        if (!_incidents.TryGetValue(incidentId, out var incident))
            return Task.FromResult<Incident?>(null);

        if (!string.IsNullOrWhiteSpace(update.AssignedTo))
            incident.AssignedTo = update.AssignedTo;

        if (update.NewStatus.HasValue)
        {
            incident.Status = update.NewStatus.Value;
            if (update.NewStatus == IncidentStatus.Acknowledged)
                incident.AcknowledgedAt = DateTime.UtcNow;
            if (update.NewStatus == IncidentStatus.Resolved)
                incident.ResolvedAt = DateTime.UtcNow;
            if (update.NewStatus == IncidentStatus.Closed)
                incident.ClosedAt = DateTime.UtcNow;
        }

        if (!string.IsNullOrWhiteSpace(update.Notes))
        {
            AddAction(incidentId, new IncidentAction
            {
                ActionType = "Update",
                PerformedBy = update.UpdatedBy,
                Description = update.Notes
            });
        }

        return Task.FromResult<Incident?>(incident);
    }

    /// <summary>
    /// Add a response action to an incident
    /// </summary>
    public Task<IncidentAction> AddActionAsync(string incidentId, IncidentAction action)
    {
        if (!_incidents.ContainsKey(incidentId))
            throw new KeyNotFoundException($"Incident {incidentId} not found");

        return Task.FromResult(AddAction(incidentId, action));
    }

    /// <summary>
    /// Get incident timeline
    /// </summary>
    public Task<List<IncidentAction>> GetIncidentTimelineAsync(string incidentId)
    {
        if (_incidentActions.TryGetValue(incidentId, out var actions))
        {
            return Task.FromResult(actions.OrderBy(a => a.Timestamp).ToList());
        }

        return Task.FromResult(new List<IncidentAction>());
    }

    /// <summary>
    /// Link alerts to incident
    /// </summary>
    public Task<bool> LinkAlertAsync(string incidentId, string alertId)
    {
        if (!_incidents.TryGetValue(incidentId, out var incident))
            return Task.FromResult(false);

        if (!incident.RelatedAlertIds.Contains(alertId))
            incident.RelatedAlertIds.Add(alertId);

        return Task.FromResult(true);
    }

    /// <summary>
    /// Close incident with summary
    /// </summary>
    public Task<Incident?> CloseIncidentAsync(string incidentId, string resolvedBy, string summary)
    {
        if (!_incidents.TryGetValue(incidentId, out var incident))
            return Task.FromResult<Incident?>(null);

        incident.Status = IncidentStatus.Closed;
        incident.ResolvedBy = resolvedBy;
        incident.ResolvedAt = DateTime.UtcNow;
        incident.ClosedAt = DateTime.UtcNow;
        incident.Resolution = summary;

        AddAction(incidentId, new IncidentAction
        {
            ActionType = "Closure",
            PerformedBy = resolvedBy,
            Description = summary
        });

        return Task.FromResult<Incident?>(incident);
    }

    private IncidentAction AddAction(string incidentId, IncidentAction action)
    {
        action.ActionId = action.ActionId ?? Guid.NewGuid().ToString();
        action.Timestamp = DateTime.UtcNow;

        _incidentActions.AddOrUpdate(
            incidentId,
            new List<IncidentAction> { action },
            (_, list) =>
            {
                list.Add(action);
                return list;
            });

        if (_incidents.TryGetValue(incidentId, out var incident))
        {
            incident.Actions.Add(action);
        }

        return action;
    }
}
