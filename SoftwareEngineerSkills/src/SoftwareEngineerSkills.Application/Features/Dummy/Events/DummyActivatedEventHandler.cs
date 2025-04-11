using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareEngineerSkills.Application.Common.Events;
using SoftwareEngineerSkills.Domain.Events;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Events;

/// <summary>
/// Handler for the DummyActivatedEvent
/// </summary>
public class DummyActivatedEventHandler : INotificationHandler<DomainEventNotification<DummyActivatedEvent>>
{
    private readonly ILogger<DummyActivatedEventHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DummyActivatedEventHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger</param>
    public DummyActivatedEventHandler(ILogger<DummyActivatedEventHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the DummyActivatedEvent
    /// </summary>
    /// <param name="notification">The notification wrapper containing the domain event</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public Task Handle(DomainEventNotification<DummyActivatedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        
        _logger.LogInformation("Dummy entity activated with ID: {DummyId} at {OccurredOn}", 
            domainEvent.DummyId, 
            domainEvent.OccurredOn);
            
        // Additional logic can be added here like:
        // - Sending a notification
        // - Updating related systems
        // - Triggering other processes
        
        return Task.CompletedTask;
    }
}