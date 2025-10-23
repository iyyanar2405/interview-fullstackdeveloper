using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Extensions.Options;
using Module_06_Production_Monitoring.Models;

namespace Module_06_Production_Monitoring.Services;

public sealed class DashboardCatalogService
{
    private readonly ConcurrentDictionary<string, DashboardDefinition> _dashboards = new(StringComparer.OrdinalIgnoreCase);

    public DashboardCatalogService(IOptions<MonitoringOptions> options)
    {
        foreach (var dashboard in options.Value.Dashboards)
        {
            _dashboards[dashboard.Name] = Clone(dashboard);
        }

        if (_dashboards.IsEmpty)
        {
            var fallback = new DashboardDefinition
            {
                Name = "Service Health",
                Description = "Fallback dashboard showing core metrics",
                Widgets =
                {
                    new WidgetDefinition
                    {
                        Title = "CPU Utilization",
                        Visualization = "line",
                        Metric = "cpu",
                        Query = "avg(cpu) by service"
                    },
                    new WidgetDefinition
                    {
                        Title = "Request Latency",
                        Visualization = "heatmap",
                        Metric = "latency",
                        Query = "p95(latency) by route"
                    }
                }
            };

            _dashboards[fallback.Name] = fallback;
        }
    }

    public IReadOnlyCollection<DashboardDefinition> GetAll()
    {
        return _dashboards.Values.Select(Clone).ToArray();
    }

    public DashboardDefinition? Get(string name)
    {
        return _dashboards.TryGetValue(name, out var dashboard) ? Clone(dashboard) : null;
    }

    public DashboardDefinition Upsert(DashboardDefinition dashboard)
    {
        var clone = Clone(dashboard);
        _dashboards[clone.Name] = clone;
        return Clone(clone);
    }

    public void Delete(string name)
    {
        _dashboards.TryRemove(name, out _);
    }

    private static DashboardDefinition Clone(DashboardDefinition dashboard)
    {
        return new DashboardDefinition
        {
            Name = dashboard.Name,
            Description = dashboard.Description,
            Widgets = dashboard.Widgets
                .Select(widget => new WidgetDefinition
                {
                    Title = widget.Title,
                    Visualization = widget.Visualization,
                    Metric = widget.Metric,
                    Query = widget.Query
                })
                .ToList()
        };
    }
}
