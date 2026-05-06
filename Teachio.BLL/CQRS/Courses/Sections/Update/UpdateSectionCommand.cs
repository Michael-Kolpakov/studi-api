using FluentResults;
using MediatR;
using Teachio.BLL.DTOs.Courses.Sections.Request.Update;
using Teachio.BLL.DTOs.Courses.Sections.Response;

namespace Teachio.BLL.CQRS.Courses.Sections.Update;

public record UpdateSectionCommand(SectionUpdateRequestDto SectionUpdateRequestDto)
    : IRequest<Result<SectionResponseDto>>;
