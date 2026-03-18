using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Sections.Request.Update;
using Teachio.BLL.Dto.Courses.Sections.Response;

namespace Teachio.BLL.MediatR.Courses.Sections.Update;

/// <summary>
/// Represents the <see cref="UpdateSectionCommand"/> record model.
/// </summary>
/// <param name="SectionUpdateRequestDto">The request payload in <paramref name="SectionUpdateRequestDto"/>.</param>
/// <param name="RequestingUserId">The identifier of <paramref name="RequestingUserId"/>.</param>
public record UpdateSectionCommand(SectionUpdateRequestDto SectionUpdateRequestDto, Guid RequestingUserId)
    : IRequest<Result<SectionResponseDto>>;
