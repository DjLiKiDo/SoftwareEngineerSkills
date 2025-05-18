using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
        services.AddLogging(builder =>
        {
            // Configure minimum log levels
            builder.SetMinimumLevel(LogLevel.Information);
            
            // Configure entity framework logging
            var enableEfLogging = configuration.GetValue<bool>("Logging:EnableEFCoreLogging");
            if (enableEfLogging)
            {
                builder.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Information);
            }
            else
            {
                builder.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
            }
            
            // Configure additional providers (in a real application)
            // This could integrate with Serilog, Application Insights, etc.
        });
        
        return services;
    }
}
