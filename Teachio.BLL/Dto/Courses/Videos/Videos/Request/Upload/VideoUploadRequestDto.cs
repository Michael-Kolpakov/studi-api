using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Teachio.BLL.Dto.Courses.Videos.Videos.Request.Upload;

public class VideoUploadRequestDto
{
    [Required(ErrorMessage = "Required")]
    public Guid VideoId { get; set; }

    [Required(ErrorMessage = "Required")]
    public IFormFile VideoFile { get; set; } = null!;
}
