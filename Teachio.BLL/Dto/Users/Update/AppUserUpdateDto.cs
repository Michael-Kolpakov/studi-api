using System.ComponentModel.DataAnnotations;

namespace Teachio.BLL.Dto.Users.Update;

public class AppUserUpdateDto : AppUserCreateUpdateDto
{
    [Required(ErrorMessage = "Required")]
    public Guid Id { get; set; }
}
