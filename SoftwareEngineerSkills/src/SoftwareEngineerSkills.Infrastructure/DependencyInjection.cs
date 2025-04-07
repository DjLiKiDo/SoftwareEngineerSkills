using Microsoft.Extensions.DependencyInjection;
using SoftwareEngineerSkills.Application.Configuration;
using SoftwareEngineerSkills.Infrastructure.Configuration;

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

        // Register configuration service
        services.AddSingleton<IAppSettingsService, AppSettingsService>();

        // TODO: Add other infrastructure services here

        return services;
    }
}
