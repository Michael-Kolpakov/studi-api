using FluentResults;
using MediatR;
using Studi.BLL.DTOs.Courses.Sections.Response;

namespace Studi.BLL.CQRS.Courses.Sections.GetByCourseId;

public record GetSectionsByCourseIdQuery(Guid CourseId)
    : IRequest<Result<CourseSectionsResponseDto>>;
