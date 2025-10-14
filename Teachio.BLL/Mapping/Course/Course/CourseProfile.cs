using AutoMapper;
using Teachio.BLL.Dto.Courses.Courses;
using Teachio.BLL.Dto.Courses.Courses.Create;
using Teachio.BLL.Dto.Courses.Courses.Update;
using CourseEntity = Teachio.DAL.Entities.Courses.Courses.Course;

namespace Teachio.BLL.Mapping.Course.Course;

public class CourseProfile : Profile
{
    public CourseProfile()
    {
        CreateMap<CourseCreateDto, CourseEntity>();
        CreateMap<CourseUpdateDto, CourseEntity>();
        CreateMap<CourseEntity, CourseResponseDto>();
    }
}
