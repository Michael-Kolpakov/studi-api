using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Courses;
using Teachio.BLL.Dto.Courses.Courses.Update;

namespace Teachio.BLL.MediatR.Courses.Courses.Update;

public record UpdateCourseCommand(CourseUpdateDto courseUpdateDto)
    : IRequest<Result<CourseResponseDto>>;
