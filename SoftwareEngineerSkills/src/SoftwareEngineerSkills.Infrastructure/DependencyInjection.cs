using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SoftwareEngineerSkills.Application.Configuration;
using SoftwareEngineerSkills.Domain.Abstractions.Persistence;
using SoftwareEngineerSkills.Infrastructure.Configuration;
using SoftwareEngineerSkills.Infrastructure.Configuration.Validators;
using SoftwareEngineerSkills.Infrastructure.Persistence.Repositories;

namespace SoftwareEngineerSkills.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Register configuration
        services
            .AddOptions<AppSettings>()
            .BindConfiguration(AppSettings.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Register the AppSettingsValidator
        services.AddSingleton<IValidateOptions<AppSettings>, AppSettingsValidator>();

        // Register configuration service
        services.AddSingleton<IAppSettingsService, AppSettingsService>();

        // Register repositories
        services.AddSingleton<IDummyRepository, DummyRepository>();

        return services;
    }
}
