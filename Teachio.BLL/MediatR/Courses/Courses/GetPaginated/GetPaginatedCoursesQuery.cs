using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Courses.Response;

namespace Teachio.BLL.MediatR.Courses.Courses.GetPaginated;

public record GetPaginatedCoursesQuery(ushort pageNumber, ushort pageSize, Guid? requestingUserId)
    : IRequest<Result<PaginatedCoursesResponseDto>>;
