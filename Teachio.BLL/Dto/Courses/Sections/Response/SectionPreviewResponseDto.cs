using Teachio.BLL.Dto.Courses.Videos.Videos.Response;

namespace Teachio.BLL.Dto.Courses.Sections.Response;

/// <summary>
/// Represents the <see cref="SectionPreviewResponseDto"/> type.
/// </summary>
public class SectionPreviewResponseDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public int OrderIndex { get; set; }

    public int VideosCount { get; set; }

    public Guid CourseId { get; set; }

    public List<VideoPreviewResponseDto> Videos { get; set; } = new List<VideoPreviewResponseDto>();
}
