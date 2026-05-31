using FluentResults;
using MediatR;
using Studi.BLL.DTOs.Courses.Courses.Response;

namespace Studi.BLL.CQRS.Courses.Courses.GetPaginated;

public record GetPaginatedCoursesQuery(
    ushort PageNumber,
    ushort PageSize,
    string? TitleFilter = null,
    CoursesSortBy SortBy = CoursesSortBy.None,
    SortDirection SortDirection = SortDirection.None,
    CoursesPaginationMode Mode = CoursesPaginationMode.Available)
    : IRequest<Result<PaginatedCoursesResponseDto>>;

public enum CoursesSortBy
{
    None = 0,
    TotalDurationHours = 1,
    WatchingUsersCount = 2
}

public enum SortDirection
{
    None = 0,
    Ascending = 1,
    Descending = 2
}

public enum CoursesPaginationMode
{
    Available = 0,
    InProgress = 1,
    Personal = 2
}
