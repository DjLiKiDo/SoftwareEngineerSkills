namespace SoftwareEngineerSkills.Domain.Common.Interfaces;

/// <summary>
/// Interface for entities that track auditing information
/// </summary>
public interface IAuditableEntity
{
    /// <summary>
    /// Date when the entity was created
    /// </summary>
    DateTime Created { get; set; }

    /// <summary>
    /// User or system who created the entity
    /// </summary>
    string? CreatedBy { get; set; }

    /// <summary>
    /// Date when the entity was last modified
    /// </summary>
    DateTime? LastModified { get; set; }

    /// <summary>
    /// User or system who last modified the entity
    /// </summary>
    string? LastModifiedBy { get; set; }
}
