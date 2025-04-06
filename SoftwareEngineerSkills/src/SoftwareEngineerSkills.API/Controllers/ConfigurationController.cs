using Microsoft.AspNetCore.Mvc;
using SoftwareEngineerSkills.Application.Common.Services;

namespace SoftwareEngineerSkills.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConfigurationController : ControllerBase
{
    private readonly IAppSettingsService _appSettingsService;
    private readonly ILogger<ConfigurationController> _logger;

    public ConfigurationController(IAppSettingsService appSettingsService, ILogger<ConfigurationController> logger)
    {
        _appSettingsService = appSettingsService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetConfiguration()
    {
        _logger.LogInformation("Retrieving current application configuration");
        
        var config = new
        {
            ApplicationName = _appSettingsService.CurrentSettings.ApplicationName,
            Environment = _appSettingsService.CurrentSettings.Environment.ToString(),
            AspNetCoreEnvironment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"
        };
        
        return Ok(config);
    }
}
