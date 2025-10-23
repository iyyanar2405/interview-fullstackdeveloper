using System.Threading.Channels;
using Module_05_Cloud_Deployment.Models;

namespace Module_05_Cloud_Deployment.Services;

public sealed class DeploymentQueueService
{
    private readonly Channel<DeploymentQueueItem> _channel;
    private readonly ILogger<DeploymentQueueService> _logger;

    public DeploymentQueueService(ILogger<DeploymentQueueService> logger)
    {
        _logger = logger;
        _channel = Channel.CreateBounded<DeploymentQueueItem>(new BoundedChannelOptions(200)
        {
            FullMode = BoundedChannelFullMode.Wait
        });
    }

    public async ValueTask QueueAsync(DeploymentQueueItem item, CancellationToken cancellationToken = default)
    {
        await _channel.Writer.WriteAsync(item, cancellationToken);
        _logger.LogInformation("Queued deployment run {RunId} for blueprint {Blueprint}", item.RunId, item.Plan.BlueprintName);
    }

    public IAsyncEnumerable<DeploymentQueueItem> ReadAllAsync(CancellationToken cancellationToken)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }
}
