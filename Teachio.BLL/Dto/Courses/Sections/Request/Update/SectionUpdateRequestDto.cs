using System.ComponentModel.DataAnnotations;

namespace Teachio.BLL.Dto.Courses.Sections.Request.Update;

/// <summary>
/// Represents the <see cref="SectionUpdateRequestDto"/> type.
/// </summary>
public class SectionUpdateRequestDto : SectionCreateUpdateRequestDto
{
    [Required(ErrorMessage = "Required")]
    public Guid Id { get; set; }
}
