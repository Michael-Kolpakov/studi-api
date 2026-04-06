using System.ComponentModel.DataAnnotations;

namespace Teachio.BLL.Dto.Users.Request.Update;

public class AppUserUpdateRequestDto : AppUserCreateUpdateRequestDto
{
    [Required(ErrorMessage = "Required")]
    public Guid Id { get; set; }
}
