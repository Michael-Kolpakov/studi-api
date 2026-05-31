using System.ComponentModel.DataAnnotations;
using Studi.DAL.Utils.Constants;

namespace Studi.BLL.DTOs.Users.Account.Request;

public abstract class AccountCreateUpdateRequestDto
{
    [Required(ErrorMessage = "Required")]
    [RegularExpression(EntityConstants.UserNameRegexPattern, ErrorMessage = "RegexUserName")]
    [StringLength(EntityConstants.MaxUserNameLength, ErrorMessage = "Length")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Required")]
    [RegularExpression(EntityConstants.UserNameRegexPattern, ErrorMessage = "RegexUserName")]
    [StringLength(EntityConstants.MaxUserSurnameLength, ErrorMessage = "Length")]
    public string Surname { get; set; } = null!;
}
