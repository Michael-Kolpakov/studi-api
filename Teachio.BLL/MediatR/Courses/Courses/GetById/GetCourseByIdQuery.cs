using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Courses.Response;

namespace Teachio.BLL.MediatR.Courses.Courses.GetById;

public record GetCourseByIdQuery(Guid CourseId, Guid RequestingUserId, Guid? SelectedVideoId = null)
    : IRequest<Result<CourseResponseDto>>;
