using AutoMapper;
using Studi.BLL.DTOs.Courses.Sections.Request.Create;
using Studi.BLL.DTOs.Courses.Sections.Request.Update;
using Studi.BLL.DTOs.Courses.Sections.Response;
using Studi.BLL.Utils.MappingResolvers;
using SectionEntity = Studi.DAL.Entities.Courses.Sections.Section;
using VideoEntity = Studi.DAL.Entities.Courses.Videos.Videos.Video;

namespace Studi.BLL.Mappings.Course.Section;

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

        CreateMap<SectionEntity, SectionEditShortResponseDto>();
    }

    private static IEnumerable<VideoEntity> GetOrderedVideos(SectionEntity section) =>
        section.Videos.OrderBy(v => v.OrderIndex);
}
