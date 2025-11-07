using System.ComponentModel.DataAnnotations;

namespace Teachio.BLL.Dto.Courses.Courses.Update;

public class CourseUpdateDto : CourseCreateUpdateDto
{
    [Required(ErrorMessage = "Required")]
    public Guid Id { get; set; }
}
