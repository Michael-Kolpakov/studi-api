using FluentResults;
using MediatR;
using Studi.BLL.DTOs.Courses.Courses.Request.Update;
using Studi.BLL.DTOs.Courses.Courses.Response;

namespace Studi.BLL.CQRS.Courses.Courses.Update;

public record UpdateCourseCommand(CourseUpdateRequestDto CourseUpdateRequestDto)
    : IRequest<Result<CourseResponseDto>>;
