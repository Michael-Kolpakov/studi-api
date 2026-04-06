using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Teachio.BLL.Dto.Courses.Courses.Request.Create;

public class ThumbnailUploadRequestDto
{
    [Required(ErrorMessage = "Required")]
    public IFormFile ThumbnailFile { get; set; } = null!;
}
