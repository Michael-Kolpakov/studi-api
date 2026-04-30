using Microsoft.AspNetCore.Mvc;
using Teachio.BLL.Dto.Courses.Videos.Videos.Request.Create;
using Teachio.BLL.Dto.Courses.Videos.Videos.Request.Update;
using Teachio.BLL.Dto.Courses.Videos.Videos.Request.Upload;
using Teachio.BLL.Dto.Courses.Videos.Videos.Response;
using Teachio.BLL.MediatR.Courses.Videos.Videos.Create;
using Teachio.BLL.MediatR.Courses.Videos.Videos.Delete;
using Teachio.BLL.MediatR.Courses.Videos.Videos.GetById;
using Teachio.BLL.MediatR.Courses.Videos.Videos.Update;
using Teachio.BLL.MediatR.Courses.Videos.Videos.UploadVideo;
using Teachio.DAL.Utils.Constants;
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VideoResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        return HandleResult(await Mediator.Send(new GetVideoByIdQuery(id)));
    }

    /// <summary>
    /// Creates a new course video based on the provided data.
    /// </summary>
    /// <param name="videoCreateRequestDto">The data for the new course video.</param>
    /// <returns>Returns the newly created course video.</returns>
    [HttpPost(VideosRelativeRoutes.Create)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(VideoResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] VideoCreateRequestDto videoCreateRequestDto)
    {
        return HandleResult(await Mediator.Send(new CreateVideoCommand(videoCreateRequestDto)));
    }

    /// <summary>
    /// Uploads a media file for an existing course video.
    /// </summary>
    /// <param name="videoUploadRequestDto">The data for uploading video file and linking it to a course video.</param>
    /// <returns>Returns the updated course video with uploaded file metadata.</returns>
    [HttpPost(VideosRelativeRoutes.Upload)]
    [Consumes("multipart/form-data")]
    [RequestFormLimits(MultipartBodyLengthLimit = EntityConstants.MaxVideoFileSizeBytes)]
    [RequestSizeLimit(EntityConstants.MaxVideoFileSizeBytes)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VideoUploadResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Upload([FromForm] VideoUploadRequestDto videoUploadRequestDto)
    {
        return HandleResult(await Mediator.Send(new UploadVideoCommand(videoUploadRequestDto)));
    }

    /// <summary>
    /// Updates an existing course video with the provided data.
    /// </summary>
    /// <param name="videoUpdateRequestDto">The updated data for the course video.</param>
    /// <returns>Returns the newly updated course video.</returns>
    [HttpPut(VideosRelativeRoutes.Update)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VideoResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update([FromBody] VideoUpdateRequestDto videoUpdateRequestDto)
    {
        return HandleResult(await Mediator.Send(new UpdateVideoCommand(videoUpdateRequestDto)));
    }

    /// <summary>
    /// Deletes an existing course video by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the course video to delete.</param>
    /// <returns>Returns the newly deleted course video.</returns>
    [HttpDelete(VideosRelativeRoutes.Delete)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VideoResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        return HandleResult(await Mediator.Send(new DeleteVideoCommand(id)));
    }
}
