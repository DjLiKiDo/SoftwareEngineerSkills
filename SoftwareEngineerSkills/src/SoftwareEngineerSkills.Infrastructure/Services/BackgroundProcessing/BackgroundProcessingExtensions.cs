using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SoftwareEngineerSkills.Infrastructure.Configuration;

namespace SoftwareEngineerSkills.Infrastructure.Services.BackgroundProcessing;

/// <summary>
/// Extension methods for registering background processing services with the DI container
/// </summary>
public static class BackgroundProcessingExtensions
{
    /// <summary>
    /// Adds background processing services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddBackgroundProcessingServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register Background Processing settings with validation
        services.AddSettings<BackgroundTaskSettings>(configuration, BackgroundTaskSettings.SectionName)
            .Validate(settings => settings.Validate(out _), "Background task settings validation failed");

        // Register Background Task Queue using options pattern
        services.AddSingleton<IBackgroundTaskQueue>(sp => 
        {
            var settings = sp.GetRequiredService<IOptions<BackgroundTaskSettings>>().Value;
            return new BackgroundTaskQueue(settings.QueueCapacity);
        });
        
        services.AddHostedService<QueuedHostedService>();
        
        return services;
    }
}
