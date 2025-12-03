using AutoMapper;
using Teachio.BLL.Dto.Courses.Courses.Request.Create;
using Teachio.BLL.Dto.Courses.Courses.Request.Update;
using Teachio.BLL.Dto.Courses.Courses.Response;
using Teachio.BLL.Utils.MappingResolvers;
using CourseEntity = Teachio.DAL.Entities.Courses.Courses.Course;

namespace Teachio.BLL.Mapping.Course.Course;

public class CourseProfile : Profile
{
    public CourseProfile()
    {
        CreateMap<CourseCreateRequestDto, CourseEntity>()
            .ForMember(
                dest => dest.Title,
                opt => opt.MapFrom<TrimTitleResolver>())
            .ForMember(
                dest => dest.CourseName,
                opt => opt.MapFrom<CreateNameFromTitleResolver>());

        CreateMap<CourseUpdateRequestDto, CourseEntity>()
            .ForMember(
                dest => dest.Title,
                opt => opt.MapFrom<TrimTitleResolver>())
            .ForMember(
                dest => dest.CourseName,
                opt => opt.MapFrom<CreateNameFromTitleResolver>());

        CreateMap<CourseEntity, CourseResponseDto>()
            .ForMember(
                dest => dest.WatchingUsersCount,
                opt => opt.MapFrom(src => src.WatchingUsers.Count));

        CreateMap<CourseEntity, CoursePreviewResponseDto>()
            .ForMember(
                dest => dest.WatchingUsersCount,
                opt => opt.MapFrom(src => src.WatchingUsers.Count))
            .ForMember(
                dest => dest.TotalDuration,
                opt => opt.MapFrom<TotalDurationResolver>())
            .ForMember(
                dest => dest.ThumbnailRelativePath,
                opt => opt.MapFrom<CreateThumbnailRelativePathResolver>());

        CreateMap<CourseEntity, CoursePreviewShortResponseDto>()
            .ForMember(
                dest => dest.WatchingUsersCount,
                opt => opt.MapFrom(src=> src.WatchingUsers.Count));
    }
}
