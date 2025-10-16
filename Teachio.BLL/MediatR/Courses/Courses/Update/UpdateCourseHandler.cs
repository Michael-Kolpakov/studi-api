using AutoMapper;
using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Courses;
using Teachio.BLL.Services.Interfaces;
using Teachio.DAL.Repositories.Interfaces.Base;
using CourseEntity = Teachio.DAL.Entities.Courses.Courses.Course;

namespace Teachio.BLL.MediatR.Courses.Courses.Update;

public class UpdateCourseHandler : IRequestHandler<UpdateCourseCommand, Result<CourseResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public UpdateCourseHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILoggerService logger)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    public async Task<Result<CourseResponseDto>> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Entered 'UpdateCourseHandler' to update a course with Id: {request.courseUpdateDto.Id}");

        var course = _mapper.Map<CourseEntity>(request.courseUpdateDto);

        _repositoryWrapper.CoursesRepository.Update(course);
        await _repositoryWrapper.SaveChangesAsync();

        var courseResponseDto = _mapper.Map<CourseResponseDto>(course);

        return Result.Ok(courseResponseDto);
    }
}
