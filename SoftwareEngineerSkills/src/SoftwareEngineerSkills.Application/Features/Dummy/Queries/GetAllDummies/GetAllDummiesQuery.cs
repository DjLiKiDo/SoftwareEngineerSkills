using MediatR;
using SoftwareEngineerSkills.Domain.Common.Models;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Queries.GetAllDummies;

/// <summary>
/// Query to retrieve all dummy entities
/// </summary>
public record GetAllDummiesQuery(bool IncludeInactive = false) : IRequest<Result<List<DummyDto>>>;