using SoftwareEngineerSkills.Domain.Common.Interfaces;

namespace SoftwareEngineerSkills.Domain.Common.Base;

/// <summary>
/// Base class for entities that support soft deletion.
/// 
/// This entity type is tracked in the database but not physically deleted
/// when Delete() is called. Instead, it's marked as deleted with metadata about
/// who deleted it and when.
/// 
/// Usage:
/// - Inherit from this class when your entity should support soft deletion
/// - Use ISoftDeleteRepository for repository implementations that filter deleted entities
/// - Can be combined with AggregateRoot by inheriting from SoftDeleteEntity and implementing IAggregateRoot
/// </summary>
public abstract class SoftDeleteEntity : BaseEntity, ISoftDelete
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
    /// Marks the entity as deleted
    /// </summary>
    /// <param name="deletedBy">The user or system who deleted the entity</param>
    public virtual void MarkAsDeleted(string? deletedBy)
    {
        if (!IsDeleted)
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            DeletedBy = deletedBy;
            
            // Note: Domain events for soft deletion should be handled by the derived aggregate roots
            // that inherit from this class. They should override this method and add domain events.
        }
    }
    
    /// <summary>
    /// Restores a soft-deleted entity
    /// </summary>
    public virtual void Restore()
    {
        if (IsDeleted)
        {
            IsDeleted = false;
            DeletedAt = null;
            DeletedBy = null;
            
            // Note: Domain events for restoration should be handled by the derived aggregate roots
            // that inherit from this class. They should override this method and add domain events.
        }
    }
}
