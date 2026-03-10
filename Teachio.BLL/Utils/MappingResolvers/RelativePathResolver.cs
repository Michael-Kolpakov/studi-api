using AutoMapper;
using Teachio.BLL.Dto.Courses.Courses.Response;
using Teachio.BLL.Dto.Courses.Videos.Videos.Response;
using Teachio.BLL.Utils.Constants;
using Teachio.DAL.Entities.Courses.Courses;
using Teachio.DAL.Entities.Courses.Videos.Videos;

namespace Teachio.BLL.Utils.MappingResolvers;

public class RelativePathResolver : IValueResolver<object, object, string>
{
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
