using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Module_04_Application_Insights.Telemetry;

/// <summary>
/// Sets the cloud role name and instance for all telemetry to make multi-service querying easier in Application Insights.
/// </summary>
public sealed class CloudRoleTelemetryInitializer : ITelemetryInitializer
{
    private readonly string _roleName;
    private readonly string _instanceName;

    public CloudRoleTelemetryInitializer(IConfiguration configuration)
    {
        _roleName = configuration["ApplicationInsights:RoleName"] ?? "module-04-appinsights";
        _instanceName = Environment.MachineName;
    }

    public void Initialize(ITelemetry telemetry)
    {
        telemetry.Context.Cloud.RoleName = _roleName;
        telemetry.Context.Cloud.RoleInstance = _instanceName;
    }
}
