using AutoMapper;
using Studi.BLL.DTOs.Courses.Courses.Request;
using Studi.BLL.DTOs.Courses.Sections.Request;
using Studi.DAL.Entities.Courses.Courses;
using Studi.DAL.Entities.Courses.Sections;

namespace Studi.BLL.Utils.MappingResolvers;

public class NameFromTitleResolver : IValueResolver<object, object, string>
{
    public string Resolve(object source, object destination, string destMember, ResolutionContext context)
    {
        return (source, destination) switch
        {
            (CourseCreateUpdateRequestDto courseSource, Course) =>
                CreateNameFromTitle(courseSource.Title),
            (SectionCreateUpdateRequestDto sectionSource, Section) =>
                CreateNameFromTitle(sectionSource.Title),
            _ => throw new ArgumentException(
                $"Unknown source '{source.GetType().Name}' and destination '{destination.GetType().Name}' types combination.")
        };
    }

    public static string CreateNameFromTitle(string title) =>
        string.Join("-", title.Trim().ToLowerInvariant().Split(" ", StringSplitOptions.RemoveEmptyEntries));
}
