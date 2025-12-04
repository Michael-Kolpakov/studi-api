using AutoMapper;
using Teachio.BLL.Dto.Courses.Courses.Request;
using Teachio.BLL.Dto.Courses.Sections.Request;
using Teachio.DAL.Entities.Courses.Courses;

namespace Teachio.BLL.Utils.MappingResolvers;

public class CreateNameFromTitleResolver : IValueResolver<object, Course, string>
{
    public string Resolve(object source, Course destination, string destMember, ResolutionContext context)
    {
        return source switch
        {
            CourseCreateUpdateRequestDto courseSource => CreateNameFromTitle(courseSource.Title),
            SectionCreateUpdateRequestDto sectionSource => CreateNameFromTitle(sectionSource.Title),
            _ => throw new ArgumentException($"Unknown source '{nameof(source)}' type", nameof(source))
        };
    }

    private static string CreateNameFromTitle(string title) =>
        string.Join("-", title.Trim().ToLowerInvariant().Split(" ", StringSplitOptions.RemoveEmptyEntries));
}
