using FluentResults;
using MediatR;
using Teachio.BLL.DTOs.Courses.Sections.Request.Create;
using Teachio.BLL.DTOs.Courses.Sections.Response;

namespace Teachio.BLL.CQRS.Courses.Sections.Create;

public record CreateSectionCommand(SectionCreateRequestDto SectionCreateRequestDto)
    : IRequest<Result<SectionResponseDto>>;
