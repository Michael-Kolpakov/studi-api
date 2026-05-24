using FluentResults;
using MediatR;
using Teachio.BLL.DTOs.Courses.Sections.Response;

namespace Teachio.BLL.CQRS.Courses.Sections.GetByCourseId;

public record GetSectionsByCourseIdQuery(Guid CourseId)
    : IRequest<Result<CourseSectionsResponseDto>>;
