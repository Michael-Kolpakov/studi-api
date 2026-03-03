using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Sections.Response;

namespace Teachio.BLL.MediatR.Courses.Sections.Delete;

public record DeleteSectionCommand(Guid sectionId, Guid requestingUserId)
    : IRequest<Result<SectionResponseDto>>;
