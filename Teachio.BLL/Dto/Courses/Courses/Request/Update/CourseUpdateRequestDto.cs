using System.ComponentModel.DataAnnotations;

namespace Teachio.BLL.Dto.Courses.Courses.Request.Update;

public class CourseUpdateRequestDto : CourseCreateUpdateRequestDto
{
    [Required(ErrorMessage = "Required")]
    public Guid Id { get; set; }
}
