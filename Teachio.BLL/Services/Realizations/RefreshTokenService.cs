using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Teachio.BLL.Models.Auth;
using Teachio.BLL.Services.Interfaces;

namespace Teachio.BLL.Services.Realizations;

public class RefreshTokenService : IRefreshTokenService
{
    private const int RefreshTokenBytesLength = 64;

    private readonly JwtOptions _options;

    public RefreshTokenService(IOptions<JwtOptions> options)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public RefreshTokenResult CreateRefreshToken()
    {
        var tokenBytes = RandomNumberGenerator.GetBytes(RefreshTokenBytesLength);
        var refreshToken = Base64UrlEncode(tokenBytes);
        var refreshTokenHash = HashToken(refreshToken);
        var expiresAtUtc = DateTime.UtcNow.AddDays(_options.RefreshTokenDays);

        return new RefreshTokenResult(refreshToken, refreshTokenHash, expiresAtUtc);
    }

    public string HashToken(string refreshToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(refreshToken);

        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));

        return Convert.ToBase64String(hashBytes);
    }

    public bool VerifyRefreshToken(string refreshToken, string? storedHash)
    {
        if (string.IsNullOrWhiteSpace(refreshToken) || string.IsNullOrWhiteSpace(storedHash))
        {
            return false;
        }

        try
        {
            var computedHash = HashToken(refreshToken);

            return FixedTimeEquals(computedHash, storedHash);
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    private static bool FixedTimeEquals(string leftHash, string rightHash)
    {
        try
        {
            var leftBytes = Convert.FromBase64String(leftHash);
            var rightBytes = Convert.FromBase64String(rightHash);

            return CryptographicOperations.FixedTimeEquals(leftBytes, rightBytes);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static string Base64UrlEncode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
