using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Sections.Request.Create;
using Teachio.BLL.Dto.Courses.Sections.Response;

namespace Teachio.BLL.MediatR.Courses.Sections.Create;

public record CreateSectionCommand(SectionCreateRequestDto sectionCreateRequestDto, Guid requestingUserId)
    : IRequest<Result<SectionResponseDto>>;
