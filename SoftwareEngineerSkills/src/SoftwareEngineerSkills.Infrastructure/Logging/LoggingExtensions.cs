using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SoftwareEngineerSkills.Infrastructure.Configuration;

namespace SoftwareEngineerSkills.Infrastructure.Logging;

/// <summary>
/// Extension methods for configuring logging services
/// </summary>
public static class LoggingExtensions
{
    /// <summary>
    /// Adds logging services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddLoggingServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register and validate logging settings
        services
            .AddSettings<LoggingSettings>(configuration, LoggingSettings.SectionName)
            .Validate(settings => settings.Validate(out _), "Logging settings validation failed");
        
        // Configure logging using the IOptions pattern correctly
        services.AddLogging(builder =>
        {
            // Add configuration
            builder.AddConfiguration(configuration.GetSection("Logging"));
            
            // Add console provider
            builder.AddConsole();
            
            // Configure logging after the settings are resolved at runtime
            services.AddSingleton<IConfigureOptions<LoggerFilterOptions>>(serviceProvider =>
            {
                var loggingSettings = serviceProvider.GetRequiredService<IOptions<LoggingSettings>>().Value;
                
                return new ConfigureNamedOptions<LoggerFilterOptions>(
                    Options.DefaultName, 
                    options =>
                    {
                        // Set minimum log level
                        options.MinLevel = ParseLogLevel(loggingSettings.DefaultLogLevel);
                        
                        // Configure entity framework logging
                        if (loggingSettings.EnableEFCoreLogging)
                        {
                            options.Rules.Add(new LoggerFilterRule(
                                "Microsoft.EntityFrameworkCore.Database.Command", 
                                null, 
                                LogLevel.Information, 
                                null));
                        }
                        else
                        {
                            options.Rules.Add(new LoggerFilterRule(
                                "Microsoft.EntityFrameworkCore", 
                                null, 
                                LogLevel.Warning, 
                                null));
                        }
                    });
            });
        });
        
        return services;
    }
    
    /// <summary>
    /// Parses a string log level to the LogLevel enum
    /// </summary>
    /// <param name="logLevelString">The log level string</param>
    /// <returns>The LogLevel enum value</returns>
    private static LogLevel ParseLogLevel(string logLevelString)
    {
        return Enum.TryParse<LogLevel>(logLevelString, true, out var logLevel)
            ? logLevel
            : LogLevel.Information;
    }
}
