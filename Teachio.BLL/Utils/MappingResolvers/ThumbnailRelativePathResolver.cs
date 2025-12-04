using AutoMapper;
using Teachio.BLL.Dto.Courses.Courses.Response;
using Teachio.BLL.Utils.Constants;
using Teachio.DAL.Entities.Courses.Courses;

namespace Teachio.BLL.Utils.MappingResolvers;

public class ThumbnailRelativePathResolver : IValueResolver<Course, object, string>
{
    public string Resolve(Course source, object destination, string destMember, ResolutionContext context)
    {
        return destination switch
        {
            CoursePreviewResponseDto or CoursePreviewShortResponseDto => CreateThumbnailRelativePath(source),
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
}
