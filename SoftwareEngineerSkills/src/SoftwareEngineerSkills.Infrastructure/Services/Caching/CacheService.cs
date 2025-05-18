using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace SoftwareEngineerSkills.Infrastructure.Services.Caching;

/// <summary>
/// Configuration for cache service
/// </summary>
public class CacheSettings
{
    /// <summary>
    /// Default cache expiration time in minutes
    /// </summary>
    public int DefaultExpirationMinutes { get; set; } = 30;

    /// <summary>
    /// Whether to enable cache sliding expiration
    /// </summary>
    public bool EnableSlidingExpiration { get; set; } = true;

    /// <summary>
    /// Cache size limit in megabytes (for memory cache)
    /// </summary>
    public int SizeLimitInMB { get; set; } = 100;
}

/// <summary>
/// Interface for cache service
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Gets an item from the cache
    /// </summary>
    /// <typeparam name="T">The type of the item</typeparam>
    /// <param name="key">Cache key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The cached item, or default if not found</returns>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Sets an item in the cache
    /// </summary>
    /// <typeparam name="T">The type of the item</typeparam>
    /// <param name="key">Cache key</param>
    /// <param name="value">Item to cache</param>
    /// <param name="expirationMinutes">Expiration time in minutes, or null to use default</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SetAsync<T>(string key, T value, int? expirationMinutes = null, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Removes an item from the cache
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementation of cache service using IDistributedCache
/// </summary>
public class DistributedCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly CacheSettings _settings;
    private readonly ILogger<DistributedCacheService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Creates a new instance of DistributedCacheService
    /// </summary>
    /// <param name="cache">Distributed cache</param>
    /// <param name="options">Cache settings</param>
    /// <param name="logger">Logger</param>
    public DistributedCacheService(
        IDistributedCache cache,
        IOptions<CacheSettings> options,
        ILogger<DistributedCacheService> logger)
    {
        _cache = cache;
        _settings = options.Value;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    /// <inheritdoc />
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var data = await _cache.GetStringAsync(key, cancellationToken);
            if (data == null)
            {
                return null;
            }

            return JsonSerializer.Deserialize<T>(data, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error retrieving data from cache for key {Key}", key);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task SetAsync<T>(string key, T value, int? expirationMinutes = null, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
            
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expirationMinutes ?? _settings.DefaultExpirationMinutes)
            };
            
            if (_settings.EnableSlidingExpiration)
            {
                options.SlidingExpiration = TimeSpan.FromMinutes(Math.Min(expirationMinutes ?? _settings.DefaultExpirationMinutes, 20));
            }

            await _cache.SetStringAsync(key, serializedValue, options, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error setting data in cache for key {Key}", key);
        }
    }

    /// <inheritdoc />
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.RemoveAsync(key, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error removing data from cache for key {Key}", key);
        }
    }
}

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
