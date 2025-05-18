using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SoftwareEngineerSkills.Infrastructure.Services.Email;

/// <summary>
/// Extension methods for registering email services with the DI container
/// </summary>
public static class EmailServiceExtensions
{
    /// <summary>
    /// Adds email services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddEmailServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure Email Service
        services.Configure<EmailSettings>(configuration.GetSection("Email"));
        services.AddScoped<IEmailService, SmtpEmailService>();
        
        return services;
    }
}
