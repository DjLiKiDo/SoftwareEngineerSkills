using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SoftwareEngineerSkills.Infrastructure.Logging;
using SoftwareEngineerSkills.Infrastructure.Persistence;
using SoftwareEngineerSkills.Infrastructure.Services.BackgroundProcessing;
using SoftwareEngineerSkills.Infrastructure.Services.Caching;
using SoftwareEngineerSkills.Infrastructure.Services.Email;
using SoftwareEngineerSkills.Infrastructure.Services.User;

namespace SoftwareEngineerSkills.Infrastructure;

/// <summary>
/// Extension methods for the Infrastructure layer's dependency injection setup
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds infrastructure services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>    
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register database services
        services.AddDatabaseServices(configuration);
        
        // Register repositories
        services.AddRepositoryServices();
          // Configure logging services
        services.AddLoggingServices(configuration);
        
        // Register external infrastructure services
        services.AddEmailServices(configuration);
        services.AddCachingServices(configuration);
        services.AddBackgroundProcessingServices(configuration);
        
        // Register user services (for auditing)
        services.AddUserServices(configuration);

        return services;
    }
}
