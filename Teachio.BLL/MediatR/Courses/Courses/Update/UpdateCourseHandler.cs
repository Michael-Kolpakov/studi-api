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
        _logger.LogInformation($"Entered '{GetType().Name}' to update a course with Id: {request.courseUpdateRequestDto.Id}");

        // TODO: validate whether course thumbnail exists (database relationships and ownership)

        var existingCourse = await _repositoryWrapper.CoursesRepository
            .GetSingleOrDefaultAsync(x => x.Id == request.courseUpdateRequestDto.Id);

        if (existingCourse is null)
        {
            var errorMessage = _stringLocalizerCannotFind[nameof(CannotFindSharedResource_en.CannotFindCourseById), request.courseUpdateRequestDto.Id].Value;
            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        if (existingCourse.OwnerUserId != request.requestingUserId)
        {
            var errorMessage = "User don't have permission to update this course";
            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        // TODO: validate whether course was updated successfully, if yes - address Google Drive API (or CDN in the future) to delete old thumbnail image and set new one

        // TODO: validate whether course was updated successfully, if not - address Google Drive API (or CDN in the future) to delete current thumbnail image

        _mapper.Map(request.courseUpdateRequestDto, existingCourse);

        _repositoryWrapper.CoursesRepository.Update(existingCourse);
        await _repositoryWrapper.SaveChangesAsync();

        var courseResponseDto = _mapper.Map<CourseResponseDto>(existingCourse);

        return Result.Ok(courseResponseDto);
    }
}
