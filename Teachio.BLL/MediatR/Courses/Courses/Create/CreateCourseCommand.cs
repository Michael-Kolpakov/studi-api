using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Courses;
using Teachio.BLL.Dto.Courses.Courses.Create;

namespace Teachio.BLL.MediatR.Courses.Courses.Create;

public record CreateCourseCommand(CourseCreateDto courseCreateDto)
    : IRequest<Result<CourseResponseDto>>;
