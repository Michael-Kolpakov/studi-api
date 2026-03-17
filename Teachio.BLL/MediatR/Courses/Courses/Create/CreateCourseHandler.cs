using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Localization;
using Teachio.BLL.Dto.Courses.Courses.Response;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResource;
using Teachio.DAL.Repositories.Interfaces.Base;
using CourseEntity = Teachio.DAL.Entities.Courses.Courses.Course;

namespace Teachio.BLL.MediatR.Courses.Courses.Create;

public class CreateCourseHandler : IRequestHandler<CreateCourseCommand, Result<CourseResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    private readonly IStringLocalizer<CannotMapSharedResource> _stringLocalizerFailedToMap;
    private readonly IStringLocalizer<AlreadyExistsSharedResource> _stringLocalizerAlreadyExists;

    public CreateCourseHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILoggerService logger,
        IStringLocalizer<CannotMapSharedResource> stringLocalizerFailedToMap,
        IStringLocalizer<AlreadyExistsSharedResource> stringLocalizerAlreadyExists)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
        _stringLocalizerFailedToMap = stringLocalizerFailedToMap;
        _stringLocalizerAlreadyExists = stringLocalizerAlreadyExists;
    }

    public async Task<Result<CourseResponseDto>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Entered '{GetType().Name}' to create a new course");

        // TODO: validate whether course thumbnail exists (database relationships and ownership)

        var newCourse = _mapper.Map<CourseEntity>(
            request.CourseCreateRequestDto,
            opt => opt.Items["OwnerUserId"] = request.OwnerUserId);

        if (newCourse is null)
        {
            var errorMessage = _stringLocalizerFailedToMap[nameof(CannotMapSharedResource_en.CannotMapNullToCourse)].Value;
            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var courseWithSameNameExists = await _repositoryWrapper.CoursesRepository.GetSingleOrDefaultAsync(
            c => c.OwnerUserId == newCourse.OwnerUserId && c.CourseName == newCourse.CourseName,
            cancellationToken: cancellationToken);

        if (courseWithSameNameExists is not null)
        {
            var logErrorMessage = _stringLocalizerAlreadyExists[
                nameof(AlreadyExistsSharedResource_en.CourseAlreadyExistsForUserWithId),
                courseWithSameNameExists.CourseName,
                courseWithSameNameExists.OwnerUserId
            ].Value;

            _logger.LogError(request, logErrorMessage);

            var responseErrorMessage = _stringLocalizerAlreadyExists[
                nameof(AlreadyExistsSharedResource_en.CourseAlreadyExistsForUser),
                courseWithSameNameExists.Title
            ].Value;

            return Result.Fail(responseErrorMessage);
        }

        // TODO: validate whether course was created successfully, if not - address Google Drive API (or CDN in the future) to delete thumbnail image

        await _repositoryWrapper.CoursesRepository.CreateAsync(newCourse, cancellationToken);
        await _repositoryWrapper.SaveChangesAsync(cancellationToken);

        var courseResponseDto = _mapper.Map<CourseResponseDto>(newCourse);

        return Result.Ok(courseResponseDto);
    }
}
