using System.ComponentModel.DataAnnotations;

namespace Teachio.BLL.Dto.Users.Request;

/// <summary>
/// Represents the <see cref="AppUserCreateUpdateRequestDto"/> type.
/// </summary>
public abstract class AppUserCreateUpdateRequestDto
{
    [Required(ErrorMessage = "Required")]
    [StringLength(20, ErrorMessage = "Length")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Required")]
    [StringLength(30, ErrorMessage = "Length")]
    public string Surname { get; set; } = null!;
}
