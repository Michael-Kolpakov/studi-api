namespace Teachio.BLL.DTOs.Courses.Videos.Videos.Response;

public class VideoEditShortResponseDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public Guid SectionId { get; set; }

    public int OrderIndex { get; set; }

    public int? DurationSeconds { get; set; }
}
