using System.ComponentModel.DataAnnotations;
using SoftwareEngineerSkills.Domain.Common.Enums;

namespace SoftwareEngineerSkills.Application.Configuration;

/// <summary>
/// Represents the application settings defined in appsettings.json
/// </summary>
public class AppSettings
{
    /// <summary>
    /// The name of the section in appsettings.json
    /// </summary>
    public const string SectionName = "AppSettings";

    /// <summary>
    /// The name of the application
    /// </summary>
    [Required(ErrorMessage = "Application name is required")]
    [StringLength(100, ErrorMessage = "Application name cannot exceed 100 characters")]
    public string ApplicationName { get; set; } = string.Empty;
    
    /// <summary>
    /// The environment in which the application is running
    /// </summary>
    [Required(ErrorMessage = "Environment is required")]
    [EnumDataType(typeof(EnvironmentType), ErrorMessage = "Environment must be a valid environment type")]
    public EnvironmentType Environment { get; set; } = EnvironmentType.Unknown;

    /// <summary>
    /// Dummy settings for demonstration purposes
    /// </summary>
    [Required(ErrorMessage = "Dummy settings are required")]
    [ValidateObject(ErrorMessage = "Dummy settings validation failed")]
    public DummySettings DummySettings { get; set; } = new DummySettings();
}

/// <summary>
/// Validates all properties of the decorated object
/// </summary>
public class ValidateObjectAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return new ValidationResult("Object cannot be null");
        }
        var results = new List<ValidationResult>();
        var context = new ValidationContext(value, null, null);
        Validator.TryValidateObject(value, context, results, true);
        if (results.Count != 0)
        {
            var compositeResults = string.Join(" ", results.Select(r => r.ErrorMessage));
            return new ValidationResult(compositeResults);
        }
        return ValidationResult.Success;
    }
}
