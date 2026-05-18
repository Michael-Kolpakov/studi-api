namespace Teachio.BLL.DTOs.Users.Account.Response;

public class AvatarUploadResponseDto
{
    public Guid Id { get; set; }

    public string AvatarName { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public string Resolution { get; set; } = null!;

    public Guid AppUserId { get; set; }
}
