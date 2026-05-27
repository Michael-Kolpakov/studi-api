using FluentResults;
using MediatR;
using Studi.BLL.DTOs.Courses.Courses.Response;

namespace Studi.BLL.CQRS.Courses.Courses.GetByIdPreview;

public record GetCoursePreviewByIdQuery(Guid CourseId)
    : IRequest<Result<CoursePreviewResponseDto>>;
