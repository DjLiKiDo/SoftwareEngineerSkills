using SoftwareEngineerSkills.Common;
using SoftwareEngineerSkills.Application.Common.Commands;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Commands.CreateDummy;

/// <summary>
/// Command to create a new dummy entity
/// </summary>
public record CreateDummyCommand : ICommand<Result<Guid>>
{
    /// <summary>
    /// Gets or sets the name of the dummy entity
    /// </summary>
    public string? Name { get; init; }
    
    /// <summary>
    /// Gets or sets the description of the dummy entity
    /// </summary>
    public string? Description { get; init; }
    
    /// <summary>
    /// Gets or sets the priority level of the dummy entity (0-5)
    /// </summary>
    public int Priority { get; init; } = 0;
}
