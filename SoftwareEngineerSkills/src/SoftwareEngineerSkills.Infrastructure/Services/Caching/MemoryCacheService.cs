using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SoftwareEngineerSkills.Infrastructure.Services.Caching;

/// <summary>
/// Implementation of cache service using IMemoryCache
/// </summary>
public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly CacheSettings _settings;
    private readonly ILogger<MemoryCacheService> _logger;

    /// <summary>
    /// Creates a new instance of MemoryCacheService
    /// </summary>
    /// <param name="cache">Memory cache</param>
    /// <param name="options">Cache settings</param>
    /// <param name="logger">Logger</param>
    public MemoryCacheService(
        IMemoryCache cache,
        IOptions<CacheSettings> options,
        ILogger<MemoryCacheService> logger)
    {
        _cache = cache;
        _settings = options.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            if (_cache.TryGetValue(key, out T? value))
            {
                return Task.FromResult(value);
            }
            
            return Task.FromResult<T?>(null);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error retrieving data from memory cache for key {Key}", key);
            return Task.FromResult<T?>(null);
        }
    }

    /// <inheritdoc />
    public Task SetAsync<T>(string key, T value, int? expirationMinutes = null, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expirationMinutes ?? _settings.DefaultExpirationMinutes),
                Size = 1 // Size can be used with a size-limited cache
            };

            if (_settings.EnableSlidingExpiration)
            {
                options.SlidingExpiration = TimeSpan.FromMinutes(Math.Min(expirationMinutes ?? _settings.DefaultExpirationMinutes, 20));
            }

            _cache.Set(key, value, options);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error setting data in memory cache for key {Key}", key);
            return Task.CompletedTask;
        }
    }

    /// <inheritdoc />
    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            _cache.Remove(key);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error removing data from memory cache for key {Key}", key);
            return Task.CompletedTask;
        }
    }
}
