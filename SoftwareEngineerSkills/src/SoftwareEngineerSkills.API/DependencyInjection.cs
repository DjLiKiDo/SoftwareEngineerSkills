using SoftwareEngineerSkills.Application;
using SoftwareEngineerSkills.Infrastructure;

namespace SoftwareEngineerSkills.API;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationServices();
        services.AddInfrastructureServices(configuration);

        services.AddControllers();

        return services;
    }
}
