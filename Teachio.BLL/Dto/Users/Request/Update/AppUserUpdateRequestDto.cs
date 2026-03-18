using System.ComponentModel.DataAnnotations;

namespace Teachio.BLL.Dto.Users.Request.Update;

/// <summary>
/// Represents the <see cref="AppUserUpdateRequestDto"/> type.
/// </summary>
public class AppUserUpdateRequestDto : AppUserCreateUpdateRequestDto
{
    [Required(ErrorMessage = "Required")]
    public Guid Id { get; set; }
}
