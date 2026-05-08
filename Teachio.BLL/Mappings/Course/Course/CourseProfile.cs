using AutoMapper;
using Teachio.BLL.DTOs.Courses.Courses.Request.Create;
using Teachio.BLL.DTOs.Courses.Courses.Request.Update;
using Teachio.BLL.DTOs.Courses.Courses.Response;
using Teachio.BLL.Utils.MappingResolvers;
using CourseEntity = Teachio.DAL.Entities.Courses.Courses.Course;
using SectionEntity = Teachio.DAL.Entities.Courses.Sections.Section;

namespace Teachio.BLL.Mappings.Course.Course;

public class CourseProfile : Profile
{
    public CourseProfile()
    {
        CreateMap<CourseCreateRequestDto, CourseEntity>()
            .ForMember(
                dest => dest.Title,
                opt => opt.MapFrom<TrimmedTitleResolver>())
            .ForMember(
                dest => dest.Description,
                opt => opt.MapFrom<TrimmedDescriptionResolver>())
            .ForMember(
                dest => dest.CourseName,
                opt => opt.MapFrom<NameFromTitleResolver>())
            .ForMember(
                dest => dest.OwnerUserId,
                opt => opt.MapFrom<OwnerUserIdResolver>());

        CreateMap<CourseUpdateRequestDto, CourseEntity>()
            .ForMember(
                dest => dest.Title,
                opt => opt.MapFrom<TrimmedTitleResolver>())
            .ForMember(
                dest => dest.Description,
                opt => opt.MapFrom<TrimmedDescriptionResolver>())
            .ForMember(
                dest => dest.CourseName,
                opt => opt.MapFrom<NameFromTitleResolver>());

        CreateMap<CourseEntity, CourseResponseDto>()
            .ForMember(
                dest => dest.Sections,
                opt => opt.MapFrom(src => GetOrderedVisibleSectionsAndVideos(src)))
            .ForMember(
                dest => dest.SectionsCount,
                opt => opt.MapFrom<VisibleSectionsCountResolver>())
            .ForMember(
                dest => dest.TotalDuration,
                opt => opt.MapFrom<TotalDurationResolver>());

        CreateMap<CourseEntity, CoursePreviewResponseDto>()
            .ForMember(
                dest => dest.Sections,
                opt => opt.MapFrom(src => GetOrderedVisibleSectionsAndVideos(src)))
            .ForMember(
                dest => dest.SectionsCount,
                opt => opt.MapFrom<VisibleSectionsCountResolver>())
            .ForMember(
                dest => dest.TotalDuration,
                opt => opt.MapFrom<TotalDurationResolver>())
            .ForMember(
                dest => dest.ThumbnailRelativePath,
                opt => opt.MapFrom<RelativePathResolver>());

        CreateMap<CourseEntity, CoursePreviewShortResponseDto>()
            .ForMember(
                dest => dest.TotalDuration,
                opt => opt.MapFrom<TotalDurationResolver>())
            .ForMember(
                dest => dest.VideosCount,
                opt => opt.MapFrom<VideosCountResolver>())
            .ForMember(
                dest => dest.ThumbnailRelativePath,
                opt => opt.MapFrom<RelativePathResolver>());
    }

    private static List<SectionEntity> GetOrderedVisibleSectionsAndVideos(CourseEntity course)
    {
        var sections = course.Sections
            .Where(s => s.VideosCount > 0)
            .OrderBy(s => s.OrderIndex)
            .ToList();

        foreach (var section in sections)
        {
            if (section.Videos is not null)
            {
                section.Videos = section.Videos.OrderBy(v => v.OrderIndex).ToList();
            }
        }

        return sections;
    }
}
