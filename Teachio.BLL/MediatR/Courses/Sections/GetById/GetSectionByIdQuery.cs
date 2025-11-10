using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Sections;
using Teachio.BLL.Dto.Courses.Sections.Response;

namespace Teachio.BLL.MediatR.Courses.Sections.GetById;

public record GetSectionByIdQuery(Guid id)
    : IRequest<Result<SectionResponseDto>>;
