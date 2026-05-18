using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Teachio.BLL.CQRS.Users.Account.Delete;
using Teachio.BLL.CQRS.Users.Account.GetCurrent;
using Teachio.BLL.CQRS.Users.Account.Update;
using Teachio.BLL.DTOs.Users.Account.Request.Delete;
using Teachio.BLL.DTOs.Users.Account.Request.Update;
using Teachio.BLL.DTOs.Users.Account.Response;
using Teachio.WebApi.Utils.RelativeRoutes;

namespace Teachio.WebApi.Controllers.Users;

[Authorize]
public class UsersController : BaseApiController
{
    /// <summary>
    /// Retrieves the current user's profile.
    /// </summary>
    /// <returns>Returns the current user's profile.</returns>
    [HttpGet(UsersRelativeRoutes.GetCurrent)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AppUserResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrent()
    {
        return HandleResult(await Mediator.Send(new GetCurrentUserQuery()));
    }

    /// <summary>
    /// Updates the current user's profile and password.
    /// </summary>
    /// <param name="accountUpdateRequestDto">The update data.</param>
    /// <returns>Returns the updated user profile.</returns>
    [HttpPut(UsersRelativeRoutes.Update)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AppUserResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update([FromBody] AccountUpdateRequestDto accountUpdateRequestDto)
    {
        return HandleResult(await Mediator.Send(new UpdateAccountCommand(accountUpdateRequestDto)));
    }

    /// <summary>
    /// Deletes the current user's account.
    /// </summary>
    /// <param name="accountDeleteRequestDto">The delete confirmation data.</param>
    /// <returns>Returns the deleted user profile.</returns>
    [HttpDelete(UsersRelativeRoutes.Delete)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AppUserResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete([FromBody] AccountDeleteRequestDto accountDeleteRequestDto)
    {
        return HandleResult(await Mediator.Send(new DeleteAccountCommand(accountDeleteRequestDto)));
    }
}
