using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Sections;
using Teachio.BLL.Dto.Courses.Sections.Request.Create;
using Teachio.BLL.Dto.Courses.Sections.Response;

namespace Teachio.BLL.MediatR.Courses.Sections.Create;

public record CreateSectionCommand(SectionCreateRequestDto SectionCreateRequestDto)
    : IRequest<Result<SectionResponseDto>>;
