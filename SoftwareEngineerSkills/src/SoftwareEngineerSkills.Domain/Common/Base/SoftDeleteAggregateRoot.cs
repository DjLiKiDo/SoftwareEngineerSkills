using SoftwareEngineerSkills.Domain.Common.Events;
using SoftwareEngineerSkills.Domain.Common.Interfaces;

namespace SoftwareEngineerSkills.Domain.Common.Base;

/// <summary>
/// Base class for aggregate roots that also support soft deletion.
/// This class combines the capabilities of AggregateRoot and SoftDeleteEntity.
/// </summary>
public abstract class SoftDeleteAggregateRoot : AggregateRoot, ISoftDelete
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity is deleted
    /// </summary>
    public bool IsDeleted { get; set; }
    
    /// <summary>
    /// Gets or sets the date when the entity was deleted
    /// </summary>
    public DateTime? DeletedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the user or system who deleted the entity
    /// </summary>
    public string? DeletedBy { get; set; }
    
    /// <summary>
    /// Marks the aggregate as deleted and raises a domain event
    /// </summary>
    /// <param name="deletedBy">The user or system who deleted the entity</param>
    public virtual void MarkAsDeleted(string? deletedBy)
    {
        if (!IsDeleted)
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            DeletedBy = deletedBy;
            
            var deletedEvent = new EntitySoftDeletedEvent(
                Id,
                GetType().Name,
                deletedBy ?? "system");
                
            AddAndApplyEvent(deletedEvent);
        }
    }
    
    /// <summary>
    /// Restores a soft-deleted aggregate and raises a domain event
    /// </summary>
    public virtual void Restore()
    {
        if (IsDeleted)
        {
            var previouslyDeletedBy = DeletedBy;
            
            IsDeleted = false;
            DeletedAt = null;
            DeletedBy = null;
            
            var restoredEvent = new EntityRestoredEvent(
                Id,
                GetType().Name,
                previouslyDeletedBy ?? "system");
                
            AddAndApplyEvent(restoredEvent);
        }
    }
    
    /// <summary>
    /// Updates the state of this aggregate root with a domain event
    /// </summary>
    /// <param name="domainEvent">The domain event that modifies the aggregate state</param>
    protected override void Apply(IDomainEvent domainEvent)
    {
        base.Apply(domainEvent);
        
        switch (domainEvent)
        {
            case EntitySoftDeletedEvent deleted:
                // The MarkAsDeleted method already sets the properties
                break;
                
            case EntityRestoredEvent restored:
                // The Restore method already sets the properties
                break;
        }
    }
}
