using System.ComponentModel.DataAnnotations;

namespace Teachio.BLL.Dto.Courses.Sections.Update;

public class SectionUpdateDto : SectionCreateUpdateDto
{
    [Required(ErrorMessage = "'{0}' field is required")]
    public Guid Id { get; set; }
}
