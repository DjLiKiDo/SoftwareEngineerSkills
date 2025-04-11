using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareEngineerSkills.Domain.Events;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Events;

/// <summary>
/// Handler for the DummyDeletedEvent
/// </summary>
public class DummyDeletedEventHandler : INotificationHandler<DummyDeletedEvent>
{
    private readonly ILogger<DummyDeletedEventHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DummyDeletedEventHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger</param>
    public DummyDeletedEventHandler(ILogger<DummyDeletedEventHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the DummyDeletedEvent
    /// </summary>
    /// <param name="notification">The event</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public Task Handle(DummyDeletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Dummy entity deleted with ID: {DummyId} at {OccurredOn}", 
            notification.DummyId, 
            notification.OccurredOn);
            
        // Additional logic can be added here like:
        // - Cleaning up related resources
        // - Updating statistics
        // - Notifying other systems of the deletion
        // - Archiving data before permanent deletion
        
        return Task.CompletedTask;
    }
}