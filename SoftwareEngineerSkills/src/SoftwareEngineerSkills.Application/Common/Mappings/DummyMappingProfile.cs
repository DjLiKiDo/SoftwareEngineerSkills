using AutoMapper;
using SoftwareEngineerSkills.Application.Features.Dummy.Queries.GetAllDummies;

namespace SoftwareEngineerSkills.Application.Common.Mappings;

/// <summary>
/// AutoMapper profile for Dummy entity mappings
/// </summary>
public class DummyMappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DummyMappingProfile"/> class.
    /// </summary>
    public DummyMappingProfile()
    {
        // Map from Domain entity to DTO
        CreateMap<Domain.Entities.Dummy, DummyDto>();
    }
}