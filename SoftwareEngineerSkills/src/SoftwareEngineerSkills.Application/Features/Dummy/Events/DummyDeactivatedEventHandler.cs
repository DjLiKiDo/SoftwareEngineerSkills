using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareEngineerSkills.Domain.Events;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Events;

/// <summary>
/// Handler for the DummyDeactivatedEvent
/// </summary>
public class DummyDeactivatedEventHandler : INotificationHandler<DummyDeactivatedEvent>
{
    private readonly ILogger<DummyDeactivatedEventHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DummyDeactivatedEventHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger</param>
    public DummyDeactivatedEventHandler(ILogger<DummyDeactivatedEventHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the DummyDeactivatedEvent
    /// </summary>
    /// <param name="notification">The event</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public Task Handle(DummyDeactivatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Dummy entity deactivated with ID: {DummyId} at {OccurredOn}", 
            notification.DummyId, 
            notification.OccurredOn);
            
        // Additional logic can be added here like:
        // - Sending a notification
        // - Updating related systems
        // - Archiving related data
        
        return Task.CompletedTask;
    }
}