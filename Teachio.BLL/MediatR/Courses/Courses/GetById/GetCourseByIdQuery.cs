using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Courses.Response;

namespace Teachio.BLL.MediatR.Courses.Courses.GetById;

/// <summary>
/// Represents the <see cref="GetCourseByIdQuery"/> record model.
/// </summary>
/// <param name="CourseId">The identifier of <paramref name="CourseId"/>.</param>
/// <param name="RequestingUserId">The identifier of <paramref name="RequestingUserId"/>.</param>
/// <param name="SelectedVideoId">The identifier of <paramref name="SelectedVideoId"/>.</param>
public record GetCourseByIdQuery(Guid CourseId, Guid RequestingUserId, Guid? SelectedVideoId = null)
    : IRequest<Result<CourseResponseDto>>;
