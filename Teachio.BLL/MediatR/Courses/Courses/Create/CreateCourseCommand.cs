using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Courses.Request.Create;
using Teachio.BLL.Dto.Courses.Courses.Response;

namespace Teachio.BLL.MediatR.Courses.Courses.Create;

/// <summary>
/// Represents the <see cref="CreateCourseCommand"/> record model.
/// </summary>
/// <param name="CourseCreateRequestDto">The request payload in <paramref name="CourseCreateRequestDto"/>.</param>
/// <param name="OwnerUserId">The identifier of <paramref name="OwnerUserId"/>.</param>
public record CreateCourseCommand(CourseCreateRequestDto CourseCreateRequestDto, Guid OwnerUserId)
    : IRequest<Result<CourseResponseDto>>;
