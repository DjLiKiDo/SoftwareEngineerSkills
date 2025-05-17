using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SoftwareEngineerSkills.Application.Common.Events;
using SoftwareEngineerSkills.Application.Configuration;
using SoftwareEngineerSkills.Domain.Abstractions.Persistence;
using SoftwareEngineerSkills.Domain.Abstractions.Services;
using SoftwareEngineerSkills.Infrastructure.Configuration;
using SoftwareEngineerSkills.Infrastructure.Configuration.Validators;
using SoftwareEngineerSkills.Infrastructure.Persistence;
using SoftwareEngineerSkills.Infrastructure.Persistence.Repositories;
using SoftwareEngineerSkills.Infrastructure.Services;

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
    /// <returns>The service collection for chaining</returns>    
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Register configuration
        services
            .AddOptions<ApplicationOptions>()
            .BindConfiguration(ApplicationOptions.SectionName)
            .ValidateDataAnnotations()  // Add data annotations validation
            .ValidateOnStart();

        // Register the AppSettingsValidator
        services.AddSingleton<IValidateOptions<ApplicationOptions>, AppSettingsValidator>();

        // Register configuration service
        services.AddSingleton<IApplicationService, ApplicationService>();

        // Register DateTimeProvider
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        // Register DbContext - uses in-memory database by default
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseInMemoryDatabase("SoftwareEngineerSkillsDb");
        });

        // Register repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IDummyRepository, DummyRepository>();

        // Register UnitOfWork - must be scoped to match DbContext lifetime
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register domain event dispatcher
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        return services;
    }

    /// <summary>
    /// Adds email services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    private static IServiceCollection AddEmailServices(this IServiceCollection services)
    {
        // Register Email configuration with validation
        services
            .AddOptions<EmailOptions>()
            .BindConfiguration(EmailOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Register custom validator for more complex validation rules
        services.AddSingleton<IValidateOptions<EmailOptions>, EmailOptionsValidator>();

        // Register email service
        services.AddSingleton<IEmailService, EmailService>();

        return services;
    }
}
