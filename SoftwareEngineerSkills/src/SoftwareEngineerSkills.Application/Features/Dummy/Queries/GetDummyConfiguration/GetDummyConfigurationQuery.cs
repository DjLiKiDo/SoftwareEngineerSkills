using MediatR;
using SoftwareEngineerSkills.Domain.Common.Models;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Queries.GetDummyConfiguration;

/// <summary>
/// Query to retrieve dummy configuration settings
/// </summary>
public record GetDummyConfigurationQuery : IRequest<Result<GetDummyConfigurationResponse>>;
