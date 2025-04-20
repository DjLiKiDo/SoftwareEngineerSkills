namespace SoftwareEngineerSkills.Domain.Abstractions.Services;

/// <summary>
/// Provides methods to work with date and time.
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// Gets the current date and time.
    /// </summary>
    DateTimeOffset Now { get; }

    /// <summary>
    /// Gets the current date and time in UTC.
    /// </summary>
    DateTime UtcNow { get; }
}
