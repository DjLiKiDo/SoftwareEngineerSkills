using SoftwareEngineerSkills.Domain.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace SoftwareEngineerSkills.Application.Configuration;

/// <summary>
/// Represents the application settings defined in appsettings.json
/// </summary>
public class ApplicationOptions : BaseOptions
{
    /// <summary>
    /// The name of the section in appsettings.json
    /// </summary>
    public const string SectionNameConst = "ApplicationSettings";

    /// <inheritdoc />
    public override string SectionName => SectionNameConst;

    /// <summary>
    /// The name of the application
    /// </summary>
    [Required(ErrorMessage = "Application name is required")]
    [StringLength(100, ErrorMessage = "Application name cannot exceed 100 characters")]
    public required string ApplicationName { get; set; }

    /// <summary>
    /// The environment in which the application is running
    /// </summary>
    [Required(ErrorMessage = "Environment type is required")]
    public required EnvironmentType Environment { get; set; }
}
