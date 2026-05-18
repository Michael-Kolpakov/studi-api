using System.ComponentModel.DataAnnotations;
using Teachio.BLL.DTOs.Users.Account.Request;
using Teachio.DAL.Utils.Constants;

namespace Teachio.BLL.DTOs.Users.Auth.Request;

public class AuthRegisterRequestDto : AppUserCreateUpdateRequestDto
{
    [Required(ErrorMessage = "Required")]
    [EmailAddress(ErrorMessage = "Email")]
    [StringLength(EntityConstants.MaxUserEmailLength, ErrorMessage = "Length")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Required")]
    [StringLength(EntityConstants.MaxUserPasswordLength, MinimumLength = EntityConstants.MinUserPasswordLength, ErrorMessage = "LengthRange")]
    public string Password { get; set; } = null!;
}
