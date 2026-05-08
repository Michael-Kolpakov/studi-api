using System.Linq.Expressions;
using AutoMapper;
using Teachio.BLL.DTOs.Courses.Courses.Response;
using Teachio.DAL.Entities.Courses.Courses;
using Teachio.DAL.Entities.Courses.Videos.Videos;

namespace Teachio.BLL.Utils.MappingResolvers;

public class TotalDurationHoursResolver : IValueResolver<Course, object, float>
{
    public static Expression<Func<Course, float>> TotalDurationHoursExpression { get; } =
        course => (float)Math.Round(
            (course.Sections
                .SelectMany(s => s.Videos)
                .Sum(v => (int?)(v.VideoFile == null ? 0 : v.VideoFile.DurationSeconds)) ?? 0) / 3600f,
            1);

    public float Resolve(Course source, object destination, float destMember, ResolutionContext context)
    {
        return destination switch
        {
            CourseResponseDto or CoursePreviewResponseDto or CoursePreviewShortResponseDto => CalculateTotalDurationHours(source),
            _ => throw new ArgumentException($"Unknown destination '{nameof(destination)}' type", nameof(destination))
        };
    }

    private static float CalculateTotalDurationHours(Course source)
    {
        var durationSeconds = source.Sections?
            .SelectMany(s => s.Videos ?? Enumerable.Empty<Video>())
            .Sum(v => v.VideoFile?.DurationSeconds ?? 0) ?? 0;

        var durationHours = durationSeconds / 3600f;

        return MathF.Round(durationHours, 1);
    }
}
