using AutoMapper;
using Teachio.BLL.DTOs.Courses.Courses.Response;
using Teachio.BLL.DTOs.Courses.Videos.Videos.Response;
using Teachio.BLL.Utils.Constants;
using Teachio.DAL.Entities.Courses.Courses;
using Teachio.DAL.Entities.Courses.Videos.Videos;

namespace Teachio.BLL.Utils.MappingResolvers;

public class RelativePathResolver : IValueResolver<object, object, string?>
{
    public string? Resolve(object source, object destination, string? destMember, ResolutionContext context)
    {
        return (source, destination) switch
        {
            (Course course, CoursePreviewResponseDto or CoursePreviewShortResponseDto) => CreateThumbnailRelativePath(course),
            (Video video, VideoResponseDto) => CreateVideoRelativePath(video),
            _ => throw new ArgumentException($"Unknown destination '{nameof(destination)}' type", nameof(destination))
        };
    }

    private static string? CreateThumbnailRelativePath(Course source)
    {
        if (source.ThumbnailFile is null)
        {
            return null;
        }

        return HandlerConstants.ThumbnailRelativePathTemplate
            .Replace("{AppUser}", source.OwnerUser.Email)
            .Replace("{CourseName}", source.CourseName)
            .Replace("{ThumbnailName}", source.ThumbnailFile.ThumbnailName);
    }

    private static string? CreateVideoRelativePath(Video source)
    {
        if (source.VideoFile is null)
        {
            return null;
        }

        var section = source.Section;
        var course = section?.Course;
        var owner = course?.OwnerUser;

        if (section is null || course is null || owner is null)
        {
            return null;
        }

        return HandlerConstants.VideoRelativePathTemplate
            .Replace("{AppUser}", owner.Email)
            .Replace("{CourseName}", course.CourseName)
            .Replace("{SectionName}", section.SectionName)
            .Replace("{VideoName}", source.VideoFile.VideoName);
    }
}
