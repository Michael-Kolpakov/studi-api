using System.ComponentModel.DataAnnotations;

namespace Teachio.BLL.Dto.Courses.Sections.Request;

public abstract class SectionCreateUpdateRequestDto
{
    [Required(ErrorMessage = "Required")]
    [StringLength(60, ErrorMessage = "Length")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "Required")]
    public int OrderIndex { get; set; }
}
