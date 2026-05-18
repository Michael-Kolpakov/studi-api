using System.ComponentModel.DataAnnotations;
using Teachio.DAL.Utils.Constants;

namespace Teachio.BLL.DTOs.Users.Account.Request;

public abstract class AccountCreateUpdateRequestDto
{
    [Required(ErrorMessage = "Required")]
    [StringLength(EntityConstants.MaxUserNameLength, ErrorMessage = "Length")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Required")]
    [StringLength(EntityConstants.MaxUserSurnameLength, ErrorMessage = "Length")]
    public string Surname { get; set; } = null!;
}
