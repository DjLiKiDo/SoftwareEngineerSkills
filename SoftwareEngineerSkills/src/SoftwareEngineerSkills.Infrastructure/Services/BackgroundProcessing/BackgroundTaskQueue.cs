using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SoftwareEngineerSkills.Infrastructure.Services.BackgroundProcessing;

/// <summary>
/// Interface for a background task queue
/// </summary>
public interface IBackgroundTaskQueue
{
    /// <summary>
    /// Queues a task to run in the background
    /// </summary>
    /// <param name="task">The task to run</param>
    /// <returns>A task representing the asynchronous operation</returns>
    ValueTask QueueBackgroundWorkItemAsync(Func<CancellationToken, ValueTask> task);
    
    /// <summary>
    /// Dequeues a task from the queue
    /// </summary>
    /// <param name="cancellationToken">A token for cancelling the operation</param>
    /// <returns>A task representing the asynchronous operation</returns>
    ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken);
}

/// <summary>
/// Implementation of a background task queue using a concurrent queue
/// </summary>
public class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly System.Threading.Channels.Channel<Func<CancellationToken, ValueTask>> _queue;
    
    /// <summary>
    /// Creates a new instance of the BackgroundTaskQueue class
    /// </summary>
    /// <param name="capacity">Maximum queue size (default is unbounded)</param>
    public BackgroundTaskQueue(int capacity = -1)
    {
        var options = capacity > 0
            ? new System.Threading.Channels.UnboundedChannelOptions { SingleReader = true, SingleWriter = false }
            : new System.Threading.Channels.BoundedChannelOptions(capacity) { FullMode = System.Threading.Channels.BoundedChannelFullMode.Wait };
        
        _queue = capacity > 0
            ? System.Threading.Channels.Channel.CreateUnbounded<Func<CancellationToken, ValueTask>>(options)
            : System.Threading.Channels.Channel.CreateBounded<Func<CancellationToken, ValueTask>>(options);
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

/// <summary>
/// Background service that processes queued tasks
/// </summary>
public class QueuedHostedService : BackgroundService
{
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly ILogger<QueuedHostedService> _logger;
    
    /// <summary>
    /// Creates a new instance of the QueuedHostedService class
    /// </summary>
    /// <param name="taskQueue">The task queue to process</param>
    /// <param name="logger">Logger</param>
    public QueuedHostedService(IBackgroundTaskQueue taskQueue, ILogger<QueuedHostedService> logger)
    {
        _taskQueue = taskQueue;
        _logger = logger;
    }
    
    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Queued Hosted Service is running.");
        
        await BackgroundProcessing(stoppingToken);
    }
    
    private async Task BackgroundProcessing(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var task = await _taskQueue.DequeueAsync(stoppingToken);
            
            try
            {
                await task(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred executing task.");
            }
        }
    }
    
    /// <inheritdoc />
    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Queued Hosted Service is stopping.");
        
        await base.StopAsync(stoppingToken);
    }
}
