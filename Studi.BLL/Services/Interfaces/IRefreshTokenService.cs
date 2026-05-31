using Studi.BLL.Models.Auth;

namespace Studi.BLL.Services.Interfaces;

public interface IRefreshTokenService
{
    RefreshTokenResult CreateRefreshToken();

    string HashToken(string refreshToken);

    bool VerifyRefreshToken(string refreshToken, string? storedHash);
}
