using Studi.BLL.DTOs.Courses.Videos.VideoProgress.Response;

namespace Studi.BLL.DTOs.Courses.Videos.Videos.Response;

public class VideoResponseDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public Guid SectionId { get; set; }

    public int OrderIndex { get; set; }

    public int? DurationSeconds { get; set; }

    public VideoProgressResponseDto VideoProgress { get; set; } = null!;
}
