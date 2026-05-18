using System.ComponentModel.DataAnnotations;
using Teachio.DAL.Utils.Constants;

namespace Teachio.BLL.DTOs.Users.Account.Request.Delete;

public class AccountDeleteRequestDto
{
    [Required(ErrorMessage = "Required")]
    [StringLength(EntityConstants.MaxUserPasswordLength, MinimumLength = EntityConstants.MinUserPasswordLength, ErrorMessage = "LengthRange")]
    public string Password { get; set; } = null!;
}
