namespace SoftwareEngineerSkills.Application.Configuration;

/// <summary>
/// Service interface for accessing application configuration settings
/// </summary>
public interface IAppSettingsService
{
    /// <summary>
    /// Gets the current application settings
    /// </summary>
    AppSettings CurrentSettings { get; }
    
    /// <summary>
    /// Register a callback to be invoked when settings change
    /// </summary>
    /// <param name="listener">The callback to invoke when settings change</param>
    /// <returns>A disposable that can be used to unregister the callback</returns>
    IDisposable OnChange(Action<AppSettings> listener);
}
