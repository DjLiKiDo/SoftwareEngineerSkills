using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareEngineerSkills.Application.Common.Events;
using SoftwareEngineerSkills.Domain.Events;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Events;

/// <summary>
/// Handler for the DummyCreatedEvent
/// </summary>
public class DummyCreatedEventHandler : INotificationHandler<DomainEventNotification<DummyCreatedEvent>>
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
    /// <param name="notification">The notification wrapper containing the domain event</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public Task Handle(DomainEventNotification<DummyCreatedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        
        _logger.LogInformation("Dummy entity created with ID: {DummyId}, Name: {Name} at {OccurredOn}", 
            domainEvent.DummyId,
            domainEvent.Name ?? "Unnamed",
            domainEvent.OccurredOn);
            
        // Additional logic can be added here like:
        // - Sending welcome notifications
        // - Setting up related resources
        // - Initializing additional data
        
        return Task.CompletedTask;
    }
}
