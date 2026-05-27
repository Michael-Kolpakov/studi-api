using FluentResults;
using MediatR;
using Studi.BLL.DTOs.Courses.Courses.Response;

namespace Studi.BLL.CQRS.Courses.Courses.Delete;

public record DeleteCourseCommand(Guid CourseId)
    : IRequest<Result<CourseResponseDto>>;
