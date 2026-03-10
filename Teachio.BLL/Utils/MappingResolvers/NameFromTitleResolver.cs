using AutoMapper;
using Teachio.BLL.Dto.Courses.Courses.Request;
using Teachio.BLL.Dto.Courses.Sections.Request;
using Teachio.BLL.Dto.Courses.Videos.Videos.Request;
using Teachio.DAL.Entities.Courses.Courses;
using Teachio.DAL.Entities.Courses.Sections;
using Teachio.DAL.Entities.Courses.Videos.Videos;

namespace Teachio.BLL.Utils.MappingResolvers;

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
            (VideoCreateUpdateRequestDto videoSource, Video) => 
                CreateNameFromTitle(videoSource.Title),
            (_, _) => throw new ArgumentException(
                $"Unknown source '{source.GetType().Name}' and destination '{destination.GetType().Name}' types combination.")
        };
    }

    public static string CreateNameFromTitle(string title) =>
        string.Join("-", title.Trim().ToLowerInvariant().Split(" ", StringSplitOptions.RemoveEmptyEntries));
}
