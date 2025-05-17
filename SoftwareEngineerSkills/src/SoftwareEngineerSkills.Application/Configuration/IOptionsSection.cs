using System.Diagnostics.CodeAnalysis;

namespace SoftwareEngineerSkills.Application.Configuration;

/// <summary>
/// Interface for application configuration options with a static section name.
/// </summary>
public interface IOptionsSection
{
    /// <summary>
    /// Gets the section name in configuration.
    /// </summary>
    static abstract string SectionName { get; }
}
