using Teachio.BLL.DTOs.Courses.Videos.VideoProgress.Response;

namespace Teachio.BLL.DTOs.Courses.Videos.Videos.Response;

public class VideoShortResponseDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public Guid SectionId { get; set; }

    public int OrderIndex { get; set; }

    public int DurationSeconds { get; set; }

    public VideoProgressResponseDto VideoProgress { get; set; } = null!;
}
