using Teachio.BLL.Models.Auth;

namespace Teachio.BLL.Services.Interfaces;

public interface IRefreshTokenService
{
    RefreshTokenResult CreateRefreshToken();

    string HashToken(string refreshToken);

    bool VerifyRefreshToken(string refreshToken, string? storedHash);
}
