using AutoMapper;
using Teachio.BLL.DTOs.Courses.Sections.Request.Create;
using Teachio.BLL.DTOs.Courses.Sections.Request.Update;
using Teachio.BLL.DTOs.Courses.Sections.Response;
using Teachio.BLL.Utils.MappingResolvers;
using SectionEntity = Teachio.DAL.Entities.Courses.Sections.Section;
using VideoEntity = Teachio.DAL.Entities.Courses.Videos.Videos.Video;

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

        CreateMap<SectionEntity, SectionResponseDto>()
            .ForMember(
                dest => dest.Videos,
                opt => opt.MapFrom(src => GetOrderedVideos(src)))
            .ForMember(
                dest => dest.TotalDurationSeconds,
                opt => opt.MapFrom<TotalDurationSecondsResolver>()
            );

        CreateMap<SectionEntity, SectionPreviewResponseDto>()
            .ForMember(
                dest => dest.Videos,
                opt => opt.MapFrom(src => GetOrderedVideos(src)))
            .ForMember(
                dest => dest.TotalDurationSeconds,
                opt => opt.MapFrom<TotalDurationSecondsResolver>()
            );

        CreateMap<SectionEntity, SectionShortResponseDto>()
            .ForMember(
                dest => dest.TotalDurationSeconds,
                opt => opt.MapFrom<TotalDurationSecondsResolver>()
            );
    }

    private static IEnumerable<VideoEntity> GetOrderedVideos(SectionEntity section) =>
        section.Videos.OrderBy(v => v.OrderIndex);
}
