using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Studi.BLL.DTOs.Courses.Courses.Request.Upload;

public class ThumbnailUploadRequestDto
{
    [Required(ErrorMessage = "Required")]
    public Guid CourseId { get; set; }

    [Required(ErrorMessage = "Required")]
    public IFormFile ThumbnailFile { get; set; } = null!;
}
