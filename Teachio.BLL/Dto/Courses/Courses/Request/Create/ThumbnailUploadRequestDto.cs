using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Teachio.BLL.Dto.Courses.Courses.Request.Create;

/// <summary>
/// Represents the <see cref="ThumbnailUploadRequestDto"/> type.
/// </summary>
public class ThumbnailUploadRequestDto
{
    [Required(ErrorMessage = "Required")]
    public IFormFile ThumbnailFile { get; set; } = null!;
}
