using System.ComponentModel.DataAnnotations;

namespace Teachio.BLL.Dto.Users.Update;

public class AppUserUpdateDto : AppUserCreateUpdateDto
{
    [Required(ErrorMessage = "'{0}' field is required")]
    public Guid Id { get; set; }
}
