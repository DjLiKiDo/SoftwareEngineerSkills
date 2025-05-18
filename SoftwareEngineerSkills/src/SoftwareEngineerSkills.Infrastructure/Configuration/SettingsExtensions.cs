using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SoftwareEngineerSkills.Infrastructure.Configuration;

/// <summary>
/// Extension methods for infrastructure settings registration
/// </summary>
public static class SettingsExtensions
{
    /// <summary>
    /// Adds settings to the service collection with validation and startup validation
    /// </summary>
    /// <typeparam name="TOptions">The settings type</typeparam>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <param name="sectionName">The configuration section name</param>
    /// <returns>The options builder for additional configuration</returns>
    public static OptionsBuilder<TOptions> AddSettings<TOptions>(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName)
        where TOptions : class, new()
    {
        return services
            .AddOptions<TOptions>()
            .Bind(configuration.GetSection(sectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }

    /// <summary>
    /// Adds settings to the service collection with validation, startup validation, and custom validation
    /// </summary>
    /// <typeparam name="TOptions">The settings type</typeparam>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <param name="sectionName">The configuration section name</param>
    /// <param name="validate">Custom validation function</param>
    /// <param name="failureMessage">Message to display if validation fails</param>
    /// <returns>The options builder for additional configuration</returns>
    public static OptionsBuilder<TOptions> AddSettings<TOptions>(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName,
        Func<TOptions, bool> validate,
        string failureMessage)
        where TOptions : class, new()
    {
        return services
            .AddOptions<TOptions>()
            .Bind(configuration.GetSection(sectionName))
            .ValidateDataAnnotations()
            .Validate(validate, failureMessage)
            .ValidateOnStart();
    }
}
