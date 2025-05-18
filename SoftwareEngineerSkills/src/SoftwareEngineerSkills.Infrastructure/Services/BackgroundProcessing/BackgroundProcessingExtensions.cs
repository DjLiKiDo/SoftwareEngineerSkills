using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        // Register Background Processing
        var queueCapacity = configuration.GetValue<int>("BackgroundTasks:QueueCapacity", 100);
        services.AddSingleton<IBackgroundTaskQueue>(sp => new BackgroundTaskQueue(queueCapacity));
        services.AddHostedService<QueuedHostedService>();
        
        return services;
    }
}
