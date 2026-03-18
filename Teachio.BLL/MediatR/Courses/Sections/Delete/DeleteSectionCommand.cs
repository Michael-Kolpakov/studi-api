using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Sections.Response;

namespace Teachio.BLL.MediatR.Courses.Sections.Delete;

/// <summary>
/// Represents the <see cref="DeleteSectionCommand"/> record model.
/// </summary>
/// <param name="SectionId">The identifier of <paramref name="SectionId"/>.</param>
/// <param name="RequestingUserId">The identifier of <paramref name="RequestingUserId"/>.</param>
public record DeleteSectionCommand(Guid SectionId, Guid RequestingUserId)
    : IRequest<Result<SectionResponseDto>>;
