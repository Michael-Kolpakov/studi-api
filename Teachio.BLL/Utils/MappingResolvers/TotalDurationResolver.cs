using AutoMapper;
using Teachio.BLL.Dto.Courses.Courses.Response;
using Teachio.DAL.Entities.Courses.Courses;

namespace Teachio.BLL.Utils.MappingResolvers;

public class TotalDurationResolver : IValueResolver<Course, object, float>
{
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
            .Sum(v => v.VideoFile?.DurationSeconds ?? 0);

        var durationHours = durationSeconds / 3600f;

        return MathF.Round(durationHours, 1);
    }
}
