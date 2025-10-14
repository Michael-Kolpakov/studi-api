using System.ComponentModel.DataAnnotations;

namespace Teachio.BLL.Dto.Courses.Sections;

public abstract class SectionCreateUpdateDto
{
    [Required(ErrorMessage = "'{0}' field is required")]
    [StringLength(60, ErrorMessage = "Length of '{0}' must be not longer than {1} characters")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "'{0}' field is required")]
    public Guid CourseId { get; set; }

    [Required(ErrorMessage = "'{0}' field is required")]
    public int OrderIndex { get; set; }
}
