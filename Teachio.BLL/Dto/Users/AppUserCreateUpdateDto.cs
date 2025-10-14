using System.ComponentModel.DataAnnotations;

namespace Teachio.BLL.Dto.Users;

public abstract class AppUserCreateUpdateDto
{
    [Required(ErrorMessage = "'{0}' field is required")]
    [StringLength(20, ErrorMessage = "Length of '{0}' must be not longer than {1} characters")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "'{0}' field is required")]
    [StringLength(30, ErrorMessage = "Length of '{0}' must be not longer than {1} characters")]
    public string Surname { get; set; } = null!;

    [Required(ErrorMessage = "'{0}' field is required")]
    [StringLength(20, ErrorMessage = "Length of '{0}' must be not longer than {1} characters")]
    public string Role { get; set; } = null!;
}
