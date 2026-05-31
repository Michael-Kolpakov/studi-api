using FluentResults;
using MediatR;
using Studi.BLL.DTOs.Courses.Courses.Request.Enroll;
using Studi.BLL.DTOs.Courses.Courses.Response;

namespace Studi.BLL.CQRS.Courses.Courses.Enroll;

public record EnrollCourseCommand(CourseEnrollRequestDto CourseEnrollRequestDto)
    : IRequest<Result<CourseEnrollResponseDto>>;
