using System.ComponentModel.DataAnnotations;

namespace Teachio.BLL.Dto.Courses.Courses.Request;

public abstract class CourseCreateUpdateRequestDto
{
    [Required(ErrorMessage = "Required")]
    [StringLength(60, ErrorMessage = "Length")]
    public string Title { get; set; } = null!;

    [StringLength(1000, ErrorMessage = "Length")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Required")]
    public string ThumbnailName { get; set; } = null!;

    [Required(ErrorMessage = "Required")]
    public Guid OwnerUserId { get; set; }
}
