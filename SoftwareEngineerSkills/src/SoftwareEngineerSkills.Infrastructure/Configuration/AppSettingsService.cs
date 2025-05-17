using Microsoft.Extensions.Options;
using SoftwareEngineerSkills.Application.Configuration;

namespace SoftwareEngineerSkills.Infrastructure.Configuration;

/// <summary>
/// Implementation of the app settings service using IOptionsMonitor
/// </summary>
public class AppSettingsService : IAppSettingsService
{
    private readonly IOptionsMonitor<ApplicationOptions> _optionsMonitor;

    public AppSettingsService(IOptionsMonitor<ApplicationOptions> optionsMonitor)
    {
        _optionsMonitor = optionsMonitor;
    }

    /// <inheritdoc />
    public ApplicationOptions CurrentSettings => _optionsMonitor.CurrentValue;

    /// <inheritdoc />
    public IDisposable OnChange(Action<ApplicationOptions> listener)
    {
        return _optionsMonitor.OnChange(listener)!;
    }
}
