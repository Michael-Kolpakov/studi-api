using System.ComponentModel.DataAnnotations;

namespace Studi.BLL.DTOs.Courses.Courses.Request.Enroll;

public class CourseEnrollRequestDto
{
    [Required(ErrorMessage = "Required")]
    public Guid CourseId { get; set; }
}
