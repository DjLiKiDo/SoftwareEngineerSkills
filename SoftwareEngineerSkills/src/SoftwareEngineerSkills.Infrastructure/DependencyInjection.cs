using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SoftwareEngineerSkills.Application.Common.Events;
using SoftwareEngineerSkills.Application.Configuration;
using SoftwareEngineerSkills.Domain.Abstractions.Persistence;
using SoftwareEngineerSkills.Domain.Common;
using SoftwareEngineerSkills.Infrastructure.Configuration;
using SoftwareEngineerSkills.Infrastructure.Configuration.Validators;
using SoftwareEngineerSkills.Infrastructure.Persistence;
using SoftwareEngineerSkills.Infrastructure.Persistence.Repositories;
using SoftwareEngineerSkills.Infrastructure.Services;
using System;

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

        // Register DbContext with in-memory database
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseInMemoryDatabase("SoftwareEngineerSkillsDb");
        });

        // Register repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IDummyRepository, DummyRepository>();

        // Register UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register domain event dispatcher
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        return services;
    }
}
