using System.ComponentModel.DataAnnotations;

namespace Studi.BLL.DTOs.Courses.Sections.Request.Update;

public class SectionUpdateRequestDto : SectionCreateUpdateRequestDto
{
    [Required(ErrorMessage = "Required")]
    public Guid Id { get; set; }
}
