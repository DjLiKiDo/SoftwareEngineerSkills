using System.ComponentModel.DataAnnotations;

namespace SoftwareEngineerSkills.Infrastructure.Configuration;

/// <summary>
/// Base interface for infrastructure settings
/// </summary>
public interface IValidatableSettings
{
    /// <summary>
    /// Validates the settings
    /// </summary>
    /// <returns>True if settings are valid, otherwise false</returns>
    bool Validate(out ICollection<ValidationResult> validationResults);
}
