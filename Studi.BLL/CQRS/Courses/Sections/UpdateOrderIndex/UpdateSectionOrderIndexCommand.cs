using FluentResults;
using MediatR;
using Studi.BLL.DTOs.Courses.Sections.Request.Update;
using Studi.BLL.DTOs.Courses.Sections.Response;

namespace Studi.BLL.CQRS.Courses.Sections.UpdateOrderIndex;

public record UpdateSectionOrderIndexCommand(SectionUpdateOrderIndexRequestDto SectionUpdateOrderIndexRequestDto)
    : IRequest<Result<SectionResponseDto>>;
