using AutoMapper;
using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Courses;
using Teachio.BLL.Services.Interfaces;
using Teachio.DAL.Repositories.Interfaces.Base;
using CourseEntity = Teachio.DAL.Entities.Courses.Courses.Course;

namespace Teachio.BLL.MediatR.Courses.Courses.Create;

public class CreateCourseHandler : IRequestHandler<CreateCourseCommand, Result<CourseResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public CreateCourseHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILoggerService logger)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    public async Task<Result<CourseResponseDto>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Entered 'CreateCourseHandler' to create a new course");

        var newCourse = _mapper.Map<CourseEntity>(request.courseCreateDto);

        await _repositoryWrapper.CoursesRepository.CreateAsync(newCourse);
        await _repositoryWrapper.SaveChangesAsync();

        var courseResponseDto = _mapper.Map<CourseResponseDto>(newCourse);

        return Result.Ok(courseResponseDto);
    }
}
