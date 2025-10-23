using System.Collections.Concurrent;
using System.Linq;
using Module_06_Production_Monitoring.Models;

namespace Module_06_Production_Monitoring.Services;

public sealed class IncidentService
{
    private readonly ConcurrentDictionary<string, ActiveIncident> _incidents = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, PastIncident> _history = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyCollection<ActiveIncident> GetActiveIncidents()
    {
        return _incidents.Values.Select(Clone).ToArray();
    }

    public IReadOnlyCollection<PastIncident> GetIncidentHistory()
    {
        return _history.Values.Select(Clone).OrderByDescending(incident => incident.ResolvedAt).ToArray();
    }

    public ActiveIncident DeclareIncident(string alertId, string description, IncidentSeverity severity)
    {
        var incidentId = Guid.NewGuid().ToString("N");
        var incident = new ActiveIncident
        {
            Id = incidentId,
            AlertId = alertId,
            Summary = description,
            Severity = severity,
            DeclaredAt = DateTimeOffset.UtcNow,
            Owner = "oncall@service"
        };

        _incidents[incidentId] = Clone(incident);
        return incident;
    }

    public PastIncident ResolveIncident(string incidentId, string resolutionSummary)
    {
        if (!_incidents.TryRemove(incidentId, out var active))
        {
            throw new InvalidOperationException($"Incident {incidentId} not found");
        }

        var postMortem = new PostMortemSummary
        {
            IncidentId = incidentId,
            IncidentSummary = active.Summary,
            RootCause = "Pending analysis",
            Duration = (DateTimeOffset.UtcNow - active.DeclaredAt).ToString(),
            ImpactSummary = "Service impact ongoing investigation",
            FollowUpActions =
            {
                "Document incident timeline",
                "Identify contributing factors",
                "Schedule retrospective"
            }
        };

        var history = new PastIncident
        {
            Id = active.Id,
            AlertId = active.AlertId,
            Summary = active.Summary,
            Severity = active.Severity,
            DeclaredAt = active.DeclaredAt,
            ResolvedAt = DateTimeOffset.UtcNow,
            ResolutionSummary = resolutionSummary,
            PostMortem = postMortem
        };

        _history[incidentId] = Clone(history);
        return history;
    }

    private static ActiveIncident Clone(ActiveIncident incident)
    {
        return new ActiveIncident
        {
            Id = incident.Id,
            AlertId = incident.AlertId,
            Summary = incident.Summary,
            Severity = incident.Severity,
            DeclaredAt = incident.DeclaredAt,
            Owner = incident.Owner,
            Impact = incident.Impact,
            Mitigation = incident.Mitigation
        };
    }

    private static PastIncident Clone(PastIncident incident)
    {
        return new PastIncident
        {
            Id = incident.Id,
            AlertId = incident.AlertId,
            Summary = incident.Summary,
            Severity = incident.Severity,
            DeclaredAt = incident.DeclaredAt,
            ResolvedAt = incident.ResolvedAt,
            ResolutionSummary = incident.ResolutionSummary,
            PostMortem = incident.PostMortem is null
                ? null
                : new PostMortemSummary
                {
                    IncidentId = incident.PostMortem.IncidentId,
                    IncidentSummary = incident.PostMortem.IncidentSummary,
                    RootCause = incident.PostMortem.RootCause,
                    Duration = incident.PostMortem.Duration,
                    ImpactSummary = incident.PostMortem.ImpactSummary,
                    FollowUpActions = incident.PostMortem.FollowUpActions.ToList()
                }
        };
    }
}
