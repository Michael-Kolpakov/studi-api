using FluentResults;
using MediatR;
using Teachio.BLL.DTOs.Courses.Courses.Response;

namespace Teachio.BLL.CQRS.Courses.Courses.GetByIdShort;

public record GetCourseShortByIdQuery(Guid CourseId)
    : IRequest<Result<CourseEditShortResponseDto>>;
