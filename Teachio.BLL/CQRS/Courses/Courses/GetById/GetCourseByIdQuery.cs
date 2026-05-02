using FluentResults;
using MediatR;
using Teachio.BLL.DTOs.Courses.Courses.Response;

namespace Teachio.BLL.CQRS.Courses.Courses.GetById;

public record GetCourseByIdQuery(Guid CourseId, Guid? SelectedVideoId = null)
    : IRequest<Result<CourseResponseDto>>;
