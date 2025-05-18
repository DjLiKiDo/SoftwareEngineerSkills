using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SoftwareEngineerSkills.Infrastructure.Services.Caching;

/// <summary>
/// Extension methods for registering caching services with the DI container
/// </summary>
public static class CachingServiceExtensions
{
    /// <summary>
    /// Adds caching services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddCachingServices(this IServiceCollection services, IConfiguration configuration)
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
}
