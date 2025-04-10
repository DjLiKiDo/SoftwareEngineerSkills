using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SoftwareEngineerSkills.Application.Features.Dummy.Queries.GetDummyConfiguration;

namespace SoftwareEngineerSkills.API.Controllers;

/// <summary>
/// Example controller for the Dummy feature - Use this as a template for new features
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DummyController : ApiControllerBase
{
    private readonly ILogger<DummyController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DummyController"/> class.
    /// </summary>
    /// <param name="logger">The logger</param>
    public DummyController(ILogger<DummyController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets the application configuration information using CQRS pattern
    /// </summary>
    /// <returns>Application configuration details including environment and dummy settings</returns>
    /// <remarks>
    /// This endpoint demonstrates how to retrieve and return configuration information
    /// using the CQRS pattern with MediatR.
    /// </remarks>
    [HttpGet("config", Name = "GetDummyConfiguration")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetDummyConfigurationResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetConfigurationAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving dummy configuration through CQRS");
        
        var result = await Mediator.Send(new GetDummyConfigurationQuery(), cancellationToken);
        
        return HandleResult(result);
    }
}
