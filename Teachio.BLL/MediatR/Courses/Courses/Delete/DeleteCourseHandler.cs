using AutoMapper;
using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Courses;
using Teachio.BLL.Services.Interfaces;
using Teachio.DAL.Repositories.Interfaces.Base;

namespace Teachio.BLL.MediatR.Courses.Courses.Delete;

public class DeleteCourseHandler : IRequestHandler<DeleteCourseCommand, Result<CourseResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public DeleteCourseHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILoggerService logger)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    public async Task<Result<CourseResponseDto>> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Entered '{GetType().Name}' to delete a course with Id: {request.id}");

        var course = await _repositoryWrapper.CoursesRepository.GetSingleOrDefaultAsync(x => x.Id == request.id);

        if (course is null)
        {
            var errorMessage = $"There is no section with such Id: {request.id}";
            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        _repositoryWrapper.CoursesRepository.Delete(course);
        await _repositoryWrapper.SaveChangesAsync();

        var courseResponseDto = _mapper.Map<CourseResponseDto>(course);

        return Result.Ok(courseResponseDto);
    }
}
