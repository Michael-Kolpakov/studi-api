using System.ComponentModel.DataAnnotations;

namespace Teachio.BLL.Dto.Courses.Courses;

public abstract class CourseCreateUpdateDto
{
    [Required(ErrorMessage = "'{0}' field is required")]
    [StringLength(60, ErrorMessage = "Length of '{0}' must be not longer than {1} characters")]
    public string Title { get; set; } = null!;

    [StringLength(1000, ErrorMessage = "Length of '{0}' must be not longer than {1} characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "'{0}' field is required")]
    public Guid OwnerUserId { get; set; }
}
