using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Localization;
using Teachio.BLL.Dto.Courses.Courses.Response;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResource;
using Teachio.DAL.Repositories.Interfaces.Base;

namespace Teachio.BLL.MediatR.Courses.Courses.Update;

public class UpdateCourseHandler : IRequestHandler<UpdateCourseCommand, Result<CourseResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;

    public UpdateCourseHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILoggerService logger,
        IStringLocalizer<CannotFindSharedResource> stringLocalizerCannotFind)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
        _stringLocalizerCannotFind = stringLocalizerCannotFind;
    }

    public async Task<Result<CourseResponseDto>> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Entered '{GetType().Name}' to update a course with Id: {request.CourseUpdateRequestDto.Id}");

        // TODO: Validate whether gained course really belongs to the user making the request (and perhaps remove check below)

        var existingCourse = await _repositoryWrapper.CoursesRepository
            .GetSingleOrDefaultAsync(x => x.Id == request.CourseUpdateRequestDto.Id);

        if (existingCourse is null)
        {
            var errorMessage = _stringLocalizerCannotFind[nameof(CannotFindSharedResource_en.CannotFindCourseById), request.CourseUpdateRequestDto.Id].Value;
            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        _mapper.Map(request.CourseUpdateRequestDto, existingCourse);

        _repositoryWrapper.CoursesRepository.Update(existingCourse);
        await _repositoryWrapper.SaveChangesAsync();

        var courseResponseDto = _mapper.Map<CourseResponseDto>(existingCourse);

        return Result.Ok(courseResponseDto);
    }
}
