using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Courses.Response;

namespace Teachio.BLL.MediatR.Courses.Courses.GetById;

public record GetCourseByIdQuery(Guid courseId, Guid? selectedVideoId = null)
    : IRequest<Result<CourseResponseDto>>;
