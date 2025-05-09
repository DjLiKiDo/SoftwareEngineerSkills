using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareEngineerSkills.Application.Common.Events;
using SoftwareEngineerSkills.Domain.Events;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Events;

/// <summary>
/// Handler for the DummyDeactivatedEvent
/// </summary>
public class DummyDeactivatedEventHandler : INotificationHandler<DomainEventNotification<DummyDeactivatedEvent>>
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
    /// <param name="notification">The notification wrapper containing the domain event</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public Task Handle(DomainEventNotification<DummyDeactivatedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        
        _logger.LogInformation("Dummy entity deactivated with ID: {DummyId} at {OccurredOn}", 
            domainEvent.DummyId, 
            domainEvent.OccurredOn);
            
        // Additional logic can be added here like:
        // - Sending a notification
        // - Updating related entities or services
        // - Triggering cleanup processes
        
        return Task.CompletedTask;
    }
}
