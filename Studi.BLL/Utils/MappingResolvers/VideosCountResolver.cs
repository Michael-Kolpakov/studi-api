using AutoMapper;
using Studi.BLL.DTOs.Courses.Courses.Response;
using Studi.DAL.Entities.Courses.Courses;

namespace Studi.BLL.Utils.MappingResolvers;

public class VideosCountResolver : IValueResolver<Course, CoursePreviewShortResponseDto, int>
{
    public int Resolve(Course source, CoursePreviewShortResponseDto destination, int destMember, ResolutionContext context)
    {
        return source.Sections?.Sum(section => section.VideosCount) ?? 0;
    }
}
