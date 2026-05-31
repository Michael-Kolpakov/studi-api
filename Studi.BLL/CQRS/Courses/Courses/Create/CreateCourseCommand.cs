using FluentResults;
using MediatR;
using Studi.BLL.DTOs.Courses.Courses.Request.Create;
using Studi.BLL.DTOs.Courses.Courses.Response;

namespace Studi.BLL.CQRS.Courses.Courses.Create;

public record CreateCourseCommand(CourseCreateRequestDto CourseCreateRequestDto)
    : IRequest<Result<CourseResponseDto>>;
