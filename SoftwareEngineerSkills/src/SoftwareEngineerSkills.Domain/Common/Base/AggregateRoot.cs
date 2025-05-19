using System.Collections.Concurrent;
using SoftwareEngineerSkills.Domain.Common.Events;
using SoftwareEngineerSkills.Domain.Exceptions;

namespace SoftwareEngineerSkills.Domain.Common.Base;

/// <summary>
/// Base class for aggregate roots in the domain model.
/// Aggregate roots are the primary entities that external objects reference.
/// They are responsible for enforcing invariants and consistency rules for
/// the aggregate as a whole, and they coordinate the lifecycle of child entities.
/// </summary>
public abstract class AggregateRoot : BaseEntity, IAggregateRoot
{
    private readonly object _domainEventLock = new();
    
    /// <summary>
    /// Adds a domain event to this aggregate root
    /// </summary>
    /// <param name="domainEvent">The domain event to add</param>
    protected new void AddDomainEvent(IDomainEvent domainEvent)
    {
        lock (_domainEventLock)
        {
            base.AddDomainEvent(domainEvent);
        }
        
        // After adding a domain event, increment the version
        IncrementVersion();
    }
    
    /// <summary>
    /// Updates the state of this aggregate root with a domain event
    /// </summary>
    /// <param name="domainEvent">The domain event that modifies the aggregate state</param>
    protected virtual void Apply(IDomainEvent domainEvent)
    {
        // Default implementation does nothing
        // Derived classes should override this method to apply the event to the aggregate state
    }
    
    /// <summary>
    /// Adds a domain event and applies its effects to this aggregate root
    /// </summary>
    /// <param name="domainEvent">The domain event to add and apply</param>
    protected void AddAndApplyEvent(IDomainEvent domainEvent)
    {
        Apply(domainEvent);
        AddDomainEvent(domainEvent);
        EnforceInvariants(); // Validate invariants after state change
    }

    /// <summary>
    /// Asynchronously adds a domain event and applies its effects to this aggregate root
    /// </summary>
    /// <param name="domainEvent">The domain event to add and apply</param>
    protected async Task AddAndApplyEventAsync(IDomainEvent domainEvent)
    {
        Apply(domainEvent);
        AddDomainEvent(domainEvent);
        await EnforceInvariantsAsync(); // Validate invariants after state change
    }
}
