using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Teachio.BLL.Dto.Courses.Videos.Videos.Request.Create;
using Teachio.BLL.Dto.Courses.Videos.Videos.Response;
using Teachio.BLL.MediatR.Courses.Videos.Videos.Create;
using Teachio.BLL.MediatR.Courses.Videos.Videos.GetById;
using Teachio.WebApi.Utils.RelativeRoutes;

namespace Teachio.WebApi.Controllers.Courses.Videos.Videos;

public class VideosController : BaseApiController
{
    /// <summary>
    /// Retrieves a course video by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the course video to retrieve.</param>
    /// <returns>Returns the corresponding course video.</returns>
    [HttpGet(VideosRelativeRoutes.GetById)]
    // [Authorize(Roles = nameof(UserRole.ContentCreator))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VideoResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        // TODO: when authentication is implemented, use GetUserIdOrThrow() instead
        // var userId = GetUserIdOrThrow();
        var userId = GetUserId() ?? Guid.Parse("76bb9fd8-084c-4012-8c94-a2a04f45156f");

        return HandleResult(await Mediator.Send(new GetVideoByIdQuery(id, userId)));
    }

    /// <summary>
    /// Creates a new course video based on the provided data.
    /// </summary>
    /// <param name="videoCreateRequestDto">The data for the new course video.</param>
    /// <returns>Returns the newly created course video.</returns>
    [HttpPost(VideosRelativeRoutes.Create)]
    // [Authorize(Roles = nameof(UserRole.ContentCreator))]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(VideoResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] VideoCreateRequestDto videoCreateRequestDto)
    {
        // TODO: when authentication is implemented, use GetUserIdOrThrow() instead
        // var userId = GetUserIdOrThrow();
        var userId = GetUserId() ?? Guid.Parse("76bb9fd8-084c-4012-8c94-a2a04f45156f");

        return HandleResult(await Mediator.Send(new CreateVideoCommand(videoCreateRequestDto, userId)));
    }
}
