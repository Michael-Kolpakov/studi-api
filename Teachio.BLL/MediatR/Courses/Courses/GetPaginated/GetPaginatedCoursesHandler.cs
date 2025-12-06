using AutoMapper;
using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Courses.Response;
using Teachio.BLL.Services.Interfaces;
using Teachio.DAL.Repositories.Interfaces.Base;

namespace Teachio.BLL.MediatR.Courses.Courses.GetPaginated;

public class GetPaginatedCoursesHandler : IRequestHandler<GetPaginatedCoursesQuery, Result<PaginatedCoursesResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public GetPaginatedCoursesHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILoggerService logger)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedCoursesResponseDto>> Handle(GetPaginatedCoursesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Entered '{GetType().Name}' to get paginated courses (page number: {request.pageNumber}, page size: {request.pageSize})");

        var paginatedCourses = await _repositoryWrapper.CoursesRepository.GetAllPaginatedAsync(
            request.pageNumber,
            request.pageSize);

        var getAllCoursesResponseDto = new PaginatedCoursesResponseDto()
        {
            TotalAmount = paginatedCourses.TotalItems,
            Courses = _mapper.Map<IEnumerable<CoursePreviewShortResponseDto>>(paginatedCourses.Entities)
        };

        return Result.Ok(getAllCoursesResponseDto);
    }
}
