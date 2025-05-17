namespace SoftwareEngineerSkills.Application.Configuration;

/// <summary>
/// Service interface for accessing configuration settings
/// </summary>
public interface IOptionsService<T> where T : IOptionsSection
{
    /// <summary>
    /// Gets the current settings
    /// </summary>
    ApplicationOptions CurrentSettings { get; }

    /// <summary>
    /// Register a callback to be invoked when settings change
    /// </summary>
    /// <param name="listener">The callback to invoke when settings change</param>
    /// <returns>A disposable that can be used to unregister the callback</returns>
    IDisposable OnChange(Action<T> listener);
}
