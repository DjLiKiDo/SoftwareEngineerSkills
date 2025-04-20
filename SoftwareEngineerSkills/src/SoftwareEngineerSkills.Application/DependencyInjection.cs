using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SoftwareEngineerSkills.Application.Common.Behaviours;
using System.Reflection;

namespace SoftwareEngineerSkills.Application;

/// <summary>
/// Extension methods for the Application layer's dependency injection setup
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds application services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register MediatR - scan the assembly for all handlers
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        // Register MediatR pipeline behaviors (order matters)
        // Exception handling should be first to catch all exceptions
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
        // Logging should happen after exception handling
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
        // Validation should happen before unit of work
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        // Unit of work should manage transactions for commands
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkBehaviour<,>));
        // Domain event dispatcher should happen after unit of work
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(DomainEventDispatcherBehaviour<,>));
        // Domain event publishing should happen last (maintaining existing behavior)
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(DomainEventPublishingBehaviour<,>));

        // Register FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Register AutoMapper with all profiles from the assembly
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }
}
