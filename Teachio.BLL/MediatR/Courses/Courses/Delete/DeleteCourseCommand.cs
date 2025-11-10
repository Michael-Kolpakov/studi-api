using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Courses.Response;

namespace Teachio.BLL.MediatR.Courses.Courses.Delete;

public record DeleteCourseCommand(Guid id)
    : IRequest<Result<CourseResponseDto>>;
