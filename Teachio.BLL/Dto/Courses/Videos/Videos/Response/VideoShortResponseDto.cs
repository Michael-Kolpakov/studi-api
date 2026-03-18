using Teachio.BLL.Dto.Courses.Videos.VideoProgress.Response;

namespace Teachio.BLL.Dto.Courses.Videos.Videos.Response;

/// <summary>
/// Represents the <see cref="VideoShortResponseDto"/> type.
/// </summary>
public class VideoShortResponseDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public Guid SectionId { get; set; }

    public int OrderIndex { get; set; }

    public int DurationSeconds { get; set; }

    public VideoProgressResponseDto VideoProgress { get; set; } = null!;
}
