using FluentResults;
using MediatR;
using Studi.BLL.DTOs.Courses.Sections.Request.Update;
using Studi.BLL.DTOs.Courses.Sections.Response;

namespace Studi.BLL.CQRS.Courses.Sections.Update;

public record UpdateSectionCommand(SectionUpdateRequestDto SectionUpdateRequestDto)
    : IRequest<Result<SectionResponseDto>>;
