using SoftwareEngineerSkills.Infrastructure.Configuration;
using System.ComponentModel.DataAnnotations;

namespace SoftwareEngineerSkills.Infrastructure.Services.BackgroundProcessing;

/// <summary>
/// Configuration for background task processing
/// </summary>
public class BackgroundTaskSettings : IValidatableSettings
{
    /// <summary>
    /// The configuration section name
    /// </summary>
    public const string SectionName = "BackgroundTasks";

    /// <summary>
    /// Gets or sets the capacity of the background task queue
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Queue capacity must be greater than 0")]
    public int QueueCapacity { get; set; } = 100;

    /// <summary>
    /// Validates the settings
    /// </summary>
    /// <returns>True if settings are valid, otherwise false</returns>
    public bool Validate(out ICollection<ValidationResult> validationResults)
    {
        validationResults = new List<ValidationResult>();

        if (QueueCapacity <= 0)
        {
            validationResults.Add(new ValidationResult(
                "Queue capacity must be greater than 0",
                new[] { nameof(QueueCapacity) }));
        }

        return validationResults.Count == 0;
    }
}
