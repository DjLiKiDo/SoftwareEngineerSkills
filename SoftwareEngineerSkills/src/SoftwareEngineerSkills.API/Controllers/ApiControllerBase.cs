using MediatR;
using Microsoft.AspNetCore.Mvc;
using SoftwareEngineerSkills.Common;

namespace SoftwareEngineerSkills.API.Controllers;

/// <summary>
/// Base controller for all API controllers providing common functionality
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender? _mediator;

    /// <summary>
    /// Gets the mediator instance from the request services
    /// </summary>
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    /// <summary>
    /// Handles the result of an operation and returns the appropriate HTTP response
    /// </summary>
    /// <typeparam name="T">Type of the result value</typeparam>
    /// <param name="result">The result to handle</param>
    /// <returns>An appropriate HTTP response</returns>
    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess && !object.Equals(result.Value, default(T)))
            return Ok(result.Value);

        if (result.IsSuccess && object.Equals(result.Value, default(T)))
            return NotFound();

        return BadRequest(new ProblemDetails
        {
            Title = "Application Error",
            Detail = result.Error,
            Status = StatusCodes.Status400BadRequest
        });
    }
}
