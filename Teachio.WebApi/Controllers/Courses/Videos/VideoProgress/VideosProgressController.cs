using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Teachio.BLL.Dto.Courses.Videos.VideoProgress.Request.Update;
using Teachio.BLL.Dto.Courses.Videos.VideoProgress.Response;
using Teachio.BLL.MediatR.Courses.Videos.VideoProgress.Update;
using Teachio.WebApi.Utils.RelativeRoutes;

namespace Teachio.WebApi.Controllers.Courses.Videos.VideoProgress;

public class VideosProgressController : BaseApiController
{
    /// <summary>
    /// Updates an existing course video with the provided data.
    /// </summary>
    /// <param name="videoProgressUpdateRequestDto">The updated data for the course video.</param>
    /// <returns>Returns the newly updated course video.</returns>
    [HttpPut(VideosProgressRelativeRoutes.Update)]
    // [Authorize(Roles = nameof(UserRole.ContentCreator))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VideoProgressResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update([FromBody] VideoProgressUpdateRequestDto videoProgressUpdateRequestDto)
    {
        // TODO: when authentication is implemented, use GetUserIdOrThrow() instead
        // var userId = GetUserIdOrThrow();
        var userId = GetUserId() ?? Guid.Parse("76bb9fd8-084c-4012-8c94-a2a04f45156f");

        return HandleResult(await Mediator.Send(new UpdateVideoProgressCommand(videoProgressUpdateRequestDto, userId)));
    }
}
