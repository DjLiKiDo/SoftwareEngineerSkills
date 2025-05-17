using Microsoft.Extensions.DependencyInjection;
using SoftwareEngineerSkills.API.Extensions;
using SoftwareEngineerSkills.Application;
using SoftwareEngineerSkills.Infrastructure;

namespace SoftwareEngineerSkills.API;

public static class DependencyInjection
{    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddApplicationServices();
        services.AddInfrastructureServices();
        services.AddEmailServices(); // TODO: Move this to applicationServices// Add email services

        services.AddControllers();
        services.AddCustomOpenApi(); //Add custom instead of services.AddOpenApi();

        return services;
    }
}
