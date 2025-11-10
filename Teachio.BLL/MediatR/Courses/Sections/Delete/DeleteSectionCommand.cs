using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Sections;
using Teachio.BLL.Dto.Courses.Sections.Response;

namespace Teachio.BLL.MediatR.Courses.Sections.Delete;

public record DeleteSectionCommand(Guid id)
    : IRequest<Result<SectionResponseDto>>;
