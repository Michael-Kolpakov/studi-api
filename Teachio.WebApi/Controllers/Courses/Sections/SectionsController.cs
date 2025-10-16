using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Teachio.BLL.Dto.Courses.Sections;
using Teachio.BLL.Dto.Courses.Sections.Create;
using Teachio.BLL.Dto.Courses.Sections.Update;
using Teachio.BLL.MediatR.Courses.Sections.Create;
using Teachio.BLL.MediatR.Courses.Sections.Delete;
using Teachio.BLL.MediatR.Courses.Sections.GetById;
using Teachio.BLL.MediatR.Courses.Sections.Update;

namespace Teachio.WebApi.Controllers.Courses.Sections;

public class SectionsController : BaseApiController
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SectionResponseDto))]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        return HandleResult(await Mediator.Send(new GetSectionByIdQuery(id)));
    }

    [HttpPost]
    // [Authorize(Roles = nameof(UserRole.ContentCreator))]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SectionResponseDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] SectionCreateDto sectionCreateDto)
    {
        return HandleResult(await Mediator.Send(new CreateSectionCommand(sectionCreateDto)));
    }

    [HttpPut]
    // [Authorize(Roles = nameof(UserRole.ContentCreator))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SectionResponseDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update([FromBody] SectionUpdateDto sectionUpdateDto)
    {
        return HandleResult(await Mediator.Send(new UpdateSectionCommand(sectionUpdateDto)));
    }

    [HttpDelete("{id:guid}")]
    // [Authorize(Roles = nameof(UserRole.ContentCreator))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SectionResponseDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        return HandleResult(await Mediator.Send(new DeleteSectionCommand(id)));
    }
}
