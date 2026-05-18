using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Teachio.BLL.Models.Auth;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.Utils.Constants;
using Teachio.BLL.Utils.Helpers;
using Teachio.DAL.Entities.Users.Users;

namespace Teachio.BLL.Services.Realizations;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _options;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        ValidateOptions(_options);
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    public AccessTokenResult CreateAccessToken(AppUser user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var now = DateTime.UtcNow;
        var expiresAt = now.AddMinutes(_options.AccessTokenMinutes);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
        }

        if (!string.IsNullOrWhiteSpace(user.UserName))
        {
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
        }

        var fullName = UserFullNameHelper.BuildFullName(user.Name, user.Surname);
        if (!string.IsNullOrWhiteSpace(fullName))
        {
            claims.Add(new Claim(AuthClaimConstants.FullName, fullName));
        }

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now,
            expires: expiresAt,
            signingCredentials: signingCredentials);

        var accessToken = _tokenHandler.WriteToken(token);

        return new AccessTokenResult(accessToken, expiresAt);
    }

    private static void ValidateOptions(JwtOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Issuer))
        {
            throw new InvalidOperationException("Jwt issuer is not configured.");
        }

        if (string.IsNullOrWhiteSpace(options.Audience))
        {
            throw new InvalidOperationException("Jwt audience is not configured.");
        }

        if (string.IsNullOrWhiteSpace(options.SigningKey))
        {
            throw new InvalidOperationException("Jwt signing key is not configured.");
        }

        var signingKeyBytes = Encoding.UTF8.GetByteCount(options.SigningKey);
        if (signingKeyBytes < 32)
        {
            throw new InvalidOperationException("Jwt signing key must be at least 256 bits (32 bytes).");
        }

        if (options.AccessTokenMinutes <= 0)
        {
            throw new InvalidOperationException("Jwt access token minutes must be positive.");
        }

        if (options.RefreshTokenDays <= 0)
        {
            throw new InvalidOperationException("Jwt refresh token days must be positive.");
        }
    }
}
