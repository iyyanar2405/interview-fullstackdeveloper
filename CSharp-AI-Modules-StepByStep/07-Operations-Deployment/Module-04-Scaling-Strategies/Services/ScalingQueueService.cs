using System.Threading.Channels;
using Module_04_Scaling_Strategies.Models;

namespace Module_04_Scaling_Strategies.Services;

public sealed class ScalingQueueService
{
    private readonly Channel<ScalingQueueItem> _channel;
    private readonly ILogger<ScalingQueueService> _logger;

    public ScalingQueueService(ILogger<ScalingQueueService> logger)
    {
        _logger = logger;
        _channel = Channel.CreateBounded<ScalingQueueItem>(new BoundedChannelOptions(200)
        {
            FullMode = BoundedChannelFullMode.Wait
        });
    }

    public async ValueTask QueueAsync(ScalingQueueItem item, CancellationToken cancellationToken = default)
    {
        await _channel.Writer.WriteAsync(item, cancellationToken);
        _logger.LogInformation("Queued scaling run {RunId} for profile {Profile}", item.RunId, item.Request.ProfileName);
    }

    public IAsyncEnumerable<ScalingQueueItem> ReadAllAsync(CancellationToken cancellationToken)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }
}
