using SoftwareEngineerSkills.Domain.Events;

namespace SoftwareEngineerSkills.Domain.Common;

/// <summary>
/// Interface that defines common behavior for all entities
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Unique identifier for the entity
    /// </summary>
    Guid Id { get; }
    
    /// <summary>
    /// Date and time when the entity was created
    /// </summary>
    DateTime CreatedAt { get; }
    
    /// <summary>
    /// Date and time when the entity was last updated
    /// </summary>
    DateTime? UpdatedAt { get; }
    
    /// <summary>
    /// ID of the user who created the entity
    /// </summary>
    string? CreatedBy { get; }
    
    /// <summary>
    /// ID of the user who last modified the entity
    /// </summary>
    string? LastModifiedBy { get; }
    
    /// <summary>
    /// Gets the collection of domain events that have been raised but not yet published
    /// </summary>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    
    /// <summary>
    /// Adds a domain event to the entity's collection of domain events
    /// </summary>
    /// <param name="domainEvent">The domain event to add</param>
    void AddDomainEvent(IDomainEvent domainEvent);
    
    /// <summary>
    /// Removes a domain event from the entity's collection of domain events
    /// </summary>
    /// <param name="domainEvent">The domain event to remove</param>
    void RemoveDomainEvent(IDomainEvent domainEvent);
    
    /// <summary>
    /// Clears all domain events from the entity's collection of domain events
    /// </summary>
    void ClearDomainEvents();
}
