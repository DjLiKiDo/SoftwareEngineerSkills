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
    public string ApplicationName { get; set; } = string.Empty;

    /// <summary>
    /// The environment in which the application is running
    /// </summary>
    public EnvironmentType Environment { get; set; } = EnvironmentType.Unknown;
}
