using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Teachio.BLL.Dto.Courses.Videos.Videos.Request;

public abstract class VideoCreateUpdateRequestDto
{
    [Required(ErrorMessage = "Required")]
    [StringLength(60, ErrorMessage = "Length")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "Required")]
    public Guid SectionId { get; set; }

    [Required(ErrorMessage = "Required")]
    public int OrderIndex { get; set; }

    [Required(ErrorMessage = "Required")]
    public IFormFile VideoFile { get; set; } = null!;
}
