using FluentResults;
using MediatR;
using Teachio.BLL.DTOs.Courses.Courses.Request.Enroll;
using Teachio.BLL.DTOs.Courses.Courses.Response;

namespace Teachio.BLL.CQRS.Courses.Courses.Enroll;

public record EnrollCourseCommand(CourseEnrollRequestDto CourseEnrollRequestDto)
    : IRequest<Result<CourseEnrollResponseDto>>;
