using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Sections;
using Teachio.BLL.Dto.Courses.Sections.Update;

namespace Teachio.BLL.MediatR.Courses.Sections.Update;

public record UpdateSectionCommand(SectionUpdateDto sectionUpdateDto)
    : IRequest<Result<SectionResponseDto>>;
