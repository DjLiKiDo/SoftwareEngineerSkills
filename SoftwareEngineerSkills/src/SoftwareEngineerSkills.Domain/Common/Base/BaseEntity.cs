using System.ComponentModel.DataAnnotations.Schema;
using SoftwareEngineerSkills.Domain.Common.Events;
using SoftwareEngineerSkills.Domain.Common.Interfaces;

namespace SoftwareEngineerSkills.Domain.Common.Base;

/// <summary>
/// Base class for all entities in the domain model
/// </summary>
public abstract class BaseEntity : IAuditableEntity
{
    /// <summary>
    /// Unique identifier for the entity
    /// </summary>
    public Guid Id { get; protected set; }

    protected BaseEntity()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Date when the entity was created
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// User or system who created the entity
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Date when the entity was last modified
    /// </summary>
    public DateTime? LastModified { get; set; }

    /// <summary>
    /// User or system who last modified the entity
    /// </summary>
    public string? LastModifiedBy { get; set; }

    /// <summary>
    /// Collection of domain events raised by this entity
    /// </summary>
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// Read-only collection of domain events for external consumption
    /// </summary>
    [NotMapped]
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Adds a domain event to the entity
    /// </summary>
    /// <param name="domainEvent">The domain event to add</param>
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Removes a domain event from the entity
    /// </summary>
    /// <param name="domainEvent">The domain event to remove</param>
    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    /// <summary>
    /// Clears all domain events from the entity
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
