using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareEngineerSkills.Domain.Events;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Events;

/// <summary>
/// Handler for the DummyUpdatedEvent
/// </summary>
public class DummyUpdatedEventHandler : INotificationHandler<DummyUpdatedEvent>
{
    private readonly ILogger<DummyUpdatedEventHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DummyUpdatedEventHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger</param>
    public DummyUpdatedEventHandler(ILogger<DummyUpdatedEventHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the DummyUpdatedEvent
    /// </summary>
    /// <param name="notification">The event</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public Task Handle(DummyUpdatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Dummy entity updated with ID: {DummyId}, Name: {Name} at {OccurredOn}", 
            notification.DummyId, 
            notification.Name ?? "Unnamed",
            notification.OccurredOn);
            
        // Additional logic can be added here like:
        // - Sending a notification about the update
        // - Updating related entities or services
        // - Triggering a cache invalidation
        // - Publishing to an external system
        
        return Task.CompletedTask;
    }
}