using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Sections;

namespace Teachio.BLL.MediatR.Courses.Sections.GetById;

public record GetSectionByIdQuery(Guid id)
    : IRequest<Result<SectionResponseDto>>;
