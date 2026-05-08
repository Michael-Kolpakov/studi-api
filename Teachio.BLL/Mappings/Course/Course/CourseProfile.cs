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
                opt => opt.MapFrom(src => GetVisibleSections(src)))
            .ForMember(
                dest => dest.SectionsCount,
                opt => opt.MapFrom<VisibleSectionCountResolver>())
            .ForMember(
                dest => dest.TotalDuration,
                opt => opt.MapFrom<TotalDurationResolver>());

        CreateMap<CourseEntity, CoursePreviewResponseDto>()
            .ForMember(
                dest => dest.Sections,
                opt => opt.MapFrom(src => GetVisibleSections(src)))
            .ForMember(
                dest => dest.SectionsCount,
                opt => opt.MapFrom<VisibleSectionCountResolver>())
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

    private static IEnumerable<SectionEntity> GetVisibleSections(CourseEntity course) =>
        course.Sections.Where(x => x.VideosCount > 0);
}
