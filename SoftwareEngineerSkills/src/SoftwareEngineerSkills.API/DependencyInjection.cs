using SoftwareEngineerSkills.API.Extensions;
using SoftwareEngineerSkills.Application;
using SoftwareEngineerSkills.Infrastructure;

namespace SoftwareEngineerSkills.API;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddApplicationServices();
        services.AddInfrastructureServices();

        services.AddControllers();
        services.AddCustomOpenApi(); //Add custom instead of services.AddOpenApi();

        return services;
    }
}
