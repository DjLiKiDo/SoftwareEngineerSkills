using MediatR;
using SoftwareEngineerSkills.Common;
using SoftwareEngineerSkills.Application.Common.Commands;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Commands.DeactivateDummy;

/// <summary>
/// Command to deactivate a dummy entity
/// </summary>
public record DeactivateDummyCommand : ICommand<Result<Unit>>
{
    /// <summary>
    /// Gets or sets the ID of the dummy entity to deactivate
    /// </summary>
    public Guid Id { get; init; }
}
