using Module_05_Alerting_Systems.Models;
using Microsoft.Extensions.Options;

namespace Module_05_Alerting_Systems.Services;

public sealed class AlertDispatchService
{
    private readonly ILogger<AlertDispatchService> _logger;
    private readonly AlertingOptions _options;

    public AlertDispatchService(ILogger<AlertDispatchService> logger, IOptions<AlertingOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public Task DispatchAsync(AlertNotification notification, CancellationToken cancellationToken = default)
    {
        foreach (var channel in notification.Channels)
        {
            var endpoint = _options.Channels.TryGetValue(channel.ToString(), out var value) ? value : "not-configured";
            _logger.LogInformation(
                "Dispatching alert {AlertId} via {Channel} to {Endpoint}. Message: {Message}",
                notification.AlertId,
                channel,
                endpoint,
                notification.Message);
        }

        return Task.CompletedTask;
    }
}
