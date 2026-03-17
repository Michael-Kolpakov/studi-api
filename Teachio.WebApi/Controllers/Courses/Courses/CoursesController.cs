using Microsoft.AspNetCore.Mvc;
using Teachio.BLL.Dto.Courses.Courses.Request.Create;
using Teachio.BLL.Dto.Courses.Courses.Request.Update;
using Teachio.BLL.Dto.Courses.Courses.Response;
using Teachio.BLL.MediatR.Courses.Courses.Create;
using Teachio.BLL.MediatR.Courses.Courses.Delete;
using Teachio.BLL.MediatR.Courses.Courses.GetById;
using Teachio.BLL.MediatR.Courses.Courses.GetByIdPreview;
using Teachio.BLL.MediatR.Courses.Courses.GetPaginated;
using Teachio.BLL.MediatR.Courses.Courses.Update;
using Teachio.BLL.MediatR.Courses.Courses.UploadThumbnail;
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
        // TODO: when authentication is implemented, remove ?? fallback value
        var userId = GetUserId() ?? Guid.Parse("76bb9fd8-084c-4012-8c94-a2a04f45156f");

        return HandleResult(await Mediator.Send(new GetPaginatedCoursesQuery(pageNumber, pageSize, userId)));
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
        // TODO: when authentication is implemented, use GetUserIdOrThrow() instead
        // var userId = GetUserIdOrThrow();
        var userId = GetUserId() ?? Guid.Parse("76bb9fd8-084c-4012-8c94-a2a04f45156f");

        return HandleResult(await Mediator.Send(new GetCourseByIdQuery(id, userId, selectedVideoId)));
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
        // TODO: when authentication is implemented, use GetUserIdOrThrow() instead
        // var userId = GetUserIdOrThrow();
        var userId = GetUserId() ?? Guid.Parse("76bb9fd8-084c-4012-8c94-a2a04f45156f");

        return HandleResult(await Mediator.Send(new CreateCourseCommand(courseCreateRequestDto, userId)));
    }

    /// <summary>
    /// Uploads a thumbnail for a course.
    /// </summary>
    /// <param name="thumbnailUploadRequestDto">The data for uploading course thumbnail.</param>
    /// <returns>Returns the newly uploaded thumbnail unique identifier.</returns>
    [HttpPost(CoursesRelativeRoutes.UploadThumbnail)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ThumbnailUploadResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
        // TODO: when authentication is implemented, use GetUserIdOrThrow() instead
        // var userId = GetUserIdOrThrow();
        var userId = GetUserId() ?? Guid.Parse("76bb9fd8-084c-4012-8c94-a2a04f45156f");

        return HandleResult(await Mediator.Send(new UpdateCourseCommand(courseUpdateRequestDto, userId)));
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
        // TODO: when authentication is implemented, use GetUserIdOrThrow() instead
        // var userId = GetUserIdOrThrow();
        var userId = GetUserId() ?? Guid.Parse("76bb9fd8-084c-4012-8c94-a2a04f45156f");

        return HandleResult(await Mediator.Send(new DeleteCourseCommand(id, userId)));
    }
}
