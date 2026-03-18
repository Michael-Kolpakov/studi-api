using AutoMapper;
using Teachio.BLL.Dto.Courses.Sections.Request.Create;
using Teachio.BLL.Dto.Courses.Sections.Request.Update;
using Teachio.BLL.Dto.Courses.Sections.Response;
using Teachio.BLL.Utils.MappingResolvers;
using SectionEntity = Teachio.DAL.Entities.Courses.Sections.Section;

namespace Teachio.BLL.Mapping.Course.Section;

/// <summary>
/// Represents the <see cref="SectionProfile"/> type.
/// </summary>
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
