using AutoMapper;
using Teachio.BLL.Dto.Courses.Courses.Response;
using Teachio.BLL.Dto.Courses.Videos.Videos.Response;
using Teachio.BLL.Utils.Constants;
using Teachio.DAL.Entities.Courses.Courses;
using Teachio.DAL.Entities.Courses.Videos.Videos;

namespace Teachio.BLL.Utils.MappingResolvers;

/// <summary>
/// Represents the <see cref="RelativePathResolver"/> type.
/// </summary>
public class RelativePathResolver : IValueResolver<object, object, string>
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
            (Course course, CoursePreviewResponseDto or CoursePreviewShortResponseDto) => CreateThumbnailRelativePath(course),
            (Video video, VideoResponseDto) => CreateVideoRelativePath(video),
            _ => throw new ArgumentException($"Unknown destination '{nameof(destination)}' type", nameof(destination))
        };
    }

    private static string CreateThumbnailRelativePath(Course source)
    {
        return HandlerConstants.ThumbnailRelativePathTemplate
            .Replace("{AppUser}", source.OwnerUser.Email)
            .Replace("{CourseName}", source.CourseName)
            .Replace("{ThumbnailName}", source.ThumbnailName);
    }

    private static string CreateVideoRelativePath(Video source)
    {
        return HandlerConstants.VideoRelativePathTemplate
            .Replace("{AppUser}", source.Section!.Course!.OwnerUser.Email)
            .Replace("{CourseName}", source.Section.Course.CourseName)
            .Replace("{SectionName}", source.Section.SectionName)
            .Replace("{VideoName}", source.VideoName);
    }
}
