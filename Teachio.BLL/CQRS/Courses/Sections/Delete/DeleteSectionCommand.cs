using FluentResults;
using MediatR;
using Teachio.BLL.DTOs.Courses.Sections.Response;

namespace Teachio.BLL.CQRS.Courses.Sections.Delete;

public record DeleteSectionCommand(Guid SectionId)
    : IRequest<Result<SectionResponseDto>>;
