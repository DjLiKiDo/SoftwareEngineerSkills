using Microsoft.AspNetCore.Mvc;
using SoftwareEngineerSkills.Application.Common.Services;

namespace SoftwareEngineerSkills.API.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController(IAppSettingsService appSettingsService, ILogger<WeatherForecastController> logger) : ControllerBase
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    private readonly ILogger<WeatherForecastController> _logger = logger;
    private readonly IAppSettingsService _appSettingsService = appSettingsService;

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        var environment = _appSettingsService.CurrentSettings.Environment;
        _logger.LogInformation("Requesting weather forecast data in environment: {Environment}", environment);
        
        var forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        }).ToArray();
        
        _logger.LogInformation("Successfully retrieved {Count} weather forecasts in environment: {Environment}", 
            forecasts.Length, environment);
        
        return forecasts;
    }

    /// <summary>
    /// Retrieves the dummy settings from the configuration
    /// </summary>
    /// <returns>Current dummy settings based on the environment</returns>
    [HttpGet("dummy-settings", Name = "GetDummySettings")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetDummySettings()
    {
        var appSettings = _appSettingsService.CurrentSettings;
        var environment = appSettings.Environment;
        
        _logger.LogInformation("Retrieving dummy settings in environment: {Environment}", environment);
        
        return Ok(new
        {
            Environment = environment.ToString(),
            ApplicationName = appSettings.ApplicationName,
            DummySettings = appSettings.DummySettings
        });
    }
}
