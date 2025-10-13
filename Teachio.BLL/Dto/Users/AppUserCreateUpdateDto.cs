namespace Teachio.BLL.Dto.Users;

public abstract class AppUserCreateUpdateDto
{
    public string? Name { get; set; }

    public string? Surname { get; set; }

    public string? Role { get; set; }
}
