using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Teachio.BLL.Dto.Courses.Videos.Videos;

public abstract class VideoCreateUpdateDto
{
    [Required(ErrorMessage = "'{0}' field is required")]
    [StringLength(60, ErrorMessage = "Length of '{0}' must be not longer than {1} characters")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "'{0}' field is required")]
    public Guid SectionId { get; set; }

    [Required(ErrorMessage = "'{0}' field is required")]
    public int OrderIndex { get; set; }

    [Required(ErrorMessage = "'{0}' field is required")]
    public IFormFile File { get; set; } = null!;
}
