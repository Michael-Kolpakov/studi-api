using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Teachio.BLL.CQRS.Courses.Courses.Create;
using Teachio.BLL.CQRS.Courses.Courses.Delete;
using Teachio.BLL.CQRS.Courses.Courses.GetById;
using Teachio.BLL.CQRS.Courses.Courses.GetByIdPreview;
using Teachio.BLL.CQRS.Courses.Courses.GetPaginated;
using Teachio.BLL.CQRS.Courses.Courses.StreamThumbnail;
using Teachio.BLL.CQRS.Courses.Courses.Update;
using Teachio.BLL.CQRS.Courses.Courses.UploadThumbnail;
using Teachio.BLL.DTOs.Courses.Courses.Request.Create;
using Teachio.BLL.DTOs.Courses.Courses.Request.Update;
using Teachio.BLL.DTOs.Courses.Courses.Request.Upload;
using Teachio.BLL.DTOs.Courses.Courses.Response;
using Teachio.WebApi.Utils.RelativeRoutes;

namespace Teachio.WebApi.Controllers.Courses.Courses;

public class CoursesController : BaseApiController
{
    /// <summary>
    /// Retrieves a paginated list of courses based on the provided page number and page size.
    /// </summary>
    /// <param name="pageNumber">The number of the page need to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>Returns a paginated list of the courses.</returns>
    [HttpGet(CoursesRelativeRoutes.GetPaginated)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedCoursesResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPaginated([FromQuery] ushort pageNumber, [FromQuery] ushort pageSize)
    {
        return HandleResult(await Mediator.Send(new GetPaginatedCoursesQuery(pageNumber, pageSize)));
    }

    /// <summary>
    /// Retrieves a preview of a course by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the course to retrieve.</param>
    /// <returns>Returns a preview of the corresponding course.</returns>
    [HttpGet(CoursesRelativeRoutes.GetByIdPreview)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CoursePreviewResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdPreview([FromRoute] Guid id)
    {
        return HandleResult(await Mediator.Send(new GetCoursePreviewByIdQuery(id)));
    }

    /// <summary>
    /// Retrieves a course by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the course to retrieve.</param>
    /// <param name="selectedVideoId">The unique identifier of the selected video to load.</param>
    /// <returns>Returns the corresponding course.</returns>
    [HttpGet(CoursesRelativeRoutes.GetById)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, [FromQuery] Guid? selectedVideoId = null)
    {
        return HandleResult(await Mediator.Send(new GetCourseByIdQuery(id, selectedVideoId)));
    }

    /// <summary>
    /// Streams a course thumbnail by course unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the course to get thumbnail for.</param>
    /// <returns>Returns the requested thumbnail stream.</returns>
    [HttpGet(CoursesRelativeRoutes.StreamThumbnail)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status206PartialContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> StreamThumbnail([FromRoute] Guid id)
    {
        var rangeHeader = Request.Headers.Range.ToString();
        var result = await Mediator.Send(new StreamCourseThumbnailQuery(id, rangeHeader));

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
    /// Creates a new course based on the provided data.
    /// </summary>
    /// <param name="courseCreateRequestDto">The data for the new course.</param>
    /// <returns>Returns the newly created course.</returns>
    [HttpPost(CoursesRelativeRoutes.Create)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CourseResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CourseCreateRequestDto courseCreateRequestDto)
    {
        return HandleResult(await Mediator.Send(new CreateCourseCommand(courseCreateRequestDto)));
    }

    /// <summary>
    /// Uploads a thumbnail for an existing course.
    /// </summary>
    /// <param name="thumbnailUploadRequestDto">The data for uploading course thumbnail.</param>
    /// <returns>Returns the newly uploaded thumbnail unique identifier.</returns>
    [HttpPost(CoursesRelativeRoutes.UploadThumbnail)]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ThumbnailUploadResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadThumbnail([FromForm] ThumbnailUploadRequestDto thumbnailUploadRequestDto)
    {
        return HandleResult(await Mediator.Send(new UploadThumbnailCommand(thumbnailUploadRequestDto)));
    }

    /// <summary>
    /// Updates an existing course with the provided data.
    /// </summary>
    /// <param name="courseUpdateRequestDto">The updated data for the course.</param>
    /// <returns>Returns the newly updated course.</returns>
    [HttpPut(CoursesRelativeRoutes.Update)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update([FromBody] CourseUpdateRequestDto courseUpdateRequestDto)
    {
        return HandleResult(await Mediator.Send(new UpdateCourseCommand(courseUpdateRequestDto)));
    }

    /// <summary>
    /// Deletes an existing course by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the course to delete.</param>
    /// <returns>Returns the newly deleted course.</returns>
    [HttpDelete(CoursesRelativeRoutes.Delete)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        return HandleResult(await Mediator.Send(new DeleteCourseCommand(id)));
    }
}
