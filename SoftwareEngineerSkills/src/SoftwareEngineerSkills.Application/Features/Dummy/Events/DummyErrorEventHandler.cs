using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareEngineerSkills.Application.Common.Events;
using SoftwareEngineerSkills.Domain.Events;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Events;

/// <summary>
/// Handler for the DummyErrorEvent
/// </summary>
public class DummyErrorEventHandler : INotificationHandler<DomainEventNotification<DummyErrorEvent>>
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
    /// <param name="notification">The notification wrapper containing the domain event</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public Task Handle(DomainEventNotification<DummyErrorEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        _logger.LogError("Error occurred in dummy entity with ID: {DummyId}, Error: {ErrorMessage} at {OccurredOn}",
            domainEvent.DummyId,
            domainEvent.ErrorMessage,
            domainEvent.OccurredOn);

        // Additional logic can be added here like:
        // - Sending error alerts or notifications
        // - Initiating recovery procedures
        // - Logging to specialized monitoring systems

        return Task.CompletedTask;
    }
}
