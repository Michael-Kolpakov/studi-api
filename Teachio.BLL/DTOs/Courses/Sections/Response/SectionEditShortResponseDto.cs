namespace Teachio.BLL.DTOs.Courses.Sections.Response;

public class SectionEditShortResponseDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public int OrderIndex { get; set; }

    public int VideosCount { get; set; }

    public Guid CourseId { get; set; }
}
