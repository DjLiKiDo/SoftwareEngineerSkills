using AutoMapper;
using SoftwareEngineerSkills.Application.Features.Dummy.DTOs;
using SoftwareEngineerSkills.Application.Features.Dummy.Commands.CreateDummy;
using SoftwareEngineerSkills.Application.Features.Dummy.Commands.UpdateDummy;

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
        // Domain entity -> DTO
        CreateMap<Domain.Entities.Dummy, DummyDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.IsActive ? "Active" : "Inactive"));
            
        // Command -> Domain entity (for creation)
        CreateMap<CreateDummyCommand, Domain.Entities.Dummy>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true));
            
        // Command -> Domain entity (for update)
        CreateMap<UpdateDummyCommand, Domain.Entities.Dummy>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore());
    }
}
