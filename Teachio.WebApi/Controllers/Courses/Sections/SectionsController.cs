using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Teachio.BLL.Dto.Courses.Sections.Request.Create;
using Teachio.BLL.Dto.Courses.Sections.Request.Update;
using Teachio.BLL.Dto.Courses.Sections.Response;
using Teachio.BLL.MediatR.Courses.Sections.Create;
using Teachio.BLL.MediatR.Courses.Sections.Delete;
using Teachio.BLL.MediatR.Courses.Sections.GetById;
using Teachio.BLL.MediatR.Courses.Sections.Update;
using Teachio.WebApi.Utils.RelativeRoutes;

namespace Teachio.WebApi.Controllers.Courses.Sections;

public class SectionsController : BaseApiController
{
    /// <summary>
    /// Retrieves a course section by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the course section to retrieve.</param>
    /// <returns>Returns the corresponding course section.</returns>
    [HttpGet(SectionsRelativeRoutes.GetById)]
    // [Authorize(Roles = nameof(UserRole.ContentCreator))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SectionResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        // TODO: when authentication is implemented, use GetUserIdOrThrow() instead
        // var userId = GetUserIdOrThrow();
        var userId = GetUserId() ?? Guid.Parse("76bb9fd8-084c-4012-8c94-a2a04f45156f");

        return HandleResult(await Mediator.Send(new GetSectionByIdQuery(id, userId)));
    }

    /// <summary>
    /// Creates a new course section based on the provided data.
    /// </summary>
    /// <param name="sectionCreateRequestDto">The data for the new course section.</param>
    /// <returns>Returns the newly created course section.</returns>
    [HttpPost(SectionsRelativeRoutes.Create)]
    // [Authorize(Roles = nameof(UserRole.ContentCreator))]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SectionResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] SectionCreateRequestDto sectionCreateRequestDto)
    {
        // TODO: when authentication is implemented, use GetUserIdOrThrow() instead
        // var userId = GetUserIdOrThrow();
        var userId = GetUserId() ?? Guid.Parse("76bb9fd8-084c-4012-8c94-a2a04f45156f");

        return HandleResult(await Mediator.Send(new CreateSectionCommand(sectionCreateRequestDto, userId)));
    }

    /// <summary>
    /// Updates an existing course section with the provided data.
    /// </summary>
    /// <param name="sectionUpdateRequestDto">The updated data for the course section.</param>
    /// <returns>Returns the newly updated course section.</returns>
    [HttpPut(SectionsRelativeRoutes.Update)]
    // [Authorize(Roles = nameof(UserRole.ContentCreator))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SectionResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update([FromBody] SectionUpdateRequestDto sectionUpdateRequestDto)
    {
        // TODO: when authentication is implemented, use GetUserIdOrThrow() instead
        // var userId = GetUserIdOrThrow();
        var userId = GetUserId() ?? Guid.Parse("76bb9fd8-084c-4012-8c94-a2a04f45156f");

        return HandleResult(await Mediator.Send(new UpdateSectionCommand(sectionUpdateRequestDto, userId)));
    }

    /// <summary>
    /// Deletes an existing course section by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the course section to delete.</param>
    /// <returns>Returns the newly deleted course section.</returns>
    [HttpDelete(SectionsRelativeRoutes.Delete)]
    // [Authorize(Roles = nameof(UserRole.ContentCreator))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SectionResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        // TODO: when authentication is implemented, use GetUserIdOrThrow() instead
        // var userId = GetUserIdOrThrow();
        var userId = GetUserId() ?? Guid.Parse("76bb9fd8-084c-4012-8c94-a2a04f45156f");

        return HandleResult(await Mediator.Send(new DeleteSectionCommand(id, userId)));
    }
}
