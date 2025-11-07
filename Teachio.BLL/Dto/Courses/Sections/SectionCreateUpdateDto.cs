using System.ComponentModel.DataAnnotations;

namespace Teachio.BLL.Dto.Courses.Sections;

public abstract class SectionCreateUpdateDto
{
    [Required(ErrorMessage = "Required")]
    [StringLength(60, ErrorMessage = "Length")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "Required")]
    public Guid CourseId { get; set; }

    [Required(ErrorMessage = "Required")]
    public int OrderIndex { get; set; }
}
