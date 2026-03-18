using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Courses.Request.Update;
using Teachio.BLL.Dto.Courses.Courses.Response;

namespace Teachio.BLL.MediatR.Courses.Courses.Update;

/// <summary>
/// Represents the <see cref="UpdateCourseCommand"/> record model.
/// </summary>
/// <param name="CourseUpdateRequestDto">The request payload in <paramref name="CourseUpdateRequestDto"/>.</param>
/// <param name="RequestingUserId">The identifier of <paramref name="RequestingUserId"/>.</param>
public record UpdateCourseCommand(CourseUpdateRequestDto CourseUpdateRequestDto, Guid RequestingUserId)
    : IRequest<Result<CourseResponseDto>>;
