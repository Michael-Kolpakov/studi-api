using System.ComponentModel.DataAnnotations;

namespace Studi.BLL.DTOs.Courses.Courses.Request.Update;

public class CourseUpdateRequestDto : CourseCreateUpdateRequestDto
{
    [Required(ErrorMessage = "Required")]
    public Guid Id { get; set; }
}
