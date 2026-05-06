using FluentResults;
using MediatR;
using Teachio.BLL.DTOs.Courses.Courses.Request.Update;
using Teachio.BLL.DTOs.Courses.Courses.Response;

namespace Teachio.BLL.CQRS.Courses.Courses.Update;

public record UpdateCourseCommand(CourseUpdateRequestDto CourseUpdateRequestDto)
    : IRequest<Result<CourseResponseDto>>;
