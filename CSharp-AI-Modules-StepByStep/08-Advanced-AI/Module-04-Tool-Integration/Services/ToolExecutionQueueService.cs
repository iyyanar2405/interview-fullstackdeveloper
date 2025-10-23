using System.Threading.Channels;
using Module_04_Tool_Integration.Models;

namespace Module_04_Tool_Integration.Services;

public sealed class ToolExecutionQueueService
{
    private readonly Channel<ToolExecutionWorkItem> _channel;
    private readonly ToolExecutionHistoryService _history;

    public ToolExecutionQueueService(ToolExecutionHistoryService history)
    {
        _history = history;
        _channel = Channel.CreateUnbounded<ToolExecutionWorkItem>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });
    }

    public string Enqueue(ToolExecutionRequest request)
    {
        var runId = Guid.NewGuid().ToString("N");
        var clonedRequest = request.Clone();
        clonedRequest.RequestedAt = DateTimeOffset.UtcNow;

        var workItem = new ToolExecutionWorkItem(runId, clonedRequest);
        _history.RegisterPending(runId, clonedRequest);
        _channel.Writer.TryWrite(workItem);
        return runId;
    }

    public IAsyncEnumerable<ToolExecutionWorkItem> ReadAllAsync(CancellationToken cancellationToken)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }
}
