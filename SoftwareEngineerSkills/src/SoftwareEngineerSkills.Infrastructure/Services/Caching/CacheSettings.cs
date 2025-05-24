using SoftwareEngineerSkills.Infrastructure.Configuration;
using System.ComponentModel.DataAnnotations;

namespace SoftwareEngineerSkills.Infrastructure.Services.Caching;

/// <summary>
/// Configuration for cache service
/// </summary>
public class CacheSettings : IValidatableSettings
{
    /// <summary>
    /// The configuration section name
    /// </summary>
    public const string SectionName = "Cache";
    
    /// <summary>
    /// The cache provider
    /// </summary>
    [Required(ErrorMessage = "Cache provider is required")]
    public string Provider { get; set; } = "memory";
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
    
    /// <summary>
    /// Redis connection string (for Redis provider)
    /// </summary>
    public string RedisConnectionString { get; set; } = string.Empty;
    
    /// <summary>
    /// Redis instance name (for Redis provider)
    /// </summary>
    public string RedisInstanceName { get; set; } = "SoftwareEngineerSkills:";
    
    /// <summary>
    /// Validates the settings
    /// </summary>
    /// <returns>True if settings are valid, otherwise false</returns>
    public bool Validate(out ICollection<ValidationResult> validationResults)
    {
        validationResults = new List<ValidationResult>();
        
        // Validate provider
        if (string.IsNullOrWhiteSpace(Provider))
        {
            validationResults.Add(new ValidationResult(
                "Cache provider cannot be empty",
                new[] { nameof(Provider) }));
        }
        else if (Provider != "memory" && Provider != "distributed" && Provider != "redis")
        {
            validationResults.Add(new ValidationResult(
                "Cache provider must be 'memory', 'distributed', or 'redis'",
                new[] { nameof(Provider) }));
        }
        
        // Validate expiration minutes
        if (DefaultExpirationMinutes <= 0)
        {
            validationResults.Add(new ValidationResult(
                "Default expiration minutes must be greater than 0",
                new[] { nameof(DefaultExpirationMinutes) }));
        }
        
        // Validate size limit
        if (Provider == "memory" && SizeLimitInMB <= 0 && SizeLimitInMB != -1)
        {
            validationResults.Add(new ValidationResult(
                "Size limit must be greater than 0 or -1 for unlimited",
                new[] { nameof(SizeLimitInMB) }));
        }
        
        // Validate Redis configuration
        if (Provider == "redis" && string.IsNullOrEmpty(RedisConnectionString))
        {
            validationResults.Add(new ValidationResult(
                "Redis connection string is required when using the Redis provider",
                new[] { nameof(RedisConnectionString) }));
        }
        
        return validationResults.Count == 0;
    }
}
