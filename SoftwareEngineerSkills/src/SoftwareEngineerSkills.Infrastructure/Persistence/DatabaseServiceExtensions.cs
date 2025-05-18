using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SoftwareEngineerSkills.Domain.Abstractions.Persistence;
using SoftwareEngineerSkills.Infrastructure.Configuration;
using SoftwareEngineerSkills.Infrastructure.Persistence.Repositories;

namespace SoftwareEngineerSkills.Infrastructure.Persistence;

/// <summary>
/// Extension methods for registering database services with the DI container
/// </summary>
public static class DatabaseServiceExtensions
{
    /// <summary>
    /// Adds database services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure database settings using our pattern with validation
        services.AddDatabaseSettings(configuration);
        
        // Register DbContext with access to options
        services.AddDbContext<ApplicationDbContext>((provider, options) =>
        {
            // Get database settings through DI properly at context creation time
            var dbSettings = provider.GetRequiredService<Microsoft.Extensions.Options.IOptionsMonitor<DatabaseSettings>>().CurrentValue;
            
            ConfigureDatabaseProvider(options, dbSettings, configuration);
            
            // Enable sensitive data logging and detailed errors
            if (dbSettings.EnableDetailedErrors)
            {
                options.EnableDetailedErrors();
            }
            
            if (dbSettings.EnableSensitiveDataLogging)
            {
                options.EnableSensitiveDataLogging();
            }
        });

        return services;
    }
    
    /// <summary>
    /// Adds database settings to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddDatabaseSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSettings<DatabaseSettings>(configuration, DatabaseSettings.SectionName)
            .Validate(settings =>
            {
                if (!settings.Validate(out var results))
                {
                    foreach (var result in results)
                    {
                        // The validation error messages will be reported during startup
                        // due to ValidateOnStart()
                    }
                    return false;
                }
                return true;
            }, "Database settings validation failed");
        
        return services;
    }

    /// <summary>
    /// Configures the database provider based on the settings
    /// </summary>
    /// <param name="options">The DbContext options builder</param>
    /// <param name="dbSettings">The database settings</param>
    /// <param name="configuration">The configuration</param>
    private static void ConfigureDatabaseProvider(
        DbContextOptionsBuilder options,
        DatabaseSettings dbSettings,
        IConfiguration configuration)
    {
        // Get the connection string from settings
        var connectionString = dbSettings.ConnectionString;

        // If connection string is empty, we have a validation issue
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Database connection string is not configured");
        }

        switch (dbSettings.Provider.ToLowerInvariant())
        {
            case "sqlserver":
                options.UseSqlServer(
                    connectionString,
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                        sqlOptions.EnableRetryOnFailure(
                            dbSettings.MaxRetryCount,
                            TimeSpan.FromSeconds(dbSettings.MaxRetryDelaySeconds),
                            null);
                    });
                break;

            case "postgres":
            case "postgresql":
                options.UseNpgsql(
                    connectionString,
                    npgsqlOptions =>
                    {
                        npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                        npgsqlOptions.EnableRetryOnFailure(
                            dbSettings.MaxRetryCount,
                            TimeSpan.FromSeconds(dbSettings.MaxRetryDelaySeconds),
                            null);
                    });
                break;

            case "inmemory":
            default:
                options.UseInMemoryDatabase("SoftwareEngineerSkillsDb");
                break;
        }
    }
    
    /// <summary>
    /// Adds repository services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        // Register repositories
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped<ISkillRepository, SkillRepository>();
        
        // Register UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return services;
    }
    
    /// <summary>
    /// Initializes the database with migrations and seed data
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <returns>The application builder for chaining</returns>
    public static async Task<IApplicationBuilder> InitializeDatabaseAsync(this IApplicationBuilder app)
    {
        // Initialize the database with migrations and seed data
        await DatabaseInitializer.SeedDatabaseAsync(app.ApplicationServices);

        return app;
    }
}
