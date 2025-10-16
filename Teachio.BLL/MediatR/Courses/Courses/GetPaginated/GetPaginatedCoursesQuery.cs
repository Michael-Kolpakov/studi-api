using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Courses;

namespace Teachio.BLL.MediatR.Courses.Courses.GetPaginated;

public record GetPaginatedCoursesQuery(ushort? pageNumber = null, ushort? pageSize = null)
    : IRequest<Result<GetAllCoursesResponseDto>>;
