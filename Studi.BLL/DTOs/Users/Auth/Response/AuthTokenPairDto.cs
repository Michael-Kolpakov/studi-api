namespace Studi.BLL.DTOs.Users.Auth.Response;

public class AuthTokenPairDto
{
    public Guid UserId { get; set; }

    public string AccessToken { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public DateTime AccessTokenExpiresAtUtc { get; set; }

    public string RefreshToken { get; set; } = null!;

    public DateTime RefreshTokenExpiresAtUtc { get; set; }
}
