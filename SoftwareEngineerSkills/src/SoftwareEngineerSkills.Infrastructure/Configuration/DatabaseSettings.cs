using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SoftwareEngineerSkills.Infrastructure.Configuration;

/// <summary>
/// Database settings
/// </summary>
public class DatabaseSettings
{
    /// <summary>
    /// Gets or sets the database provider
    /// </summary>
    public string Provider { get; set; } = "inmemory";

    /// <summary>
    /// Gets or sets the connection string
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether to enable detailed errors
    /// </summary>
    public bool EnableDetailedErrors { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to enable sensitive data logging
    /// </summary>
    public bool EnableSensitiveDataLogging { get; set; } = false;

    /// <summary>
    /// Gets or sets the maximum retry count
    /// </summary>
    public int MaxRetryCount { get; set; } = 5;

    /// <summary>
    /// Gets or sets the maximum retry delay in seconds
    /// </summary>
    public int MaxRetryDelaySeconds { get; set; } = 30;
}

/// <summary>
/// Extension methods for configuring database settings
/// </summary>
public static class DatabaseSettingsExtensions
{
    /// <summary>
    /// Adds database settings to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddDatabaseSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseSettings>(configuration.GetSection("Database"));
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<DatabaseSettings>>().Value);
        
        return services;
    }
}
