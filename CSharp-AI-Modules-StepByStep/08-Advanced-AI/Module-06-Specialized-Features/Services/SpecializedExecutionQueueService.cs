using System.Threading.Channels;
using Module_06_Specialized_Features.Models;

namespace Module_06_Specialized_Features.Services;

public sealed class SpecializedExecutionQueueService
{
    private readonly Channel<SpecializedTaskWorkItem> _channel;
    private readonly SpecializedExecutionHistoryService _history;

    public SpecializedExecutionQueueService(SpecializedExecutionHistoryService history)
    {
        _history = history;
        _channel = Channel.CreateUnbounded<SpecializedTaskWorkItem>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });
    }

    public string Enqueue(SpecializedTaskRequest request)
    {
        var runId = Guid.NewGuid().ToString("N");
        var clone = request.Clone();
        clone.RequestedAt = DateTimeOffset.UtcNow;
        _history.RegisterPending(runId, clone);
        var workItem = new SpecializedTaskWorkItem(runId, clone);
        _channel.Writer.TryWrite(workItem);
        return runId;
    }

    public IAsyncEnumerable<SpecializedTaskWorkItem> ReadAllAsync(CancellationToken cancellationToken)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }
}
