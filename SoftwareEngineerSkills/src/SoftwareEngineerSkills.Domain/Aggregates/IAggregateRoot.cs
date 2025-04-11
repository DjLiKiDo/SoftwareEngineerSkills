using SoftwareEngineerSkills.Domain.Events;

namespace SoftwareEngineerSkills.Domain.Aggregates;

/// <summary>
/// Marker interface for aggregate roots in Domain-Driven Design
/// </summary>
public interface IAggregateRoot
{
    /// <summary>
    /// Gets the collection of domain events that have been raised but not yet committed
    /// </summary>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    
    /// <summary>
    /// Adds a domain event to the aggregate's collection of domain events
    /// </summary>
    /// <param name="domainEvent">The domain event to add</param>
    void AddDomainEvent(IDomainEvent domainEvent);
    
    /// <summary>
    /// Removes a domain event from the aggregate's collection of domain events
    /// </summary>
    /// <param name="domainEvent">The domain event to remove</param>
    void RemoveDomainEvent(IDomainEvent domainEvent);
    
    /// <summary>
    /// Clears all domain events from the aggregate's collection of domain events
    /// </summary>
    void ClearDomainEvents();
}
