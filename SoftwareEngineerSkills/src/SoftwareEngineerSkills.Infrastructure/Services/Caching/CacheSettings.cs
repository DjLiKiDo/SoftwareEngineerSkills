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
