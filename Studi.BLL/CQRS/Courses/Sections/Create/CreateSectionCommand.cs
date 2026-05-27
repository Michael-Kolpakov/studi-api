using FluentResults;
using MediatR;
using Studi.BLL.DTOs.Courses.Sections.Request.Create;
using Studi.BLL.DTOs.Courses.Sections.Response;

namespace Studi.BLL.CQRS.Courses.Sections.Create;

public record CreateSectionCommand(SectionCreateRequestDto SectionCreateRequestDto)
    : IRequest<Result<SectionResponseDto>>;
