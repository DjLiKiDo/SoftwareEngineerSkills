using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareEngineerSkills.Domain.Events;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Events;

/// <summary>
/// Handler for the DummyErrorEvent
/// </summary>
public class DummyErrorEventHandler : INotificationHandler<DummyErrorEvent>
{
    private readonly ILogger<DummyErrorEventHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DummyErrorEventHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger</param>
    public DummyErrorEventHandler(ILogger<DummyErrorEventHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the DummyErrorEvent
    /// </summary>
    /// <param name="notification">The event</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public Task Handle(DummyErrorEvent notification, CancellationToken cancellationToken)
    {
        if (notification.DummyId.HasValue)
        {
            _logger.LogError("Dummy error occurred during {Operation} for entity ID: {DummyId}. Error: {ErrorMessage}",
                notification.Operation,
                notification.DummyId,
                notification.ErrorMessage);
        }
        else
        {
            _logger.LogError("Dummy error occurred during {Operation}. Error: {ErrorMessage}",
                notification.Operation,
                notification.ErrorMessage);
        }
        
        // Additional error handling logic can be added here like:
        // - Sending alerts to administrators
        // - Recording the error in a separate error tracking system
        // - Taking corrective actions if possible
        
        return Task.CompletedTask;
    }
}