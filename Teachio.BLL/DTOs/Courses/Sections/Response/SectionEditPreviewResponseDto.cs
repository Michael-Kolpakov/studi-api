namespace Teachio.BLL.DTOs.Courses.Sections.Response;

public class SectionEditPreviewResponseDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public int OrderIndex { get; set; }

    public int VideosCount { get; set; }

    public float TotalDurationSeconds { get; set; }

    public Guid CourseId { get; set; }
}
