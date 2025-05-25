using SoftwareEngineerSkills.Domain.Common.Events;

namespace SoftwareEngineerSkills.Domain.Common.Interfaces;

/// <summary>
/// Defines the contract for aggregate roots in Domain-Driven Design.
/// Aggregate roots are the only entities that external objects can reference and serve as consistency boundaries.
/// </summary>
/// <remarks>
/// Only aggregate roots should have repositories. Child entities are accessed through their aggregate root.
/// </remarks>
/// <seealso cref="AggregateRoot"/>
/// <seealso cref="IDomainEvent"/>
public interface IAggregateRoot
{
    /// <summary>
    /// Gets the collection of domain events that have been raised by this aggregate root.
    /// </summary>
    /// <remarks>
    /// Events should be dispatched after successful persistence and then cleared to prevent duplicate processing.
    /// </remarks>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    
    /// <summary>
    /// Gets the version number for optimistic concurrency control.
    /// </summary>
    /// <remarks>
    /// Used by Entity Framework as a concurrency token to prevent lost updates.
    /// </remarks>
    int Version { get; }
    
    /// <summary>
    /// Removes a specific domain event from the aggregate root's event collection.
    /// </summary>
    /// <param name="domainEvent">The domain event to remove.</param>
    void RemoveDomainEvent(IDomainEvent domainEvent);
    
    /// <summary>
    /// Clears all domain events from the aggregate root's event collection.
    /// </summary>
    /// <remarks>
    /// Should be called after events have been successfully dispatched.
    /// </remarks>
    void ClearDomainEvents();
    
    /// <summary>
    /// Validates and enforces all business invariants for this aggregate root.
    /// </summary>
    /// <exception cref="DomainValidationException">Thrown when business invariants are violated.</exception>
    void EnforceInvariants();
    
    /// <summary>
    /// Asynchronously validates and enforces all business invariants for this aggregate root.
    /// </summary>
    /// <returns>A task that represents the asynchronous validation operation.</returns>
    /// <exception cref="DomainValidationException">Thrown when business invariants are violated.</exception>
    Task EnforceInvariantsAsync();
}
