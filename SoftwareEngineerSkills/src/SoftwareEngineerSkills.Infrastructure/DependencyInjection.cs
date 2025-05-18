using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;
using SoftwareEngineerSkills.Domain.Abstractions.Persistence;
using SoftwareEngineerSkills.Infrastructure.Configuration;
using SoftwareEngineerSkills.Infrastructure.Logging;
using SoftwareEngineerSkills.Infrastructure.Persistence;
using SoftwareEngineerSkills.Infrastructure.Persistence.Repositories;
using SoftwareEngineerSkills.Infrastructure.Services.BackgroundProcessing;
using SoftwareEngineerSkills.Infrastructure.Services.Caching;
using SoftwareEngineerSkills.Infrastructure.Services.Email;
using System;
using System.Reflection;

namespace SoftwareEngineerSkills.Infrastructure;

/// <summary>
/// Extension methods for the Infrastructure layer's dependency injection setup
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds infrastructure services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>    
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register database services
        services.AddDatabaseServices(configuration);
        
        // Register repositories
        services.AddRepositoryServices();
          // Configure logging services
        services.AddLoggingServices(configuration);
        
        // Register external infrastructure services
        AddEmailServices(services, configuration);
        AddCachingServices(services, configuration);
        AddBackgroundProcessingServices(services, configuration);

        return services;
    }
    
    /// <summary>
    /// Adds database services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    private static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
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
    private static IServiceCollection AddRepositoryServices(this IServiceCollection services)
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
    
    /// <summary>
    /// Adds email services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    private static IServiceCollection AddEmailServices(IServiceCollection services, IConfiguration configuration)
    {
        // Configure Email Service
        services.Configure<EmailSettings>(configuration.GetSection("Email"));
        services.AddScoped<IEmailService, SmtpEmailService>();
        
        return services;
    }
    
    /// <summary>
    /// Adds caching services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    private static IServiceCollection AddCachingServices(IServiceCollection services, IConfiguration configuration)
    {
        // Configure Caching
        services.Configure<CacheSettings>(configuration.GetSection("Cache"));
        
        // Get cache settings from configuration
        var cacheSettings = configuration.GetSection("Cache").Get<CacheSettings>() ?? new CacheSettings();
        
        // Register different cache implementations based on configuration
        var cacheProvider = configuration.GetValue<string>("Cache:Provider")?.ToLowerInvariant() ?? "memory";
        
        switch (cacheProvider)
        {
            case "redis":
                ConfigureRedisCache(services, cacheSettings);
                break;
                
            case "distributed":
                ConfigureDistributedCache(services);
                break;
                
            case "memory":
            default:
                ConfigureMemoryCache(services, cacheSettings);
                break;
        }
        
        return services;
    }
    
    /// <summary>
    /// Configures Redis cache services
    /// </summary>
    private static void ConfigureRedisCache(IServiceCollection services, CacheSettings cacheSettings)
    {
        // If using Redis, you would need to add the Redis distributed cache implementation
        // services.AddStackExchangeRedisCache(options => { /* Configure Redis */ });
        services.AddSingleton<ICacheService, DistributedCacheService>();
    }
    
    /// <summary>
    /// Configures distributed memory cache services
    /// </summary>
    private static void ConfigureDistributedCache(IServiceCollection services)
    {
        services.AddDistributedMemoryCache(); // Use this for development/testing
        services.AddSingleton<ICacheService, DistributedCacheService>();
    }
    
    /// <summary>
    /// Configures in-memory cache services
    /// </summary>
    private static void ConfigureMemoryCache(IServiceCollection services, CacheSettings cacheSettings)
    {
        services.AddMemoryCache(options =>
        {
            if (cacheSettings.SizeLimitInMB > 0)
            {
                options.SizeLimit = cacheSettings.SizeLimitInMB * 1024 * 1024; // Convert MB to bytes
            }
        });
        services.AddSingleton<ICacheService, MemoryCacheService>();
    }
    
    /// <summary>
    /// Adds background processing services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    private static IServiceCollection AddBackgroundProcessingServices(IServiceCollection services, IConfiguration configuration)
    {
        // Register Background Processing
        var queueCapacity = configuration.GetValue<int>("BackgroundTasks:QueueCapacity", 100);
        services.AddSingleton<IBackgroundTaskQueue>(sp => new BackgroundTaskQueue(queueCapacity));
        services.AddHostedService<QueuedHostedService>();
        
        return services;
    }
}
