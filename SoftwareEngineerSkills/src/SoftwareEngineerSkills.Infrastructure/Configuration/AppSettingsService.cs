using Microsoft.Extensions.Options;
using SoftwareEngineerSkills.Application.Configuration;

namespace SoftwareEngineerSkills.Infrastructure.Configuration;

/// <summary>
/// Implementation of the app settings service using IOptionsMonitor
/// </summary>
public class AppSettingsService : IAppSettingsService
{
    private readonly IOptionsMonitor<AppSettings> _optionsMonitor;

    public AppSettingsService(IOptionsMonitor<AppSettings> optionsMonitor)
    {
        _optionsMonitor = optionsMonitor;
    }

    /// <inheritdoc />
    public AppSettings CurrentSettings => _optionsMonitor.CurrentValue;

    /// <inheritdoc />
    public IDisposable OnChange(Action<AppSettings> listener)
    {
        return _optionsMonitor.OnChange(listener);
    }
}
