using SoftwareEngineerSkills.Domain.Common.Events;
using SoftwareEngineerSkills.Domain.Common.Interfaces;
using SoftwareEngineerSkills.Domain.Exceptions;

namespace SoftwareEngineerSkills.Domain.Common.Base;

/// <summary>
/// Base class for aggregate roots implementing Domain-Driven Design patterns.
/// Provides thread-safe domain event handling and enhanced event sourcing support.
/// </summary>
/// <remarks>
/// Thread-safe: Multiple threads can safely add events concurrently.
/// Only aggregate roots should have repositories.
/// </remarks>
public abstract class AggregateRoot : BaseEntity, IAggregateRoot
{
    private readonly object _domainEventLock = new();
    
    /// <summary>
    /// Adds a domain event in a thread-safe manner and increments version.
    /// </summary>
    /// <param name="domainEvent">The domain event to add.</param>
    protected new void AddDomainEvent(IDomainEvent domainEvent)
    {
        lock (_domainEventLock)
        {
            base.AddDomainEvent(domainEvent);
        }
        
        IncrementVersion();
    }
    
    /// <summary>
    /// Applies the effects of a domain event to the aggregate's state.
    /// Override in derived classes to implement event-driven state changes.
    /// </summary>
    /// <param name="domainEvent">The domain event to apply.</param>
    protected virtual void Apply(IDomainEvent domainEvent)
    {
        // Default implementation does nothing
    }
    
    /// <summary>
    /// Applies a domain event to state and adds it to the event collection atomically.
    /// Automatically enforces invariants after applying the event.
    /// </summary>
    /// <param name="domainEvent">The domain event to apply and add.</param>
    /// <exception cref="DomainValidationException">Thrown when invariants are violated.</exception>
    protected void AddAndApplyEvent(IDomainEvent domainEvent)
    {
        Apply(domainEvent);
        AddDomainEvent(domainEvent);
        EnforceInvariants();
    }

    /// <summary>
    /// Asynchronously applies a domain event to state and adds it to the event collection.
    /// Supports async invariant validation.
    /// </summary>
    /// <param name="domainEvent">The domain event to apply and add.</param>
    /// <exception cref="DomainValidationException">Thrown when invariants are violated.</exception>
    protected async Task AddAndApplyEventAsync(IDomainEvent domainEvent)
    {
        Apply(domainEvent);
        AddDomainEvent(domainEvent);
        await EnforceInvariantsAsync();
    }

    /// <summary>
    /// Thread-safe removal of a domain event.
    /// </summary>
    /// <param name="domainEvent">The domain event to remove.</param>
    public new void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        lock (_domainEventLock)
        {
            base.RemoveDomainEvent(domainEvent);
        }
    }

    /// <summary>
    /// Thread-safe clearing of all domain events.
    /// </summary>
    public new void ClearDomainEvents()
    {
        lock (_domainEventLock)
        {
            base.ClearDomainEvents();
        }
    }

    /// <summary>
    /// Asynchronously enforces business invariants.
    /// </summary>
    /// <exception cref="DomainValidationException">Thrown when invariants are violated.</exception>
    public Task EnforceInvariantsAsync()
    {
        return base.EnforceInvariantsAsync();
    }
}
