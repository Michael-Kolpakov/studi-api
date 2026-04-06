using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Courses.Request.Update;
using Teachio.BLL.Dto.Courses.Courses.Response;

namespace Teachio.BLL.MediatR.Courses.Courses.Update;

public record UpdateCourseCommand(CourseUpdateRequestDto CourseUpdateRequestDto, Guid RequestingUserId)
    : IRequest<Result<CourseResponseDto>>;
