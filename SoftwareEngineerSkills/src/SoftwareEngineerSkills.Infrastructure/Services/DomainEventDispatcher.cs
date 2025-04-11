using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareEngineerSkills.Application.Common.Events;
using SoftwareEngineerSkills.Domain.Events;
using System.Reflection;

namespace SoftwareEngineerSkills.Infrastructure.Services;

/// <summary>
/// Implementation of the domain event dispatcher that uses MediatR to publish domain events as notifications
/// </summary>
public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMediator _mediator;
    private readonly ILogger<DomainEventDispatcher> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainEventDispatcher"/> class.
    /// </summary>
    /// <param name="mediator">The MediatR mediator</param>
    /// <param name="logger">The logger</param>
    public DomainEventDispatcher(IMediator mediator, ILogger<DomainEventDispatcher> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Dispatches a domain event to its handlers using MediatR
    /// </summary>
    /// <param name="domainEvent">The domain event to dispatch</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Dispatching domain event of type {EventType}", domainEvent.GetType().Name);

        // Create a generic notification wrapper for the domain event
        var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
        var notification = Activator.CreateInstance(notificationType, domainEvent);

        // Publish the wrapped event via MediatR
        await _mediator.Publish(notification, cancellationToken);
    }

    /// <summary>
    /// Dispatches multiple domain events to their handlers
    /// </summary>
    /// <param name="domainEvents">The domain events to dispatch</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        if (domainEvents == null || !domainEvents.Any())
        {
            return;
        }

        _logger.LogDebug("Dispatching {EventCount} domain events", domainEvents.Count());

        foreach (var domainEvent in domainEvents)
        {
            await DispatchAsync(domainEvent, cancellationToken);
        }
    }
}