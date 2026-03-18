using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Courses.Response;

namespace Teachio.BLL.MediatR.Courses.Courses.Delete;

/// <summary>
/// Represents the <see cref="DeleteCourseCommand"/> record model.
/// </summary>
/// <param name="CourseId">The identifier of <paramref name="CourseId"/>.</param>
/// <param name="RequestingUserId">The identifier of <paramref name="RequestingUserId"/>.</param>
public record DeleteCourseCommand(Guid CourseId, Guid RequestingUserId)
    : IRequest<Result<CourseResponseDto>>;
