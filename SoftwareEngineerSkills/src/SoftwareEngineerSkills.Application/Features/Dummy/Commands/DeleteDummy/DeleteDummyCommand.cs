using MediatR;
using SoftwareEngineerSkills.Domain.Common.Models;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Commands.DeleteDummy;

/// <summary>
/// Command to delete a dummy entity
/// </summary>
public record DeleteDummyCommand(Guid Id) : IRequest<Result<bool>>;
