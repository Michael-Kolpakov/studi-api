using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Studi.BLL.Utils.Attributes;
using Studi.DAL.Utils.Constants;

namespace Studi.BLL.DTOs.Users.Account.Request.Upload;

public class AvatarUploadRequestDto
{
    [Required(ErrorMessage = "Required")]
    [NotEmptyFile(ErrorMessage = "FileEmpty")]
    [MaxFileSize(EntityConstants.MaxAvatarFileSizeBytes, ErrorMessage = "FileSizeMax")]
    [AllowedContentTypes(typeof(EntityConstants), nameof(EntityConstants.AllowedAvatarContentTypes), ErrorMessage = "FileContentType")]
    public IFormFile AvatarFile { get; set; } = null!;
}
