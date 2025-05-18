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
        // Configure database settings
        services.Configure<DatabaseSettings>(configuration.GetSection("Database"));
        services.AddSingleton(sp => sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<DatabaseSettings>>().Value);
        
        // Get database settings from configuration
        var dbSettings = configuration.GetSection("Database").Get<DatabaseSettings>() 
                       ?? new DatabaseSettings();
        
        // Register DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
        {
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
        switch (dbSettings.Provider.ToLowerInvariant())
        {
            case "sqlserver":
                options.UseSqlServer(
                    dbSettings.ConnectionString ?? configuration.GetConnectionString("DefaultConnection"),
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
                    dbSettings.ConnectionString ?? configuration.GetConnectionString("DefaultConnection"),
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
