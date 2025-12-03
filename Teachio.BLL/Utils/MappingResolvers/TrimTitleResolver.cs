using AutoMapper;
using Teachio.BLL.Dto.Courses.Courses.Request;
using Teachio.BLL.Dto.Courses.Sections.Request;

namespace Teachio.BLL.Utils.MappingResolvers;

public class TrimTitleResolver : IValueResolver<object, object, string>
{
    public string Resolve(object source, object destination, string destMember, ResolutionContext context)
    {
        return source switch
        {
            CourseCreateUpdateRequestDto courseSource => FormatTitle(courseSource.Title),
            SectionCreateUpdateRequestDto sectionSource => FormatTitle(sectionSource.Title),
            _ => throw new ArgumentException($"Unknown source '{nameof(source)}' type", nameof(source))
        };
    }

    private static string FormatTitle(string title) =>
        title.Trim();
}
