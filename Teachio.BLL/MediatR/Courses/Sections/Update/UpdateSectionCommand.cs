using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Sections;
using Teachio.BLL.Dto.Courses.Sections.Request.Update;
using Teachio.BLL.Dto.Courses.Sections.Response;

namespace Teachio.BLL.MediatR.Courses.Sections.Update;

public record UpdateSectionCommand(SectionUpdateRequestDto SectionUpdateRequestDto)
    : IRequest<Result<SectionResponseDto>>;
