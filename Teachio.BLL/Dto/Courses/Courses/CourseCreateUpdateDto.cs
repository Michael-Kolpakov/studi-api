namespace Teachio.BLL.Dto.Courses.Courses;

public abstract class CourseCreateUpdateDto
{
    public string? Title { get; set; }

    public string? Description { get; set; }

    public Guid OwnerUserId { get; set; }
}
