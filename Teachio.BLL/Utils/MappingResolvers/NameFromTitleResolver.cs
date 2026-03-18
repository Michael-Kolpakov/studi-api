using AutoMapper;
using Teachio.BLL.Dto.Courses.Courses.Request;
using Teachio.BLL.Dto.Courses.Sections.Request;
using Teachio.BLL.Dto.Courses.Videos.Videos.Request;
using Teachio.DAL.Entities.Courses.Courses;
using Teachio.DAL.Entities.Courses.Sections;
using Teachio.DAL.Entities.Courses.Videos.Videos;

namespace Teachio.BLL.Utils.MappingResolvers;

/// <summary>
/// Represents the <see cref="NameFromTitleResolver"/> type.
/// </summary>
public class NameFromTitleResolver : IValueResolver<object, object, string>
{
    /// <summary>
    /// Maps the input data to the target representation.
    /// </summary>
    /// <param name="source">The source object to map from.</param>
    /// <param name="destination">The destination object to map to.</param>
    /// <param name="destMember">The destination member value.</param>
    /// <param name="context">The mapping context.</param>
    /// <returns>The result produced by this operation.</returns>
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
            _ => throw new ArgumentException(
                $"Unknown source '{source.GetType().Name}' and destination '{destination.GetType().Name}' types combination.")
        };
    }

    /// <summary>
    /// Creates a new instance in the target store.
    /// </summary>
    /// <param name="title">The <paramref name="title"/> argument.</param>
    /// <returns>The result produced by this operation.</returns>
    public static string CreateNameFromTitle(string title) =>
        string.Join("-", title.Trim().ToLowerInvariant().Split(" ", StringSplitOptions.RemoveEmptyEntries));
}
