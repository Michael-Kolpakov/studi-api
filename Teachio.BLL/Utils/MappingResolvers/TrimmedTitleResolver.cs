using AutoMapper;
using Teachio.BLL.Dto.Courses.Courses.Request;
using Teachio.BLL.Dto.Courses.Sections.Request;

namespace Teachio.BLL.Utils.MappingResolvers;

public class TrimmedTitleResolver : IValueResolver<object, object, string>
{
    public string Resolve(object source, object destination, string destMember, ResolutionContext context)
    {
        return source switch
        {
            CourseCreateUpdateRequestDto courseSource => TrimTitle(courseSource.Title),
            SectionCreateUpdateRequestDto sectionSource => TrimTitle(sectionSource.Title),
            _ => throw new ArgumentException($"Unknown source '{nameof(source)}' type", nameof(source))
        };
    }

    private static string TrimTitle(string title) =>
        title.Trim();
}
