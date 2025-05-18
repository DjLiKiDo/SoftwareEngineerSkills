using SoftwareEngineerSkills.Infrastructure.Configuration;
using System.ComponentModel.DataAnnotations;

namespace SoftwareEngineerSkills.Infrastructure.Logging;

/// <summary>
/// Configuration for logging services
/// </summary>
public class LoggingSettings : IValidatableSettings
{
    /// <summary>
    /// The configuration section name
    /// </summary>
    public const string SectionName = "Logging";

    /// <summary>
    /// Gets or sets whether to enable Entity Framework Core SQL logging
    /// </summary>
    public bool EnableEFCoreLogging { get; set; } = false;

    /// <summary>
    /// Gets or sets the default minimum log level
    /// </summary>
    public string DefaultLogLevel { get; set; } = "Information";

    /// <summary>
    /// Gets or sets whether to enable structured logging
    /// </summary>
    public bool EnableStructuredLogging { get; set; } = true;

    /// <summary>
    /// Validates the settings
    /// </summary>
    /// <returns>True if settings are valid, otherwise false</returns>
    public bool Validate(out ICollection<ValidationResult> validationResults)
    {
        validationResults = new List<ValidationResult>();

        // Validate log level
        var validLogLevels = new[] { "Trace", "Debug", "Information", "Warning", "Error", "Critical", "None" };
        if (!validLogLevels.Contains(DefaultLogLevel, StringComparer.OrdinalIgnoreCase))
        {
            validationResults.Add(new ValidationResult(
                $"Default log level must be one of: {string.Join(", ", validLogLevels)}",
                new[] { nameof(DefaultLogLevel) }));
        }

        return validationResults.Count == 0;
    }
}
