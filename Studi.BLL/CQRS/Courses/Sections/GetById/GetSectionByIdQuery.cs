using FluentResults;
using MediatR;
using Studi.BLL.DTOs.Courses.Sections.Response;

namespace Studi.BLL.CQRS.Courses.Sections.GetById;

public record GetSectionByIdQuery(Guid SectionId)
    : IRequest<Result<SectionResponseDto>>;
