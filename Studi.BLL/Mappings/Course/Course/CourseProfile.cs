using AutoMapper;
using Studi.BLL.DTOs.Courses.Courses.Request.Create;
using Studi.BLL.DTOs.Courses.Courses.Request.Update;
using Studi.BLL.DTOs.Courses.Courses.Response;
using Studi.BLL.Utils.MappingResolvers;
using CourseEntity = Studi.DAL.Entities.Courses.Courses.Course;
using SectionEntity = Studi.DAL.Entities.Courses.Sections.Section;

namespace Studi.BLL.Mappings.Course.Course;

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
                dest => dest.OwnerFullName,
                opt => opt.MapFrom<OwnerFullNameResolver>())
            .ForMember(
                dest => dest.Sections,
                opt => opt.MapFrom(src => GetOrderedVisibleSectionsAndVideos(src)))
            .ForMember(
                dest => dest.SectionsCount,
                opt => opt.MapFrom<VisibleSectionsCountResolver>())
            .ForMember(
                dest => dest.TotalDurationHours,
                opt => opt.MapFrom<TotalDurationHoursResolver>());

        CreateMap<CourseEntity, CoursePreviewResponseDto>()
            .ForMember(
                dest => dest.OwnerFullName,
                opt => opt.MapFrom<OwnerFullNameResolver>())
            .ForMember(
                dest => dest.Sections,
                opt => opt.MapFrom(src => GetOrderedVisibleSectionsAndVideos(src)))
            .ForMember(
                dest => dest.SectionsCount,
                opt => opt.MapFrom<VisibleSectionsCountResolver>())
            .ForMember(
                dest => dest.TotalDurationHours,
                opt => opt.MapFrom<TotalDurationHoursResolver>());

        CreateMap<CourseEntity, CoursePreviewShortResponseDto>()
            .ForMember(
                dest => dest.OwnerFullName,
                opt => opt.MapFrom<OwnerFullNameResolver>())
            .ForMember(
                dest => dest.TotalDurationHours,
                opt => opt.MapFrom<TotalDurationHoursResolver>())
            .ForMember(
                dest => dest.VideosCount,
                opt => opt.MapFrom<VideosCountResolver>());

        CreateMap<CourseEntity, CourseEditShortResponseDto>();
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
