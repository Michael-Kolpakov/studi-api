using AutoMapper;
using Teachio.BLL.DTOs.Courses.Courses.Response;
using Teachio.DAL.Entities.Courses.Courses;

namespace Teachio.BLL.Utils.MappingResolvers;

public class VideosCountResolver : IValueResolver<Course, CoursePreviewShortResponseDto, int>
{
    public int Resolve(Course source, CoursePreviewShortResponseDto destination, int destMember, ResolutionContext context)
    {
        return source.Sections.Sum(section => section.VideosCount);
    }
}
