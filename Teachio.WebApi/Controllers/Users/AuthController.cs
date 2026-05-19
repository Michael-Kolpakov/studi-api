using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Teachio.BLL.CQRS.Users.Auth.Login;
using Teachio.BLL.CQRS.Users.Auth.Logout;
using Teachio.BLL.CQRS.Users.Auth.Refresh;
using Teachio.BLL.CQRS.Users.Auth.Register;
using Teachio.BLL.CQRS.Users.Auth.VerifyPin;
using Teachio.BLL.DTOs.Users.Auth.Request;
using Teachio.BLL.DTOs.Users.Auth.Response;
using Teachio.WebApi.Utils.Constants;
using Teachio.WebApi.Utils.RelativeRoutes;

namespace Teachio.WebApi.Controllers.Users;

public class AuthController : BaseApiController
{
    /// <summary>
    /// Sends a registration PIN code to the user's email.
    /// </summary>
    /// <param name="authRegisterRequestDto">The registration data.</param>
    /// <returns>Returns registration PIN details.</returns>
    [HttpPost(AuthRelativeRoutes.Register)]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RegistrationPinResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] AuthRegisterRequestDto authRegisterRequestDto)
    {
        var result = await Mediator.Send(new RegisterCommand(authRegisterRequestDto));

        if (result.IsFailed)
        {
            return HandleResult(result);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Verifies a registration PIN code and issues access tokens.
    /// </summary>
    /// <param name="verifyRegistrationPinRequestDto">The PIN verification data.</param>
    /// <returns>Returns access token details.</returns>
    [HttpPost(AuthRelativeRoutes.VerifyPin)]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthTokensResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyPin([FromBody] VerifyRegistrationPinRequestDto verifyRegistrationPinRequestDto)
    {
        var result = await Mediator.Send(new VerifyRegistrationPinCommand(verifyRegistrationPinRequestDto));

        if (result.IsFailed)
        {
            return HandleResult(result);
        }

        SetRefreshTokenCookie(result.Value.RefreshToken, result.Value.RefreshTokenExpiresAtUtc);

        return Ok(CreateTokensResponse(result.Value));
    }

    /// <summary>
    /// Logs in a user and returns access token details.
    /// </summary>
    /// <param name="authLoginRequestDto">The login data.</param>
    /// <returns>Returns access token details.</returns>
    [HttpPost(AuthRelativeRoutes.Login)]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthTokensResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] AuthLoginRequestDto authLoginRequestDto)
    {
        var result = await Mediator.Send(new LoginCommand(authLoginRequestDto));

        if (result.IsFailed)
        {
            return HandleResult(result);
        }

        SetRefreshTokenCookie(result.Value.RefreshToken, result.Value.RefreshTokenExpiresAtUtc);

        return Ok(CreateTokensResponse(result.Value));
    }

    /// <summary>
    /// Refreshes access token using refresh token cookie.
    /// </summary>
    /// <returns>Returns refreshed access token details.</returns>
    [HttpPost(AuthRelativeRoutes.Refresh)]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthTokensResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies[AuthConstants.RefreshTokenCookieName];
        var result = await Mediator.Send(new RefreshCommand(refreshToken));

        if (result.IsFailed)
        {
            DeleteRefreshTokenCookie();

            return HandleResult(result);
        }

        SetRefreshTokenCookie(result.Value.RefreshToken, result.Value.RefreshTokenExpiresAtUtc);

        return Ok(CreateTokensResponse(result.Value));
    }

    /// <summary>
    /// Logs out a user and revokes refresh token.
    /// </summary>
    /// <returns>Returns success result.</returns>
    [HttpPost(AuthRelativeRoutes.Logout)]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies[AuthConstants.RefreshTokenCookieName];
        var result = await Mediator.Send(new LogoutCommand(refreshToken));

        DeleteRefreshTokenCookie();

        return HandleResult(result);
    }

    private static AuthTokensResponseDto CreateTokensResponse(AuthTokenPairDto tokenPair)
    {
        return new AuthTokensResponseDto()
        {
            UserId = tokenPair.UserId,
            AccessToken = tokenPair.AccessToken,
            FullName = tokenPair.FullName,
            AccessTokenExpiresAtUtc = tokenPair.AccessTokenExpiresAtUtc
        };
    }

    private void SetRefreshTokenCookie(string refreshToken, DateTime expiresAtUtc)
    {
        var cookieOptions = BuildRefreshTokenCookieOptions(expiresAtUtc);
        Response.Cookies.Append(AuthConstants.RefreshTokenCookieName, refreshToken, cookieOptions);
    }

    private void DeleteRefreshTokenCookie()
    {
        var cookieOptions = BuildRefreshTokenCookieOptions(DateTime.UtcNow.AddDays(-1));
        Response.Cookies.Delete(AuthConstants.RefreshTokenCookieName, cookieOptions);
    }

    private static CookieOptions BuildRefreshTokenCookieOptions(DateTime expiresAtUtc)
    {
        return new CookieOptions()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = expiresAtUtc,
            IsEssential = true,
            Path = "/"
        };
    }
}
