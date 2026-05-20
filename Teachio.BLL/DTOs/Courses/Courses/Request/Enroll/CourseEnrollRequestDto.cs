using System.ComponentModel.DataAnnotations;

namespace Teachio.BLL.DTOs.Courses.Courses.Request.Enroll;

public class CourseEnrollRequestDto
{
    [Required(ErrorMessage = "Required")]
    public Guid CourseId { get; set; }
}
