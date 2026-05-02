using FluentResults;
using MediatR;
using Teachio.BLL.DTOs.Courses.Sections.Response;

namespace Teachio.BLL.CQRS.Courses.Sections.GetById;

public record GetSectionByIdQuery(Guid SectionId)
    : IRequest<Result<SectionResponseDto>>;
