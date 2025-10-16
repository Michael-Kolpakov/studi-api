using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Sections;
using Teachio.BLL.Dto.Courses.Sections.Create;

namespace Teachio.BLL.MediatR.Courses.Sections.Create;

public record CreateSectionCommand(SectionCreateDto sectionCreateDto)
    : IRequest<Result<SectionResponseDto>>;
