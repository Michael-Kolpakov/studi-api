using FluentResults;
using MediatR;
using Studi.BLL.DTOs.Courses.Courses.Response;

namespace Studi.BLL.CQRS.Courses.Courses.GetById;

public record GetCourseByIdQuery(Guid CourseId, Guid? SelectedVideoId = null)
    : IRequest<Result<CourseResponseDto>>;
