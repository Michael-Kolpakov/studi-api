using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Studi.BLL.DTOs.Users.Auth.Response;
using Studi.BLL.Resources.SharedResource;
using Studi.BLL.Services.Interfaces;
using Studi.BLL.SharedResource;
using Studi.BLL.Utils.Helpers;
using Studi.DAL.Entities.Users.Users;

namespace Studi.BLL.Services.Realizations;

public class AuthTokenIssuer : IAuthTokenIssuer
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ILoggerService _logger;
    private readonly IStringLocalizer<AuthSharedResource> _stringLocalizerAuth;

    public AuthTokenIssuer(
        UserManager<AppUser> userManager,
        IJwtTokenService jwtTokenService,
        IRefreshTokenService refreshTokenService,
        ILoggerService logger,
        IStringLocalizer<AuthSharedResource> stringLocalizerAuth)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _refreshTokenService = refreshTokenService;
        _logger = logger;
        _stringLocalizerAuth = stringLocalizerAuth;
    }

    public async Task<Result<AuthTokenPairDto>> IssueTokenPairAsync(AppUser user, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);

        var accessToken = _jwtTokenService.CreateAccessToken(user);
        var refreshToken = _refreshTokenService.CreateRefreshToken();
        var now = DateTime.UtcNow;

        user.RefreshTokenHash = refreshToken.TokenHash;
        user.RefreshTokenCreatedAt = now;
        user.RefreshTokenExpiresAt = refreshToken.ExpiresAtUtc;
        user.RefreshTokenRevokedAt = null;

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            var errorMessage = _stringLocalizerAuth[
                nameof(AuthSharedResource_en.TokenIssueFailed)
            ].Value;

            var errorDetails = string.Join(
                "; ",
                updateResult.Errors.Select(error => $"{error.Code}: {error.Description}"));

            _logger.LogError(null, $"{errorMessage} {errorDetails}");

            return Result.Fail(errorMessage);
        }

        var tokenPair = new AuthTokenPairDto()
        {
            UserId = user.Id,
            AccessToken = accessToken.Token,
            AccessTokenExpiresAtUtc = accessToken.ExpiresAtUtc,
            FullName = UserFullNameHelper.BuildFullName(user.Name, user.Surname),
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiresAtUtc = refreshToken.ExpiresAtUtc
        };

        return Result.Ok(tokenPair);
    }
}
