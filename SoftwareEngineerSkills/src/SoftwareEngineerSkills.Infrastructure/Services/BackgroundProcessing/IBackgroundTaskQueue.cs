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
