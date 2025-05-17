using System.Diagnostics.CodeAnalysis;

namespace SoftwareEngineerSkills.Application.Configuration;

/// <summary>
/// Base class for application configuration options.
/// </summary>
public abstract class BaseOptions
{
    /// <summary>
    /// Gets the section name in configuration.
    /// </summary>
    public abstract string SectionName { get; }
}
