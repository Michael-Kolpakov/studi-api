using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Studi.BLL.CQRS.Courses.Videos.VideoProgress.Update;
using Studi.BLL.DTOs.Courses.Videos.VideoProgress.Request.Update;
using Studi.BLL.DTOs.Courses.Videos.VideoProgress.Response;
using Studi.WebApi.Utils.RelativeRoutes;

namespace Studi.WebApi.Controllers.Courses.Videos.VideoProgress;

[Authorize]
public class VideoProgressController : BaseApiController
{
    /// <summary>
    /// Updates an existing course video with the provided data.
    /// </summary>
    /// <param name="videoProgressUpdateRequestDto">The updated data for the course video.</param>
    /// <returns>Returns the newly updated course video.</returns>
    [HttpPut(VideoProgressRelativeRoutes.Update)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VideoProgressResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update([FromBody] VideoProgressUpdateRequestDto videoProgressUpdateRequestDto)
    {
        return HandleResult(await Mediator.Send(new UpdateVideoProgressCommand(videoProgressUpdateRequestDto)));
    }
}
