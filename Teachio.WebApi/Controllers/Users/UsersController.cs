using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Teachio.BLL.CQRS.Users.Account.Delete;
using Teachio.BLL.CQRS.Users.Account.GetCurrent;
using Teachio.BLL.CQRS.Users.Account.StreamAvatar;
using Teachio.BLL.CQRS.Users.Account.Update;
using Teachio.BLL.CQRS.Users.Account.UploadAvatar;
using Teachio.BLL.DTOs.Users.Account.Request.Delete;
using Teachio.BLL.DTOs.Users.Account.Request.Update;
using Teachio.BLL.DTOs.Users.Account.Request.Upload;
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
    /// Streams the current user's avatar.
    /// </summary>
    /// <returns>Returns the avatar stream.</returns>
    [HttpGet(UsersRelativeRoutes.StreamAvatar)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status206PartialContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> StreamAvatar()
    {
        var rangeHeader = Request.Headers.Range.ToString();
        var result = await Mediator.Send(new StreamAvatarQuery(rangeHeader));

        if (result.IsFailed)
        {
            return HandleResult(result);
        }

        var streamResult = result.Value;

        Response.Headers[HeaderNames.AcceptRanges] = "bytes";

        if (!string.IsNullOrWhiteSpace(streamResult.ContentRange))
        {
            Response.Headers[HeaderNames.ContentRange] = streamResult.ContentRange;
        }

        if (streamResult.ContentLength.HasValue)
        {
            Response.ContentLength = streamResult.ContentLength.Value;
        }

        Response.StatusCode = streamResult.IsPartialContent
            ? StatusCodes.Status206PartialContent
            : StatusCodes.Status200OK;

        return new FileStreamResult(streamResult.ContentStream, streamResult.ContentType);
    }

    /// <summary>
    /// Uploads an avatar for the current user.
    /// </summary>
    /// <param name="avatarUploadRequestDto">The data for uploading user avatar.</param>
    /// <returns>Returns the newly uploaded avatar metadata.</returns>
    [HttpPost(UsersRelativeRoutes.UploadAvatar)]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AvatarUploadResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UploadAvatar([FromForm] AvatarUploadRequestDto avatarUploadRequestDto)
    {
        return HandleResult(await Mediator.Send(new UploadAvatarCommand(avatarUploadRequestDto)));
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
