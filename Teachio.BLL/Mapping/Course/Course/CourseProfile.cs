using AutoMapper;
using Teachio.BLL.Dto.Courses.Courses.Request.Create;
using Teachio.BLL.Dto.Courses.Courses.Request.Update;
using Teachio.BLL.Dto.Courses.Courses.Response;
using CourseEntity = Teachio.DAL.Entities.Courses.Courses.Course;

namespace Teachio.BLL.Mapping.Course.Course;

public class CourseProfile : Profile
{
    public CourseProfile()
    {
        CreateMap<CourseCreateRequestDto, CourseEntity>();
        CreateMap<CourseUpdateRequestDto, CourseEntity>();

        CreateMap<CourseEntity, CourseResponseDto>()
            .ForMember(
                dest => dest.WatchingUsersCount,
                opt => opt.MapFrom(src => src.WatchingUsers.Count));

        CreateMap<CourseEntity, CoursePreviewResponseDto>()
            .ForMember(
                dest => dest.WatchingUsersCount,
                opt => opt.MapFrom(src => src.WatchingUsers.Count));

        CreateMap<CourseEntity, CoursePreviewShortResponseDto>()
            .ForMember(
                dest => dest.WatchingUsersCount,
                opt => opt.MapFrom(src=> src.WatchingUsers.Count));
    }
}
