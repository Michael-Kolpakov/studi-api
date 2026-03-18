using System.ComponentModel.DataAnnotations;

namespace Teachio.BLL.Dto.Courses.Sections.Request.Create;

/// <summary>
/// Represents the <see cref="SectionCreateRequestDto"/> type.
/// </summary>
public class SectionCreateRequestDto : SectionCreateUpdateRequestDto
{
    [Required(ErrorMessage = "Required")]
    public Guid CourseId { get; set; }
}
