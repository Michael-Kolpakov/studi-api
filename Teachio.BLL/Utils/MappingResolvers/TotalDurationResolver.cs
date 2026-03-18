using AutoMapper;
using Teachio.BLL.Dto.Courses.Courses.Response;
using Teachio.DAL.Entities.Courses.Courses;

namespace Teachio.BLL.Utils.MappingResolvers;

/// <summary>
/// Represents the <see cref="TotalDurationResolver"/> type.
/// </summary>
public class TotalDurationResolver : IValueResolver<Course, object, float>
{
    /// <summary>
    /// Maps the input data to the target representation.
    /// </summary>
    /// <param name="source">The source object to map from.</param>
    /// <param name="destination">The destination object to map to.</param>
    /// <param name="destMember">The destination member value.</param>
    /// <param name="context">The mapping context.</param>
    /// <returns>The result produced by this operation.</returns>
    public float Resolve(Course source, object destination, float destMember, ResolutionContext context)
    {
        return destination switch
        {
            CourseResponseDto or CoursePreviewResponseDto or CoursePreviewShortResponseDto => CalculateTotalDurationInHours(source),
            _ => throw new ArgumentException($"Unknown destination '{nameof(destination)}' type", nameof(destination))
        };
    }

    private static float CalculateTotalDurationInHours(Course source)
    {
        var durationSeconds = source.Sections
            .SelectMany(s => s.Videos)
            .Sum(v => v.DurationSeconds);

        var durationHours = durationSeconds / 3600f;

        return MathF.Round(durationHours, 1);
    }
}
