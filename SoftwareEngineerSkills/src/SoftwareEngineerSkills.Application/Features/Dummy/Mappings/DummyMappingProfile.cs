using AutoMapper;
using SoftwareEngineerSkills.Application.Configuration;
using SoftwareEngineerSkills.Application.Features.Dummy.Queries.GetDummyConfiguration;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Mappings;

/// <summary>
/// AutoMapper profile for Dummy feature-related mappings
/// </summary>
public class DummyMappingProfile : Profile
{
    public DummyMappingProfile()
    {
        CreateMap<DummySettings, DummySettingsDto>()
            .ForMember(dest => dest.Setting1, opt => opt.MapFrom(src => src.DummySetting1))
            .ForMember(dest => dest.Setting2, opt => opt.MapFrom(src => src.DummySetting2))
            .ForMember(dest => dest.Setting3, opt => opt.MapFrom(src => src.DummySetting3));
    }
}
