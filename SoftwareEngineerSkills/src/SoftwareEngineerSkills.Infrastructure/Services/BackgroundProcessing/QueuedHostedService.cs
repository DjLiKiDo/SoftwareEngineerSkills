using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SoftwareEngineerSkills.Infrastructure.Services.BackgroundProcessing;

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
