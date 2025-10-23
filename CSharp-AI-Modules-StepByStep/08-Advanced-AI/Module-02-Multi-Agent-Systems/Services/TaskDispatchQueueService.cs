using System.Threading.Channels;
using Microsoft.Extensions.Options;
using Module_02_Multi_Agent_Systems.Models;

namespace Module_02_Multi_Agent_Systems.Services;

public sealed class TaskDispatchQueueService
{
    private readonly Channel<MultiAgentTask> _channel;

    public TaskDispatchQueueService(IOptions<MultiAgentOptions> options)
    {
        var capacity = Math.Max(1, options.Value.Simulation.MaxConcurrentTasks * 2);
        _channel = Channel.CreateBounded<MultiAgentTask>(new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false,
            SingleWriter = false
        });
    }

    public ValueTask QueueAsync(MultiAgentTask task, CancellationToken token)
    {
        return _channel.Writer.WriteAsync(task, token);
    }

    public IAsyncEnumerable<MultiAgentTask> ReadAllAsync(CancellationToken token)
    {
        return _channel.Reader.ReadAllAsync(token);
    }
}
