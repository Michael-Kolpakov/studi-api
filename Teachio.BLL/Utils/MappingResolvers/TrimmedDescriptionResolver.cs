using AutoMapper;
using Teachio.BLL.DTOs.Courses.Courses.Request;
using Teachio.BLL.Utils.Helpers;
using Teachio.DAL.Entities.Courses.Courses;

namespace Teachio.BLL.Utils.MappingResolvers;

public class TrimmedDescriptionResolver : IValueResolver<object, object, string?>
{
    public string? Resolve(object source, object destination, string? destMember, ResolutionContext context)
    {
        return (source, destination) switch
        {
            (CourseCreateUpdateRequestDto courseSource, Course) =>
                TrimAndCapitalizeDescription(courseSource.Description),
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
