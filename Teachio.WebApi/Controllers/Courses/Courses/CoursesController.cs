using Microsoft.AspNetCore.Authorization;
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
using Teachio.WebApi.RelativeRoutes;

namespace Teachio.WebApi.Controllers.Courses.Courses;

public class CoursesController : BaseApiController
{
    [HttpGet(CoursesRelativeRoutes.GetPaginated)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetPaginatedCoursesResponseDto))]
    public async Task<IActionResult> GetPaginated([FromQuery] ushort pageNumber, [FromQuery] ushort pageSize)
    {
        return HandleResult(await Mediator.Send(new GetPaginatedCoursesQuery(pageNumber, pageSize)));
    }

    [HttpGet(CoursesRelativeRoutes.GetById)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseResponseDto))]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        return HandleResult(await Mediator.Send(new GetCourseByIdQuery(id)));
    }

    [HttpGet(CoursesRelativeRoutes.GetByIdPreview)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CoursePreviewResponseDto))]
    public async Task<IActionResult> GetByIdPreview([FromRoute] Guid id)
    {
        return HandleResult(await Mediator.Send(new GetCoursePreviewByIdQuery(id)));
    }

    [HttpPost(CoursesRelativeRoutes.Create)]
    // [Authorize(Roles = nameof(UserRole.ContentCreator))]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CourseResponseDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CourseCreateRequestDto courseCreateRequestDto)
    {
        return HandleResult(await Mediator.Send(new CreateCourseCommand(courseCreateRequestDto)));
    }

    [HttpPut(CoursesRelativeRoutes.Update)]
    // [Authorize(Roles = nameof(UserRole.ContentCreator))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseResponseDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update([FromBody] CourseUpdateRequestDto courseUpdateRequestDto)
    {
        return HandleResult(await Mediator.Send(new UpdateCourseCommand(courseUpdateRequestDto)));
    }

    [HttpDelete(CoursesRelativeRoutes.Delete)]
    // [Authorize(Roles = nameof(UserRole.ContentCreator))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseResponseDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        return HandleResult(await Mediator.Send(new DeleteCourseCommand(id)));
    }
}
