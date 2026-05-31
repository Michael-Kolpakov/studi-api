using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Studi.BLL.Utils.Attributes;
using Studi.DAL.Utils.Constants;

namespace Studi.BLL.DTOs.Courses.Courses.Request.Upload;

public class ThumbnailUploadRequestDto
{
    [Required(ErrorMessage = "Required")]
    public Guid CourseId { get; set; }

    [Required(ErrorMessage = "Required")]
    [NotEmptyFile(ErrorMessage = "FileEmpty")]
    [MaxFileSize(EntityConstants.MaxThumbnailFileSizeBytes, ErrorMessage = "FileSizeMax")]
    [AllowedContentTypes(typeof(EntityConstants), nameof(EntityConstants.AllowedThumbnailContentTypes), ErrorMessage = "FileContentType")]
    public IFormFile ThumbnailFile { get; set; } = null!;
}
