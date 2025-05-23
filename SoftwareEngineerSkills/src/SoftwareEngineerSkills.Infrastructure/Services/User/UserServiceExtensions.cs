using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SoftwareEngineerSkills.Domain.DomainServices.Interfaces;

namespace SoftwareEngineerSkills.Infrastructure.Services.User;

/// <summary>
/// Extension methods for registering user services in the DI container
/// </summary>
public static class UserServiceExtensions
{
    /// <summary>
    /// Adds user-related services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddUserServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register the HTTP context accessor if not already registered
        services.AddHttpContextAccessor();
        
        // Register the current user service
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        
        return services;
    }
}
