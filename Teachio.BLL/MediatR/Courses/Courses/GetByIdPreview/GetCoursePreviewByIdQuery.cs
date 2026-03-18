using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Courses.Response;

namespace Teachio.BLL.MediatR.Courses.Courses.GetByIdPreview;

/// <summary>
/// Represents the <see cref="GetCoursePreviewByIdQuery"/> record model.
/// </summary>
/// <param name="CourseId">The identifier of <paramref name="CourseId"/>.</param>
public record GetCoursePreviewByIdQuery(Guid CourseId)
    : IRequest<Result<CoursePreviewResponseDto>>;
