using MediatR;
using SoftwareEngineerSkills.Domain.Common.Models;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Commands.ActivateDummy;

/// <summary>
/// Command to activate a dummy entity
/// </summary>
public record ActivateDummyCommand(Guid Id) : IRequest<Result<bool>>;