using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Courses.Response;

namespace Teachio.BLL.MediatR.Courses.Courses.GetByIdPreview;

public record GetCoursePreviewByIdQuery(Guid id)
    : IRequest<Result<CoursePreviewResponseDto>>;
