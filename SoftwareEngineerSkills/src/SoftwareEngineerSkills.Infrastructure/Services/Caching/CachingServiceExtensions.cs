using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SoftwareEngineerSkills.Infrastructure.Configuration;

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
        // Configure Caching with validation
        services
            .AddSettings<CacheSettings>(configuration, CacheSettings.SectionName)
            .Validate(settings => settings.Validate(out _), "Cache settings validation failed");
        
        // Register base caching services
        services.AddMemoryCache();
        services.AddDistributedMemoryCache();
        
        // Configure the appropriate cache implementation based on settings
        services.AddSingleton<ICacheService>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<CacheSettings>>();
            var cacheSettings = options.Value;
            
            // Choose the appropriate implementation based on the provider setting
            if (cacheSettings.Provider.Equals("redis", StringComparison.OrdinalIgnoreCase) ||
                cacheSettings.Provider.Equals("distributed", StringComparison.OrdinalIgnoreCase))
            {
                var distributedCache = serviceProvider.GetRequiredService<IDistributedCache>();
                var logger = serviceProvider.GetRequiredService<ILogger<DistributedCacheService>>();
                
                return new DistributedCacheService(distributedCache, options, logger);
            }
            else
            {
                var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
                var logger = serviceProvider.GetRequiredService<ILogger<MemoryCacheService>>();
                
                return new MemoryCacheService(memoryCache, options, logger);
            }
        });
        
        // Configure Redis if needed based on settings
        services.PostConfigure<CacheSettings>(settings =>
        {
            if (settings.Provider.Equals("redis", StringComparison.OrdinalIgnoreCase) && 
                !string.IsNullOrEmpty(settings.RedisConnectionString))
            {
                // In a real implementation, we would register Redis here
                // services.AddStackExchangeRedisCache(options => 
                // {
                //     options.Configuration = settings.RedisConnectionString;
                //     options.InstanceName = settings.RedisInstanceName;
                // });
            }
        });
        
        return services;
    }
}
