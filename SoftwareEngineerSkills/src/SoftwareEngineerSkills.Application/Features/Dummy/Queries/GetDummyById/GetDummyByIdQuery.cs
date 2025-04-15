using MediatR;
using SoftwareEngineerSkills.Application.Features.Dummy.DTOs;
using SoftwareEngineerSkills.Common;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Queries.GetDummyById;

/// <summary>
/// Query to retrieve a specific dummy entity by ID
/// </summary>
public record GetDummyByIdQuery(Guid Id) : IRequest<Result<DummyDto>>;
