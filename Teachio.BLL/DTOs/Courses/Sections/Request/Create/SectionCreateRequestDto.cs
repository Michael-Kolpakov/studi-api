using System.ComponentModel.DataAnnotations;

namespace Teachio.BLL.DTOs.Courses.Sections.Request.Create;

public class SectionCreateRequestDto : SectionCreateUpdateRequestDto
{
    [Required(ErrorMessage = "Required")]
    public Guid CourseId { get; set; }
}
