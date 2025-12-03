using AutoMapper;
using Teachio.BLL.Dto.Courses.Courses.Response;
using Teachio.DAL.Entities.Courses.Courses;

namespace Teachio.BLL.Utils.MappingResolvers;

public class TotalDurationResolver : IValueResolver<Course, CoursePreviewResponseDto, float>
{
    public float Resolve(Course source, CoursePreviewResponseDto destination, float destMember, ResolutionContext context)
    {
        var durationSeconds = source.Sections
            .SelectMany(s => s.Videos)
            .Sum(v => v.DurationSeconds);

        var durationHours = durationSeconds / 3600f;

        return MathF.Round(durationHours, 1);
    }
}
