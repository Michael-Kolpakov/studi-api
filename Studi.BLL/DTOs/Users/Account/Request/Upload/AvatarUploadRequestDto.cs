using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Studi.BLL.DTOs.Users.Account.Request.Upload;

public class AvatarUploadRequestDto
{
    [Required(ErrorMessage = "Required")]
    public IFormFile AvatarFile { get; set; } = null!;
}
