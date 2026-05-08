using AutoMapper;
using Teachio.DAL.Entities.Courses.Courses;

namespace Teachio.BLL.Utils.MappingResolvers;

public class VisibleSectionCountResolver : IValueResolver<Course, object, int>
{
    public int Resolve(Course source, object destination, int destMember, ResolutionContext context)
    {
        return source.Sections?.Count(section => section.VideosCount > 0) ?? 0;
    }
}
