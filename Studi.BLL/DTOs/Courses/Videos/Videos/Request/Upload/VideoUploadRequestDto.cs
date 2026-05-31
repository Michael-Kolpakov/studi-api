using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Studi.BLL.Utils.Attributes;
using Studi.DAL.Utils.Constants;

namespace Studi.BLL.DTOs.Courses.Videos.Videos.Request.Upload;

public class VideoUploadRequestDto
{
    [Required(ErrorMessage = "Required")]
    public Guid VideoId { get; set; }

    [Required(ErrorMessage = "Required")]
    [NotEmptyFile(ErrorMessage = "FileEmpty")]
    [MaxFileSize(EntityConstants.MaxVideoFileSizeBytes, ErrorMessage = "FileSizeMax")]
    [AllowedContentTypes(typeof(EntityConstants), nameof(EntityConstants.AllowedVideoContentTypes), ErrorMessage = "FileContentType")]
    public IFormFile VideoFile { get; set; } = null!;
}
