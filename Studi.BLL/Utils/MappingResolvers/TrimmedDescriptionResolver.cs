using AutoMapper;
using Studi.BLL.DTOs.Courses.Courses.Request;
using Studi.BLL.DTOs.Courses.Videos.Videos.Request;
using Studi.BLL.Utils.Helpers;
using Studi.DAL.Entities.Courses.Courses;
using Studi.DAL.Entities.Courses.Videos.Videos;

namespace Studi.BLL.Utils.MappingResolvers;

public class TrimmedDescriptionResolver : IValueResolver<object, object, string?>
{
    public string? Resolve(object source, object destination, string? destMember, ResolutionContext context)
    {
        return (source, destination) switch
        {
            (CourseCreateUpdateRequestDto courseSource, Course) =>
                TrimAndCapitalizeDescription(courseSource.Description),
            (VideoCreateUpdateRequestDto videoSource, Video) =>
                TrimAndCapitalizeDescription(videoSource.Description),
            _ => throw new ArgumentException(
                $"Unknown source '{source.GetType().Name}' and destination '{destination.GetType().Name}' types combination.")
        };
    }

    private static string? TrimAndCapitalizeDescription(string? description)
    {
        if (description == null)
        {
            return null;
        }

        return TextNormalizationHelper.TrimAndCapitalizeFirstLatinLetter(description);
    }
}
