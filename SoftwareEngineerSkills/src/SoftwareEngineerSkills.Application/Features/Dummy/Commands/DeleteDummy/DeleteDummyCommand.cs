using MediatR;
using SoftwareEngineerSkills.Domain.Common.Models;
using SoftwareEngineerSkills.Application.Common.Commands;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Commands.DeleteDummy;

/// <summary>
/// Command to delete a dummy entity
/// </summary>
public record DeleteDummyCommand : ICommand<Result<Unit>>
{
    /// <summary>
    /// Gets or sets the ID of the dummy entity to delete
    /// </summary>
    public Guid Id { get; init; }
}
