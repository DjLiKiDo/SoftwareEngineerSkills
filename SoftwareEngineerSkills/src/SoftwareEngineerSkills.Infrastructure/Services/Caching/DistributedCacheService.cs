using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace SoftwareEngineerSkills.Infrastructure.Services.Caching;

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
