using FluentResults;
using MediatR;
using Studi.BLL.DTOs.Courses.Courses.Response;

namespace Studi.BLL.CQRS.Courses.Courses.GetByIdShort;

public record GetCourseShortByIdQuery(Guid CourseId)
    : IRequest<Result<CourseEditShortResponseDto>>;
