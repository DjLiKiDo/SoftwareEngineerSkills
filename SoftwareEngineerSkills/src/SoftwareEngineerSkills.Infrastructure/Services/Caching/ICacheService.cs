namespace SoftwareEngineerSkills.Infrastructure.Services.Caching;

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
