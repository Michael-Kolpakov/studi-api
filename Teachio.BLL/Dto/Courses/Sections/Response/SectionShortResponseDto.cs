using Teachio.BLL.Dto.Courses.Videos.Videos.Response;

namespace Teachio.BLL.Dto.Courses.Sections.Response;

/// <summary>
/// Represents the <see cref="SectionShortResponseDto"/> type.
/// </summary>
public class SectionShortResponseDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public int OrderIndex { get; set; }

    public int VideosCount { get; set; }

    public Guid CourseId { get; set; }

    public List<VideoShortResponseDto> Videos { get; set; } = new List<VideoShortResponseDto>();
}
