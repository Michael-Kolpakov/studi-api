using AutoMapper;
using Studi.BLL.DTOs.Courses.Courses.Request;
using Studi.BLL.DTOs.Courses.Sections.Request;
using Studi.BLL.DTOs.Courses.Videos.Videos.Request;
using Studi.BLL.Utils.Helpers;
using Studi.DAL.Entities.Courses.Courses;
using Studi.DAL.Entities.Courses.Sections;
using Studi.DAL.Entities.Courses.Videos.Videos;

namespace Studi.BLL.Utils.MappingResolvers;

public class TrimmedTitleResolver : IValueResolver<object, object, string>
{
    public string Resolve(object source, object destination, string destMember, ResolutionContext context)
    {
        return (source, destination) switch
        {
            (CourseCreateUpdateRequestDto courseSource, Course) =>
                TrimTitle(courseSource.Title),
            (SectionCreateUpdateRequestDto sectionSource, Section) =>
                TrimTitle(sectionSource.Title),
            (VideoCreateUpdateRequestDto videoSource, Video) =>
                TrimTitle(videoSource.Title),
            _ => throw new ArgumentException(
                $"Unknown source '{source.GetType().Name}' and destination '{destination.GetType().Name}' types combination.")
        };
    }

    private static string TrimTitle(string title) =>
        TextNormalizationHelper.TrimAndCapitalizeFirstLatinLetter(title);
}
