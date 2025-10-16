using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Courses;

namespace Teachio.BLL.MediatR.Courses.Courses.GetById;

public record GetCourseByIdQuery(Guid id)
    : IRequest<Result<CourseResponseDto>>;
