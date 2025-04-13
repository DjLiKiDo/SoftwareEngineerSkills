using MediatR;
using SoftwareEngineerSkills.Domain.Common.Models;
using SoftwareEngineerSkills.Application.Common.Commands;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Commands.ActivateDummy;

/// <summary>
/// Command to activate a dummy entity
/// </summary>
public record ActivateDummyCommand : ICommand<Result<Unit>>
{
    /// <summary>
    /// Gets or sets the ID of the dummy entity to activate
    /// </summary>
    public Guid Id { get; init; }
}
