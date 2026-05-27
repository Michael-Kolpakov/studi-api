using System.ComponentModel.DataAnnotations;

namespace Studi.BLL.DTOs.Courses.Sections.Request.Create;

public class SectionCreateRequestDto : SectionCreateUpdateRequestDto
{
    [Required(ErrorMessage = "Required")]
    public Guid CourseId { get; set; }
}
