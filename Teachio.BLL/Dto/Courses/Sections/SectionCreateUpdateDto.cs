namespace Teachio.BLL.Dto.Courses.Sections;

public abstract class SectionCreateUpdateDto
{
    public string? Title { get; set; }

    public Guid CourseId { get; set; }

    public int OrderIndex { get; set; }
}
