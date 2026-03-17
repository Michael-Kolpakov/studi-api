using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Sections.Response;

namespace Teachio.BLL.MediatR.Courses.Sections.GetById;

public record GetSectionByIdQuery(Guid SectionId, Guid RequestingUserId)
    : IRequest<Result<SectionResponseDto>>;
