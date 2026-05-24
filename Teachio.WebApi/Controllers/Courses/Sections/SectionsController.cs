using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Teachio.BLL.CQRS.Courses.Sections.Create;
using Teachio.BLL.CQRS.Courses.Sections.Delete;
using Teachio.BLL.CQRS.Courses.Sections.GetByCourseId;
using Teachio.BLL.CQRS.Courses.Sections.GetById;
using Teachio.BLL.CQRS.Courses.Sections.Update;
using Teachio.BLL.CQRS.Courses.Sections.UpdateOrderIndex;
using Teachio.BLL.DTOs.Courses.Sections.Request.Create;
using Teachio.BLL.DTOs.Courses.Sections.Request.Update;
using Teachio.BLL.DTOs.Courses.Sections.Response;
using Teachio.WebApi.Utils.RelativeRoutes;

namespace Teachio.WebApi.Controllers.Courses.Sections;

[Authorize]
public class SectionsController : BaseApiController
{
    /// <summary>
    /// Retrieves a course section by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the course section to retrieve.</param>
    /// <returns>Returns the corresponding course section.</returns>
    [HttpGet(SectionsRelativeRoutes.GetById)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SectionResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        return HandleResult(await Mediator.Send(new GetSectionByIdQuery(id)));
    }

    /// <summary>
    /// Retrieves all course sections for the specified course.
    /// </summary>
    /// <param name="courseId">The unique identifier of the course.</param>
    /// <returns>Returns the course sections ordered by their position.</returns>
    [HttpGet(SectionsRelativeRoutes.GetByCourseId)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseSectionsResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCourseId([FromRoute] Guid courseId)
    {
        return HandleResult(await Mediator.Send(new GetSectionsByCourseIdQuery(courseId)));
    }

    /// <summary>
    /// Creates a new course section based on the provided data.
    /// </summary>
    /// <param name="sectionCreateRequestDto">The data for the new course section.</param>
    /// <returns>Returns the newly created course section.</returns>
    [HttpPost(SectionsRelativeRoutes.Create)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SectionResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] SectionCreateRequestDto sectionCreateRequestDto)
    {
        return HandleResult(await Mediator.Send(new CreateSectionCommand(sectionCreateRequestDto)));
    }

    /// <summary>
    /// Updates an existing course section with the provided data.
    /// </summary>
    /// <param name="sectionUpdateRequestDto">The updated data for the course section.</param>
    /// <returns>Returns the newly updated course section.</returns>
    [HttpPut(SectionsRelativeRoutes.Update)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SectionResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update([FromBody] SectionUpdateRequestDto sectionUpdateRequestDto)
    {
        return HandleResult(await Mediator.Send(new UpdateSectionCommand(sectionUpdateRequestDto)));
    }

    /// <summary>
    /// Updates the order index of an existing course section.
    /// </summary>
    /// <param name="sectionUpdateOrderIndexRequestDto">The data for updating the section order index.</param>
    /// <returns>Returns the newly updated course section.</returns>
    [HttpPut(SectionsRelativeRoutes.UpdateOrderIndex)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SectionResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateOrderIndex([FromBody] SectionUpdateOrderIndexRequestDto sectionUpdateOrderIndexRequestDto)
    {
        return HandleResult(await Mediator.Send(new UpdateSectionOrderIndexCommand(sectionUpdateOrderIndexRequestDto)));
    }

    /// <summary>
    /// Deletes an existing course section by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the course section to delete.</param>
    /// <returns>Returns the newly deleted course section.</returns>
    [HttpDelete(SectionsRelativeRoutes.Delete)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SectionResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        return HandleResult(await Mediator.Send(new DeleteSectionCommand(id)));
    }
}
