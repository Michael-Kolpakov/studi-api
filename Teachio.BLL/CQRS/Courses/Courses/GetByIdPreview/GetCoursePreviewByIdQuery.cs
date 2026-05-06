using FluentResults;
using MediatR;
using Teachio.BLL.DTOs.Courses.Courses.Response;

namespace Teachio.BLL.CQRS.Courses.Courses.GetByIdPreview;

public record GetCoursePreviewByIdQuery(Guid CourseId)
    : IRequest<Result<CoursePreviewResponseDto>>;
