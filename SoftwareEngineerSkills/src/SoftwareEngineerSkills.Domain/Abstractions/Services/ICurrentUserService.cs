namespace SoftwareEngineerSkills.Domain.Abstractions.Services;

/// <summary>
/// Service for providing the current user information for auditing
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the ID of the current user
    /// </summary>
    string? UserId { get; }
    
    /// <summary>
    /// Gets the name of the current user
    /// </summary>
    string? UserName { get; }
    
    /// <summary>
    /// Gets a value indicating whether the current user is authenticated
    /// </summary>
    bool IsAuthenticated { get; }
}
