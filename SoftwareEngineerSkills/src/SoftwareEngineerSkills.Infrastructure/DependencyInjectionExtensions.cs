using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SoftwareEngineerSkills.Application.Configuration;
using SoftwareEngineerSkills.Infrastructure.Configuration.Validators;
using SoftwareEngineerSkills.Infrastructure.Services;

namespace SoftwareEngineerSkills.Infrastructure;

/// <summary>
/// Additional extension methods for the Infrastructure layer's dependency injection setup
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds email services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddEmailServices(this IServiceCollection services)
    {
        // Register Email configuration with validation
        services
            .AddOptions<EmailOptions>()
            .BindConfiguration(EmailOptions.SectionName)
            .ValidateDataAnnotations()
            .Validate(options => {
                // Custom validation: if username is provided, password must also be provided
                if (!string.IsNullOrEmpty(options.Username) && string.IsNullOrEmpty(options.Password))
                {
                    return false;
                }
                return true;
            }, "Password must be provided when username is specified.")
            .ValidateOnStart();

        // Register custom validator for more complex validation rules
        services.AddSingleton<IValidateOptions<EmailOptions>, EmailOptionsValidator>();

        // Register email service
        services.AddSingleton<IEmailService, EmailService>();

        return services;
    }
}
