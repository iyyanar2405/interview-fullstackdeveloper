using System.Threading.Channels;
using Module_02_CICD_Pipelines.Models;

namespace Module_02_CICD_Pipelines.Services;

public sealed class PipelineQueueService
{
    private readonly Channel<PipelineQueueItem> _queue;
    private readonly ILogger<PipelineQueueService> _logger;

    public PipelineQueueService(ILogger<PipelineQueueService> logger)
    {
        _logger = logger;
        _queue = Channel.CreateBounded<PipelineQueueItem>(new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait
        });
    }

    public async ValueTask EnqueueAsync(PipelineQueueItem item, CancellationToken cancellationToken)
    {
        await _queue.Writer.WriteAsync(item, cancellationToken);
        _logger.LogInformation("Queued pipeline for {Project} using template {Template}", item.Request.ProjectName, item.TemplateName);
    }

    public IAsyncEnumerable<PipelineQueueItem> DequeueAllAsync(CancellationToken cancellationToken)
    {
        return _queue.Reader.ReadAllAsync(cancellationToken);
    }
}
