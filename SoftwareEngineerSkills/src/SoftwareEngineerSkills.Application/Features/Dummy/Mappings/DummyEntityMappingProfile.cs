using AutoMapper;
using SoftwareEngineerSkills.Application.Features.Dummy.DTOs;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Mappings;

/// <summary>
/// AutoMapper profile for Dummy entity mappings
/// </summary>
public class DummyEntityMappingProfile : Profile
{
    public DummyEntityMappingProfile()
    {
        // Entity -> DTO
        CreateMap<SoftwareEngineerSkills.Domain.Entities.Dummy, DummyDto>();
    }
}
