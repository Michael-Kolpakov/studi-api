using FluentResults;
using MediatR;
using Teachio.BLL.DTOs.Courses.Courses.Request.Create;
using Teachio.BLL.DTOs.Courses.Courses.Response;

namespace Teachio.BLL.CQRS.Courses.Courses.Create;

public record CreateCourseCommand(CourseCreateRequestDto CourseCreateRequestDto)
    : IRequest<Result<CourseResponseDto>>;
