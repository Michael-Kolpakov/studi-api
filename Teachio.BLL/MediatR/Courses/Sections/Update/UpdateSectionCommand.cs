using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Sections.Request.Update;
using Teachio.BLL.Dto.Courses.Sections.Response;

namespace Teachio.BLL.MediatR.Courses.Sections.Update;

public record UpdateSectionCommand(SectionUpdateRequestDto sectionUpdateRequestDto, Guid requestingUserId)
    : IRequest<Result<SectionResponseDto>>;
