using System.ComponentModel.DataAnnotations;

namespace Teachio.BLL.Dto.Courses.Courses.Request.Update;

/// <summary>
/// Represents the <see cref="CourseUpdateRequestDto"/> type.
/// </summary>
public class CourseUpdateRequestDto : CourseCreateUpdateRequestDto
{
    [Required(ErrorMessage = "Required")]
    public Guid Id { get; set; }
}
