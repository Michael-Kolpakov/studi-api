using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Teachio.BLL.DTOs.Users.Auth.Response;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResource;
using Teachio.DAL.Entities.Users.Users;

namespace Teachio.BLL.CQRS.Users.Auth.Login;

public class LoginHandler : IRequestHandler<LoginCommand, Result<AuthTokenPairDto>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IAuthTokenIssuer _authTokenIssuer;
    private readonly ILoggerService _logger;
    private readonly IStringLocalizer<AuthSharedResource> _stringLocalizerAuth;

    public LoginHandler(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IAuthTokenIssuer authTokenIssuer,
        ILoggerService logger,
        IStringLocalizer<AuthSharedResource> stringLocalizerAuth)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _authTokenIssuer = authTokenIssuer;
        _logger = logger;
        _stringLocalizerAuth = stringLocalizerAuth;
    }

    public async Task<Result<AuthTokenPairDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var email = request.AuthLoginRequestDto.Email;
        _logger.LogInformation($"Entered '{GetType().Name}' to log in user with Email: {email}");

        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            var errorMessage = _stringLocalizerAuth[
                nameof(AuthSharedResource_en.InvalidCredentials)
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var signInResult = await _signInManager.CheckPasswordSignInAsync(
            user,
            request.AuthLoginRequestDto.Password,
            lockoutOnFailure: false);

        if (!signInResult.Succeeded)
        {
            var errorMessage = _stringLocalizerAuth[
                nameof(AuthSharedResource_en.InvalidCredentials)
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        return await _authTokenIssuer.IssueTokenPairAsync(user, cancellationToken);
    }
}
