using AutoMapper;
using Teachio.BLL.Dto.Courses.Courses.Response;
using Teachio.BLL.Utils.Constants;
using Teachio.DAL.Entities.Courses.Courses;

namespace Teachio.BLL.Utils.MappingResolvers;

public class CreateThumbnailRelativePathResolver : IValueResolver<Course, CoursePreviewResponseDto, string>
{
    public string Resolve(Course source, CoursePreviewResponseDto destination, string destMember, ResolutionContext context)
    {
        return HandlerConstants.ThumbnailRelativePathTemplate
            .Replace("{AppUser}", source.OwnerUser.Email)
            .Replace("{CourseName}", source.CourseName)
            .Replace("{ThumbnailName}", source.ThumbnailName);
    }
}
