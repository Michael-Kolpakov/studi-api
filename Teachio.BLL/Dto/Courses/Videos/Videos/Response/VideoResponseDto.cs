using Teachio.BLL.Dto.Courses.Videos.VideoProgress.Response;

namespace Teachio.BLL.Dto.Courses.Videos.Videos.Response;

public class VideoResponseDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string? ContentType { get; set; }

    public string VideoRelativePath { get; set; } = null!;

    public Guid SectionId { get; set; }

    public int OrderIndex { get; set; }

    public int DurationSeconds { get; set; }

    public VideoProgressResponseDto VideoProgress { get; set; } = null!;
}
