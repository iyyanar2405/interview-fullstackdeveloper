using System.Threading.Channels;
using Module_03_Infrastructure_as_Code.Models;

namespace Module_03_Infrastructure_as_Code.Services;

public sealed class ProvisioningQueueService
{
    private readonly Channel<ProvisioningQueueItem> _channel;
    private readonly ILogger<ProvisioningQueueService> _logger;

    public ProvisioningQueueService(ILogger<ProvisioningQueueService> logger)
    {
        _logger = logger;
        _channel = Channel.CreateBounded<ProvisioningQueueItem>(new BoundedChannelOptions(250)
        {
            FullMode = BoundedChannelFullMode.Wait
        });
    }

    public async ValueTask QueueAsync(ProvisioningQueueItem item, CancellationToken cancellationToken = default)
    {
        await _channel.Writer.WriteAsync(item, cancellationToken);
        _logger.LogInformation("Queued provisioning run {RunId} for {Project}", item.RunId, item.Request.ProjectName);
    }

    public IAsyncEnumerable<ProvisioningQueueItem> ReadAllAsync(CancellationToken cancellationToken)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }
}
