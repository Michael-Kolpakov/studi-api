using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Courses.Response;

namespace Teachio.BLL.MediatR.Courses.Courses.GetPaginated;

/// <summary>
/// Represents the <see cref="GetPaginatedCoursesQuery"/> record model.
/// </summary>
/// <param name="PageNumber">The page number to retrieve.</param>
/// <param name="PageSize">The number of items per page.</param>
/// <param name="RequestingUserId">The identifier of <paramref name="RequestingUserId"/>.</param>
public record GetPaginatedCoursesQuery(ushort PageNumber, ushort PageSize, Guid? RequestingUserId)
    : IRequest<Result<PaginatedCoursesResponseDto>>;
