using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareEngineerSkills.Domain.Events;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Events;

/// <summary>
/// Handler for the DummyCreatedEvent
/// </summary>
public class DummyCreatedEventHandler : INotificationHandler<DummyCreatedEvent>
{
    private readonly ILogger<DummyCreatedEventHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DummyCreatedEventHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger</param>
    public DummyCreatedEventHandler(ILogger<DummyCreatedEventHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the DummyCreatedEvent
    /// </summary>
    /// <param name="notification">The event</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public Task Handle(DummyCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Dummy entity created with ID: {DummyId}, Name: {Name}", 
            notification.DummyId, 
            notification.Name ?? "Unnamed");
            
        // Additional logic can be added here like:
        // - Sending a notification email
        // - Updating statistics
        // - Publishing to a message bus
        
        return Task.CompletedTask;
    }
}