using AutoMapper;
using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Courses;
using Teachio.BLL.Services.Interfaces;
using Teachio.DAL.Repositories.Interfaces.Base;

namespace Teachio.BLL.MediatR.Courses.Courses.GetById;

public class GetByIdCourseHandler : IRequestHandler<GetCourseByIdQuery, Result<CourseResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public GetByIdCourseHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILoggerService logger)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    public async Task<Result<CourseResponseDto>> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Entered 'GetByIdCourseHandler' to get course by Id: {request.id}");

        var course = await _repositoryWrapper.CoursesRepository.GetSingleOrDefaultAsync(x => x.Id == request.id);

        if (course is null)
        {
            var errorMessage = $"There is no course with such Id: {request.id}";
            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var courseResponseDto = _mapper.Map<CourseResponseDto>(course);

        return Result.Ok(courseResponseDto);
    }
}
