namespace SoftwareEngineerSkills.Domain.Common;

/// <summary>
/// Base record for implementing Value Objects
/// </summary>
public abstract record ValueObject
{
    // Records already implement value-based equality
    // This class serves as a marker type and for potential shared functionality
}