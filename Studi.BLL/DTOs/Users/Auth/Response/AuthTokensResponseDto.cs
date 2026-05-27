namespace Studi.BLL.DTOs.Users.Auth.Response;

public class AuthTokensResponseDto
{
    public Guid UserId { get; set; }

    public string AccessToken { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string TokenType { get; set; } = "Bearer";

    public DateTime AccessTokenExpiresAtUtc { get; set; }
}
