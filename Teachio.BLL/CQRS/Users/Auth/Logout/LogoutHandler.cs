using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResource;
using Teachio.DAL.Entities.Users.Users;

namespace Teachio.BLL.CQRS.Users.Auth.Logout;

public class LogoutHandler : IRequestHandler<LogoutCommand, Result>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ILoggerService _logger;
    private readonly IStringLocalizer<AuthSharedResource> _stringLocalizerAuth;

    public LogoutHandler(
        UserManager<AppUser> userManager,
        IRefreshTokenService refreshTokenService,
        ILoggerService logger,
        IStringLocalizer<AuthSharedResource> stringLocalizerAuth)
    {
        _userManager = userManager;
        _refreshTokenService = refreshTokenService;
        _logger = logger;
        _stringLocalizerAuth = stringLocalizerAuth;
    }

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Entered '{GetType().Name}' to log out user");

        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return Result.Ok();
        }

        var refreshTokenHash = _refreshTokenService.HashToken(request.RefreshToken);

        var user = await _userManager.Users.SingleOrDefaultAsync(
            u => u.RefreshTokenHash == refreshTokenHash,
            cancellationToken);

        if (user is null)
        {
            return Result.Ok();
        }

        var userEmail = user.Email ?? user.UserName ?? user.Id.ToString();
        _logger.LogInformation($"Entered '{GetType().Name}' to log out user with Email: {userEmail}");

        user.RefreshTokenHash = null;
        user.RefreshTokenCreatedAt = null;
        user.RefreshTokenExpiresAt = null;
        user.RefreshTokenRevokedAt = DateTime.UtcNow;

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            var errorMessage = _stringLocalizerAuth[
                nameof(AuthSharedResource_en.UserUpdateFailed)
            ].Value;

            var errorDetails = string.Join(
                "; ",
                updateResult.Errors.Select(error => $"{error.Code}: {error.Description}"));

            _logger.LogError(request, $"{errorMessage} {errorDetails}");

            return Result.Fail(errorMessage);
        }

        return Result.Ok();
    }
}
