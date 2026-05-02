using System.ComponentModel.DataAnnotations;
using Teachio.DAL.Utils.Constants;

namespace Teachio.BLL.DTOs.Users.Request;

public abstract class AppUserCreateUpdateRequestDto
{
    [Required(ErrorMessage = "Required")]
    [StringLength(EntityConstants.MaxUserNameLength, ErrorMessage = "Length")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Required")]
    [StringLength(EntityConstants.MaxUserSurnameLength, ErrorMessage = "Length")]
    public string Surname { get; set; } = null!;
}
