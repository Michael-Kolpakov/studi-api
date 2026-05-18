using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Teachio.BLL.DTOs.Users.Auth.Response;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResource;
using Teachio.DAL.Entities.Users.Users;

namespace Teachio.BLL.CQRS.Users.Auth.Refresh;

public class RefreshHandler : IRequestHandler<RefreshCommand, Result<AuthTokenPairDto>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IAuthTokenIssuer _authTokenIssuer;
    private readonly ILoggerService _logger;
    private readonly IStringLocalizer<AuthSharedResource> _stringLocalizerAuth;

    public RefreshHandler(
        UserManager<AppUser> userManager,
        IRefreshTokenService refreshTokenService,
        IAuthTokenIssuer authTokenIssuer,
        ILoggerService logger,
        IStringLocalizer<AuthSharedResource> stringLocalizerAuth)
    {
        _userManager = userManager;
        _refreshTokenService = refreshTokenService;
        _authTokenIssuer = authTokenIssuer;
        _logger = logger;
        _stringLocalizerAuth = stringLocalizerAuth;
    }

    public async Task<Result<AuthTokenPairDto>> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Entered '{GetType().Name}' to refresh authentication tokens");

        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            var errorMessage = _stringLocalizerAuth[
                nameof(AuthSharedResource_en.RefreshTokenMissing)
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var refreshTokenHash = _refreshTokenService.HashToken(request.RefreshToken);

        var user = await _userManager.Users.SingleOrDefaultAsync(
            u => u.RefreshTokenHash == refreshTokenHash,
            cancellationToken);

        if (user is null)
        {
            var errorMessage = _stringLocalizerAuth[
                nameof(AuthSharedResource_en.RefreshTokenInvalid)
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var userEmail = user.Email ?? user.UserName ?? user.Id.ToString();
        _logger.LogInformation($"Entered '{GetType().Name}' to refresh authentication tokens for Email: {userEmail}");

        if (!_refreshTokenService.VerifyRefreshToken(request.RefreshToken, user.RefreshTokenHash))
        {
            var errorMessage = _stringLocalizerAuth[
                nameof(AuthSharedResource_en.RefreshTokenInvalid)
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        if (user.RefreshTokenRevokedAt is not null)
        {
            var errorMessage = _stringLocalizerAuth[
                nameof(AuthSharedResource_en.RefreshTokenRevoked)
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        if (user.RefreshTokenExpiresAt is null || user.RefreshTokenExpiresAt <= DateTime.UtcNow)
        {
            var errorMessage = _stringLocalizerAuth[
                nameof(AuthSharedResource_en.RefreshTokenExpired)
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        return await _authTokenIssuer.IssueTokenPairAsync(user, cancellationToken);
    }
}
