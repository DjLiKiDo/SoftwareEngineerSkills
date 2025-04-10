using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace SoftwareEngineerSkills.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register MediatR - scan the assembly for all handlers
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        // Register AutoMapper with all profiles from the assembly
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }
}
