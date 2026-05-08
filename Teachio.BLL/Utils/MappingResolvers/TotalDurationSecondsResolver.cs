using AutoMapper;
using Teachio.BLL.DTOs.Courses.Sections.Response;
using Teachio.DAL.Entities.Courses.Sections;

namespace Teachio.BLL.Utils.MappingResolvers;

public class TotalDurationSecondsResolver : IValueResolver<Section, object, float>
{
    public float Resolve(Section source, object destination, float destMember, ResolutionContext context)
    {
        return destination switch
        {
            SectionResponseDto or SectionPreviewResponseDto or SectionShortResponseDto => CalculateTotalDurationSeconds(source),
            _ => throw new ArgumentException($"Unknown destination '{nameof(destination)}' type", nameof(destination))
        };
    }

    private static float CalculateTotalDurationSeconds(Section source)
    {
        var durationSeconds = source.Videos?
            .Sum(v => v.VideoFile?.DurationSeconds ?? 0) ?? 0;

        return durationSeconds;
    }
}
