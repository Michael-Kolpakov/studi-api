using System.ComponentModel.DataAnnotations;

namespace Teachio.BLL.Dto.Courses.Courses.Update;

public class CourseUpdateDto : CourseCreateUpdateDto
{
    [Required(ErrorMessage = "'{0}' field is required")]
    public Guid Id { get; set; }
}
