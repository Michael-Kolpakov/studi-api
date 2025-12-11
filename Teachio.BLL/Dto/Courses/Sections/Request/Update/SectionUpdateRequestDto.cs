using System.ComponentModel.DataAnnotations;

namespace Teachio.BLL.Dto.Courses.Sections.Request.Update;

public class SectionUpdateRequestDto : SectionCreateUpdateRequestDto
{
    [Required(ErrorMessage = "Required")]
    public Guid Id { get; set; }
}
