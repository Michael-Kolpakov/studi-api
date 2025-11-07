using System.ComponentModel.DataAnnotations;

namespace Teachio.BLL.Dto.Courses.Sections.Update;

public class SectionUpdateDto : SectionCreateUpdateDto
{
    [Required(ErrorMessage = "Required")]
    public Guid Id { get; set; }
}
