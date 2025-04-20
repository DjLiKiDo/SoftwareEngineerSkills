using MediatR;
using SoftwareEngineerSkills.Common;
using SoftwareEngineerSkills.Application.Common.Commands;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Commands.UpdateDummy;

/// <summary>
/// Command to update an existing dummy entity
/// </summary>
public record UpdateDummyCommand : ICommand<Result<Unit>>
{
    /// <summary>
    /// Gets or sets the ID of the dummy entity to update
    /// </summary>
    public Guid Id { get; init; }
    
    /// <summary>
    /// Gets or sets the updated name of the dummy entity
    /// </summary>
    public string? Name { get; init; }
    
    /// <summary>
    /// Gets or sets the updated description of the dummy entity
    /// </summary>
    public string? Description { get; init; }
    
    /// <summary>
    /// Gets or sets the updated priority level of the dummy entity (0-5)
    /// </summary>
    public int Priority { get; init; }
}
