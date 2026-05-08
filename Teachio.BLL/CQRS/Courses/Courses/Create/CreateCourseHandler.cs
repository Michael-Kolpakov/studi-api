using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Localization;
using Teachio.BLL.DTOs.Courses.Courses.Response;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResource;
using Teachio.DAL.Repositories.Interfaces.Base;
using CourseEntity = Teachio.DAL.Entities.Courses.Courses.Course;

namespace Teachio.BLL.CQRS.Courses.Courses.Create;

public class CreateCourseHandler : IRequestHandler<CreateCourseCommand, Result<CourseResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<CannotMapSharedResource> _stringLocalizerFailedToMap;
    private readonly IStringLocalizer<AlreadyExistsSharedResource> _stringLocalizerAlreadyExists;

    public CreateCourseHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILoggerService logger,
        ICurrentUserService currentUserService,
        IStringLocalizer<CannotMapSharedResource> stringLocalizerFailedToMap,
        IStringLocalizer<AlreadyExistsSharedResource> stringLocalizerAlreadyExists)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
        _currentUserService = currentUserService;
        _stringLocalizerFailedToMap = stringLocalizerFailedToMap;
        _stringLocalizerAlreadyExists = stringLocalizerAlreadyExists;
    }

    public async Task<Result<CourseResponseDto>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        _logger.LogInformation($"Entered '{GetType().Name}' to create a new course by UserId: {userId}");

        var newCourse = _mapper.Map<CourseEntity>(
            request.CourseCreateRequestDto,
            opt => opt.Items["OwnerUserId"] = userId);

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

        await _repositoryWrapper.CoursesRepository.CreateAsync(newCourse, cancellationToken);
        await _repositoryWrapper.SaveChangesAsync(cancellationToken);

        var courseResponseDto = _mapper.Map<CourseResponseDto>(newCourse);

        return Result.Ok(courseResponseDto);
    }
}
