using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareEngineerSkills.Application.Common.Events;
using SoftwareEngineerSkills.Domain.Events;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Events;

/// <summary>
/// Handler for the DummyDeletedEvent
/// </summary>
public class DummyDeletedEventHandler : INotificationHandler<DomainEventNotification<DummyDeletedEvent>>
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
    /// <param name="notification">The notification wrapper containing the domain event</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public Task Handle(DomainEventNotification<DummyDeletedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        
        _logger.LogInformation("Dummy entity deleted with ID: {DummyId} at {OccurredOn}", 
            domainEvent.DummyId, 
            domainEvent.OccurredOn);
            
        // Additional logic can be added here like:
        // - Cleaning up related resources
        // - Triggering cascading deletes
        // - Auditing the deletion
        
        return Task.CompletedTask;
    }
}