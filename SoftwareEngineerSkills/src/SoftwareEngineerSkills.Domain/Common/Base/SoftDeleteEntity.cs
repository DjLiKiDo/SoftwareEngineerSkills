using SoftwareEngineerSkills.Domain.Common.Interfaces;

namespace SoftwareEngineerSkills.Domain.Common.Base;

/// <summary>
/// Base class for entities that support soft deletion
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
    public void MarkAsDeleted(string? deletedBy)
    {
        if (!IsDeleted)
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            DeletedBy = deletedBy;
        }
    }
    
    /// <summary>
    /// Restores a soft-deleted entity
    /// </summary>
    public void Restore()
    {
        if (IsDeleted)
        {
            IsDeleted = false;
            DeletedAt = null;
            DeletedBy = null;
        }
    }
}
