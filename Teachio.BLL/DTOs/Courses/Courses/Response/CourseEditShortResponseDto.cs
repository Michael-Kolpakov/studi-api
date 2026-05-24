namespace Teachio.BLL.DTOs.Courses.Courses.Response;

public class CourseEditShortResponseDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }
}
