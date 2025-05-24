using System.ComponentModel.DataAnnotations;
using SoftwareEngineerSkills.Infrastructure.Configuration;

namespace SoftwareEngineerSkills.Infrastructure.Persistence;

/// <summary>
/// Database settings
/// </summary>
public class DatabaseSettings : IValidatableSettings
{
    /// <summary>
    /// The configuration section name
    /// </summary>
    public const string SectionName = "Database";

    /// <summary>
    /// Gets or sets the database provider
    /// </summary>
    [Required(ErrorMessage = "Database provider is required")]
    public string Provider { get; set; } = "inmemory";

    /// <summary>
    /// Gets or sets the connection string
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether to enable detailed errors
    /// </summary>
    public bool EnableDetailedErrors { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to enable sensitive data logging
    /// </summary>
    public bool EnableSensitiveDataLogging { get; set; } = false;

    /// <summary>
    /// Gets or sets the maximum retry count
    /// </summary>
    public int MaxRetryCount { get; set; } = 5;

    /// <summary>
    /// Gets or sets the maximum retry delay in seconds
    /// </summary>
    public int MaxRetryDelaySeconds { get; set; } = 30;
    
    /// <summary>
    /// Validates the settings
    /// </summary>
    /// <returns>True if settings are valid, otherwise false</returns>
    public bool Validate(out ICollection<ValidationResult> validationResults)
    {
        validationResults = new List<ValidationResult>();

        // For sqlserver and postgresql, connection string must be provided
        if ((Provider.Equals("sqlserver", StringComparison.OrdinalIgnoreCase) || 
             Provider.Equals("postgresql", StringComparison.OrdinalIgnoreCase)) && 
             string.IsNullOrEmpty(ConnectionString))
        {
            validationResults.Add(new ValidationResult(
                "ConnectionString is required when Provider is 'sqlserver' or 'postgresql'",
                new[] { nameof(ConnectionString) }));
        }

        // Check for valid MaxRetryCount
        if (MaxRetryCount < 0)
        {
            validationResults.Add(new ValidationResult(
                "MaxRetryCount must be greater than or equal to 0",
                new[] { nameof(MaxRetryCount) }));
        }

        // Check for valid MaxRetryDelaySeconds
        if (MaxRetryDelaySeconds < 1)
        {
            validationResults.Add(new ValidationResult(
                "MaxRetryDelaySeconds must be greater than 0",
                new[] { nameof(MaxRetryDelaySeconds) }));
        }

        return validationResults.Count == 0;
    }
}
