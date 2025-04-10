using MediatR;
using SoftwareEngineerSkills.Domain.Common.Models;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Commands.DeactivateDummy;

/// <summary>
/// Command to deactivate a dummy entity
/// </summary>
public record DeactivateDummyCommand(Guid Id) : IRequest<Result<bool>>;