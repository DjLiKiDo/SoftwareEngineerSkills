using SoftwareEngineerSkills.Domain.Common.Events;

namespace SoftwareEngineerSkills.Domain.Common.Base;

/// <summary>
/// Interface to identify aggregate roots in the domain model.
/// Aggregate roots are the primary entities that external objects reference.
/// They are responsible for enforcing invariants and consistency rules for
/// the aggregate as a whole, and they coordinate the lifecycle of child entities.
/// </summary>
public interface IAggregateRoot
{
    /// <summary>
    /// Gets the domain events raised by this aggregate root
    /// </summary>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    
    /// <summary>
    /// Gets the version of the aggregate root for optimistic concurrency control
    /// </summary>
    int Version { get; }
    
    /// <summary>
    /// Removes a domain event from the aggregate root
    /// </summary>
    /// <param name="domainEvent">The domain event to remove</param>
    void RemoveDomainEvent(IDomainEvent domainEvent);
    
    /// <summary>
    /// Clears all domain events from the aggregate root
    /// </summary>
    void ClearDomainEvents();
    
    /// <summary>
    /// Enforces that all invariants of the aggregate root are satisfied
    /// </summary>
    void EnforceInvariants();
    
    /// <summary>
    /// Asynchronously enforces that all invariants of the aggregate root are satisfied
    /// </summary>
    Task EnforceInvariantsAsync();
}
