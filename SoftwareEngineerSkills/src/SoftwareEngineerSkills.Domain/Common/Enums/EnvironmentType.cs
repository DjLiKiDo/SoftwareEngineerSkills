namespace SoftwareEngineerSkills.Domain.Common.Enums;

/// <summary>
/// Represents the different environment types the application can run in
/// </summary>
public enum EnvironmentType
{
    /// <summary>
    /// Development environment
    /// </summary>
    Development,
    
    /// <summary>
    /// Staging environment
    /// </summary>
    Staging,
    
    /// <summary>
    /// Production environment
    /// </summary>
    Production,
    
    /// <summary>
    /// Unknown environment (default value if not specified)
    /// </summary>
    Unknown
}
