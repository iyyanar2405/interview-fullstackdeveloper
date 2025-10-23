using System.Threading.Channels;
using Module_01_Containerization.Models;

namespace Module_01_Containerization.Services;

public sealed class ContainerBuildQueueService
{
    private readonly Channel<BuildQueueItem> _channel;
    private readonly ILogger<ContainerBuildQueueService> _logger;

    public ContainerBuildQueueService(ILogger<ContainerBuildQueueService> logger)
    {
        _logger = logger;
        _channel = Channel.CreateBounded<BuildQueueItem>(new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait
        });
    }

    public async ValueTask QueueAsync(BuildQueueItem item, CancellationToken cancellationToken = default)
    {
        await _channel.Writer.WriteAsync(item, cancellationToken);
        _logger.LogInformation("Queued container build for {Project}", item.Request.ProjectName);
    }

    public IAsyncEnumerable<BuildQueueItem> ReadAllAsync(CancellationToken cancellationToken)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }
}
