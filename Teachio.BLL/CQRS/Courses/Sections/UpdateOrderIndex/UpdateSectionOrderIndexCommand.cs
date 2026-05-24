using FluentResults;
using MediatR;
using Teachio.BLL.DTOs.Courses.Sections.Request.Update;
using Teachio.BLL.DTOs.Courses.Sections.Response;

namespace Teachio.BLL.CQRS.Courses.Sections.UpdateOrderIndex;

public record UpdateSectionOrderIndexCommand(SectionUpdateOrderIndexRequestDto SectionUpdateOrderIndexRequestDto)
    : IRequest<Result<SectionResponseDto>>;
