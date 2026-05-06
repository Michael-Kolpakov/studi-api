using FluentResults;
using MediatR;
using Teachio.BLL.DTOs.Courses.Courses.Response;

namespace Teachio.BLL.CQRS.Courses.Courses.GetPaginated;

public record GetPaginatedCoursesQuery(ushort PageNumber, ushort PageSize)
    : IRequest<Result<PaginatedCoursesResponseDto>>;
