using FluentResults;
using MediatR;
using Studi.BLL.DTOs.Courses.Sections.Response;

namespace Studi.BLL.CQRS.Courses.Sections.Delete;

public record DeleteSectionCommand(Guid SectionId)
    : IRequest<Result<SectionResponseDto>>;
