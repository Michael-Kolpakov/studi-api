using AutoMapper;
using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Courses.Response;
using Teachio.BLL.Services.Interfaces;
using Teachio.DAL.Repositories.Interfaces.Base;

namespace Teachio.BLL.MediatR.Courses.Courses.GetPaginated;

public class GetPaginatedCoursesHandler : IRequestHandler<GetPaginatedCoursesQuery, Result<GetPaginatedCoursesResponseDto>>
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

    public Task<Result<GetPaginatedCoursesResponseDto>> Handle(GetPaginatedCoursesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Entered '{GetType().Name}' to get paginated courses");

        var paginatedCourses = _repositoryWrapper.CoursesRepository.GetAllPaginated(request.pageNumber, request.pageSize);

        var getAllCoursesResponseDto = new GetPaginatedCoursesResponseDto()
        {
            TotalAmount = paginatedCourses.TotalItems,
            Courses = _mapper.Map<IEnumerable<CoursePreviewShortResponseDto>>(paginatedCourses.Entities)
        };

        return Task.FromResult(Result.Ok(getAllCoursesResponseDto));
    }
}
