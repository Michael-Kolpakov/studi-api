using System.ComponentModel.DataAnnotations;
using Teachio.DAL.Utils.Constants;

namespace Teachio.BLL.DTOs.Users.Account.Request.Update;

public class AccountUpdateRequestDto : AccountCreateUpdateRequestDto
{
    [StringLength(EntityConstants.MaxUserPasswordLength, MinimumLength = EntityConstants.MinUserPasswordLength, ErrorMessage = "LengthRange")]
    public string? CurrentPassword { get; set; }

    [StringLength(EntityConstants.MaxUserPasswordLength, MinimumLength = EntityConstants.MinUserPasswordLength, ErrorMessage = "LengthRange")]
    public string? NewPassword { get; set; }
}
