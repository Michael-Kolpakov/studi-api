using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Sections.Request.Create;
using Teachio.BLL.Dto.Courses.Sections.Response;

namespace Teachio.BLL.MediatR.Courses.Sections.Create;

/// <summary>
/// Represents the <see cref="CreateSectionCommand"/> record model.
/// </summary>
/// <param name="SectionCreateRequestDto">The request payload in <paramref name="SectionCreateRequestDto"/>.</param>
/// <param name="RequestingUserId">The identifier of <paramref name="RequestingUserId"/>.</param>
public record CreateSectionCommand(SectionCreateRequestDto SectionCreateRequestDto, Guid RequestingUserId)
    : IRequest<Result<SectionResponseDto>>;
