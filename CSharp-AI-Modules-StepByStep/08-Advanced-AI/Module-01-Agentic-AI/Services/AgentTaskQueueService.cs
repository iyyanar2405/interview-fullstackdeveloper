using System.Threading.Channels;
using Microsoft.Extensions.Options;
using Module_01_Agentic_AI.Models;

namespace Module_01_Agentic_AI.Services;

public sealed class AgentTaskQueueService
{
    private readonly Channel<AgentTask> _channel;

    public AgentTaskQueueService(IOptions<AgenticOptions> options)
    {
        var capacity = Math.Max(1, options.Value.Simulation.MaxConcurrentTasks * 4);
        _channel = Channel.CreateBounded<AgentTask>(new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false,
            SingleWriter = false
        });
    }

    public ValueTask QueueAsync(AgentTask task, CancellationToken cancellationToken)
    {
        return _channel.Writer.WriteAsync(task, cancellationToken);
    }

    public IAsyncEnumerable<AgentTask> ReadAllAsync(CancellationToken cancellationToken)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }
}
