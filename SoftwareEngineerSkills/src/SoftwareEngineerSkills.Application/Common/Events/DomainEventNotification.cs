using MediatR;
using SoftwareEngineerSkills.Domain.Events;

namespace SoftwareEngineerSkills.Application.Common.Events;

/// <summary>
/// Generic adapter that wraps domain events as MediatR notifications
/// </summary>
/// <typeparam name="TDomainEvent">The type of domain event being wrapped</typeparam>
public class DomainEventNotification<TDomainEvent> : INotification where TDomainEvent : IDomainEvent
{
    /// <summary>
    /// Gets the domain event being wrapped
    /// </summary>
    public TDomainEvent DomainEvent { get; }

    /// <summary>
    /// Creates a new instance of the DomainEventNotification class
    /// </summary>
    /// <param name="domainEvent">The domain event to wrap</param>
    public DomainEventNotification(TDomainEvent domainEvent)
    {
        DomainEvent = domainEvent;
    }
}