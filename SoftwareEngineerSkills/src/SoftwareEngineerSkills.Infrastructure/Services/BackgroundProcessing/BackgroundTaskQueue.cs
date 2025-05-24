using System.Threading.Channels;

namespace SoftwareEngineerSkills.Infrastructure.Services.BackgroundProcessing;

/// <summary>
/// Implementation of a background task queue using a concurrent queue
/// </summary>
public class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<Func<CancellationToken, ValueTask>> _queue;
    
    /// <summary>
    /// Creates a new instance of the BackgroundTaskQueue class
    /// </summary>
    /// <param name="capacity">Maximum queue size (default is unbounded)</param>
    public BackgroundTaskQueue(int capacity = -1)
    {
        if (capacity <= 0)
        {
            var unboundedOptions = new UnboundedChannelOptions { SingleReader = true, SingleWriter = false };
            _queue = Channel.CreateUnbounded<Func<CancellationToken, ValueTask>>(unboundedOptions);
        }
        else
        {
            var boundedOptions = new BoundedChannelOptions(capacity) { FullMode = BoundedChannelFullMode.Wait };
            _queue = Channel.CreateBounded<Func<CancellationToken, ValueTask>>(boundedOptions);
        }
    }
    
    /// <inheritdoc />
    public async ValueTask QueueBackgroundWorkItemAsync(Func<CancellationToken, ValueTask> task)
    {
        if (task == null)
        {
            throw new ArgumentNullException(nameof(task));
        }
        
        await _queue.Writer.WriteAsync(task);
    }
    
    /// <inheritdoc />
    public async ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken)
    {
        return await _queue.Reader.ReadAsync(cancellationToken);
    }
}
