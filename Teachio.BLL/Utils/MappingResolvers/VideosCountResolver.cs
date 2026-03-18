using AutoMapper;
using Teachio.BLL.Dto.Courses.Courses.Response;
using Teachio.DAL.Entities.Courses.Courses;

namespace Teachio.BLL.Utils.MappingResolvers;

/// <summary>
/// Represents the <see cref="VideosCountResolver"/> type.
/// </summary>
public class VideosCountResolver : IValueResolver<Course, CoursePreviewShortResponseDto, int>
{
    /// <summary>
    /// Maps the input data to the target representation.
    /// </summary>
    /// <param name="source">The source object to map from.</param>
    /// <param name="destination">The destination object to map to.</param>
    /// <param name="destMember">The destination member value.</param>
    /// <param name="context">The mapping context.</param>
    /// <returns>The result produced by this operation.</returns>
    public int Resolve(Course source, CoursePreviewShortResponseDto destination, int destMember, ResolutionContext context)
    {
        return source.Sections.Sum(section => section.VideosCount);
    }
}
