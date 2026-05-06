using AutoMapper;
using Teachio.BLL.DTOs.Courses.Sections.Request.Create;
using Teachio.BLL.DTOs.Courses.Sections.Request.Update;
using Teachio.BLL.DTOs.Courses.Sections.Response;
using Teachio.BLL.Utils.MappingResolvers;
using SectionEntity = Teachio.DAL.Entities.Courses.Sections.Section;

namespace Teachio.BLL.Mappings.Course.Section;

public class SectionProfile : Profile
{
    public SectionProfile()
    {
        CreateMap<SectionCreateRequestDto, SectionEntity>()
            .ForMember(
                dest => dest.Title,
                opt => opt.MapFrom<TrimmedTitleResolver>())
            .ForMember(
                dest => dest.SectionName,
                opt => opt.MapFrom<NameFromTitleResolver>());

        CreateMap<SectionUpdateRequestDto, SectionEntity>()
            .ForMember(
                dest => dest.Title,
                opt => opt.MapFrom<TrimmedTitleResolver>())
            .ForMember(
                dest => dest.SectionName,
                opt => opt.MapFrom<NameFromTitleResolver>());

        CreateMap<SectionEntity, SectionResponseDto>();

        CreateMap<SectionEntity, SectionPreviewResponseDto>();

        CreateMap<SectionEntity, SectionShortResponseDto>();
    }
}
