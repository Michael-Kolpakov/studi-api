using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Courses.Request.Create;
using Teachio.BLL.Dto.Courses.Courses.Response;

namespace Teachio.BLL.MediatR.Courses.Courses.Create;

public record CreateCourseCommand(CourseCreateRequestDto courseCreateRequestDto, Guid ownerUserId)
    : IRequest<Result<CourseResponseDto>>;
