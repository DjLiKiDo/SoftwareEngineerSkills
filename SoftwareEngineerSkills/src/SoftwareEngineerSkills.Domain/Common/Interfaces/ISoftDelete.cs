namespace SoftwareEngineerSkills.Domain.Common.Interfaces;

/// <summary>
/// Interface for entities that track logical deletion information
/// </summary>
public interface ISoftDelete
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity is deleted
    /// </summary>
    bool IsDeleted { get; set; }
    
    /// <summary>
    /// Gets or sets the date when the entity was deleted
    /// </summary>
    DateTime? DeletedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the user or system who deleted the entity
    /// </summary>
    string? DeletedBy { get; set; }
}
