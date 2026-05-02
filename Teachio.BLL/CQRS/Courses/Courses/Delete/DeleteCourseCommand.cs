using FluentResults;
using MediatR;
using Teachio.BLL.DTOs.Courses.Courses.Response;

namespace Teachio.BLL.CQRS.Courses.Courses.Delete;

public record DeleteCourseCommand(Guid CourseId)
    : IRequest<Result<CourseResponseDto>>;
