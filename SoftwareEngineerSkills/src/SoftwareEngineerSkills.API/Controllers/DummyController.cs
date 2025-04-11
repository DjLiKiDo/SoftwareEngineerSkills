using Microsoft.AspNetCore.Mvc;
using SoftwareEngineerSkills.Application.Features.Dummy.Commands.ActivateDummy;
using SoftwareEngineerSkills.Application.Features.Dummy.Commands.CreateDummy;
using SoftwareEngineerSkills.Application.Features.Dummy.Commands.DeactivateDummy;
using SoftwareEngineerSkills.Application.Features.Dummy.Commands.DeleteDummy;
using SoftwareEngineerSkills.Application.Features.Dummy.Commands.UpdateDummy;
using SoftwareEngineerSkills.Application.Features.Dummy.DTOs;
using SoftwareEngineerSkills.Application.Features.Dummy.Queries.GetAllDummies;
using SoftwareEngineerSkills.Application.Features.Dummy.Queries.GetDummyById;

namespace SoftwareEngineerSkills.API.Controllers;

/// <summary>
/// Controller for managing Dummy entities
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
    /// Gets all dummy entities
    /// </summary>
    /// <param name="includeInactive">Whether to include inactive entities</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A list of dummy entities</returns>
    [HttpGet(Name = "GetAllDummies")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<DummyDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> GetAllAsync([FromQuery] bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving all dummy entities (includeInactive: {IncludeInactive})", includeInactive);

        var result = await Mediator.Send(new GetAllDummiesQuery(includeInactive), cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Gets a specific dummy entity by its ID
    /// </summary>
    /// <param name="id">The ID of the dummy entity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The dummy entity if found</returns>
    [HttpGet("{id}", Name = "GetDummyById")] // Ensure route matches the CreatedAtAction call
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DummyDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving dummy entity with ID: {Id}", id);

        var result = await Mediator.Send(new GetDummyByIdQuery(id), cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Creates a new dummy entity
    /// </summary>
    /// <param name="command">The create command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The ID of the newly created dummy entity</returns>
    [HttpPost(Name = "CreateDummy")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> CreateAsync([FromBody] CreateDummyCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating a new dummy entity");

        var result = await Mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetByIdAsync), new { id = result.Value }, result.Value);

        return HandleResult(result);
    }

    /// <summary>
    /// Updates an existing dummy entity
    /// </summary>
    /// <param name="id">The ID of the dummy entity to update</param>
    /// <param name="command">The update command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A success response if the update was successful</returns>
    [HttpPut("{id}", Name = "UpdateDummy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateDummyCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Bad Request",
                Detail = "The ID in the URL does not match the ID in the request body",
                Status = StatusCodes.Status400BadRequest
            });
        }

        _logger.LogInformation("Updating dummy entity with ID: {Id}", id);

        var result = await Mediator.Send(command, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Deletes a dummy entity
    /// </summary>
    /// <param name="id">The ID of the dummy entity to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A success response if the deletion was successful</returns>
    [HttpDelete("{id}", Name = "DeleteDummy")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting dummy entity with ID: {Id}", id);

        var result = await Mediator.Send(new DeleteDummyCommand(id), cancellationToken);

        if (result.IsSuccess && result.Value)
            return NoContent();

        return HandleResult(result);
    }

    /// <summary>
    /// Activates a dummy entity
    /// </summary>
    /// <param name="id">The ID of the dummy entity to activate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A success response if the activation was successful</returns>
    [HttpPatch("{id}/activate", Name = "ActivateDummy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateAsync(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Activating dummy entity with ID: {Id}", id);

        var result = await Mediator.Send(new ActivateDummyCommand(id), cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Deactivates a dummy entity
    /// </summary>
    /// <param name="id">The ID of the dummy entity to deactivate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A success response if the deactivation was successful</returns>
    [HttpPatch("{id}/deactivate", Name = "DeactivateDummy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateAsync(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deactivating dummy entity with ID: {Id}", id);

        var result = await Mediator.Send(new DeactivateDummyCommand(id), cancellationToken);

        return HandleResult(result);
    }
}
