using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Localization;
using Studi.BLL.DTOs.Courses.Courses.Response;
using Studi.BLL.Resources.SharedResource;
using Studi.BLL.Services.Interfaces;
using Studi.BLL.SharedResource;
using Studi.DAL.Entities.Courses.Courses;
using Studi.DAL.Repositories.Interfaces.Base;

namespace Studi.BLL.CQRS.Courses.Courses.Create;

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

        var newCourse = _mapper.Map<Course>(
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
                courseWithSameNameExists.CourseName
            ].Value;

            return Result.Fail(responseErrorMessage);
        }

        await _repositoryWrapper.CoursesRepository.CreateAsync(newCourse, cancellationToken);
        await _repositoryWrapper.SaveChangesAsync(cancellationToken);

        await LoadOwnerUserAsync(newCourse, userId, cancellationToken);

        var courseResponseDto = _mapper.Map<CourseResponseDto>(newCourse);

        return Result.Ok(courseResponseDto);
    }

    private async Task LoadOwnerUserAsync(Course course, Guid ownerUserId, CancellationToken cancellationToken)
    {
        if (course.OwnerUser is not null)
        {
            return;
        }

        var ownerUser = await _repositoryWrapper.AppUsersRepository.GetSingleOrDefaultAsync(
            appUser => appUser.Id == ownerUserId,
            cancellationToken: cancellationToken);

        if (ownerUser is not null)
        {
            course.OwnerUser = ownerUser;
        }
    }
}
