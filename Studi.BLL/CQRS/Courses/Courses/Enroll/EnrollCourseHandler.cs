using System.Diagnostics.CodeAnalysis;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Localization;
using Studi.BLL.DTOs.Courses.Courses.Response;
using Studi.BLL.Resources.SharedResource;
using Studi.BLL.Services.Interfaces;
using Studi.BLL.SharedResource;
using Studi.BLL.Utils.Helpers;
using Studi.DAL.Entities.Courses.Courses;
using Studi.DAL.Entities.Shared;
using Studi.DAL.Repositories.Interfaces.Base;

namespace Studi.BLL.CQRS.Courses.Courses.Enroll;

public class EnrollCourseHandler : IRequestHandler<EnrollCourseCommand, Result<CourseEnrollResponseDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IEntityExistenceService _entityExistenceService;
    private readonly ILoggerService _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<AlreadyExistsSharedResource> _stringLocalizerAlreadyExists;

    public EnrollCourseHandler(
        IRepositoryWrapper repositoryWrapper,
        IEntityExistenceService entityExistenceService,
        ILoggerService logger,
        ICurrentUserService currentUserService,
        IStringLocalizer<AlreadyExistsSharedResource> stringLocalizerAlreadyExists)
    {
        _repositoryWrapper = repositoryWrapper;
        _entityExistenceService = entityExistenceService;
        _logger = logger;
        _currentUserService = currentUserService;
        _stringLocalizerAlreadyExists = stringLocalizerAlreadyExists;
    }

    public async Task<Result<CourseEnrollResponseDto>> Handle(EnrollCourseCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        var courseId = request.CourseEnrollRequestDto.CourseId;

        _logger.LogInformation(
            $"Entered '{GetType().Name}' to enroll user with Id: {userId} into course with Id: {courseId}");

        var (course, existenceErrorMessage) = await _entityExistenceService.CheckCourseExistenceAsync(
            courseId,
            nameof(request.CourseEnrollRequestDto.CourseId),
            cancellationToken);

        if (course is null)
        {
            _logger.LogError(request, existenceErrorMessage!);

            return Result.Fail(existenceErrorMessage);
        }

        var existingEnrollment = await _repositoryWrapper.UserCoursesRepository.GetSingleOrDefaultAsync(
            x => x.AppUserId == userId && x.CourseId == courseId,
            cancellationToken: cancellationToken);

        if (existingEnrollment is not null)
        {
            var logErrorMessage = _stringLocalizerAlreadyExists[
                nameof(AlreadyExistsSharedResource_en.CourseAlreadyEnrolledForUserWithId),
                courseId,
                userId
            ].Value;

            _logger.LogError(request, logErrorMessage);

            var responseErrorMessage = _stringLocalizerAlreadyExists[
                nameof(AlreadyExistsSharedResource_en.CourseAlreadyEnrolled),
                courseId
            ].Value;

            return Result.Fail(responseErrorMessage);
        }

        var userCourse = new UserCourse()
        {
            AppUserId = userId,
            CourseId = courseId
        };

        await _repositoryWrapper.UserCoursesRepository.CreateAsync(userCourse, cancellationToken);

        var courseWithVideos = await _repositoryWrapper.CoursesRepository.GetSingleOrDefaultAsync(
            c => c.Id == courseId,
            IncludeCourseRelatedEntities,
            cancellationToken);

        var videoIds = courseWithVideos is null
            ? []
            : courseWithVideos.Sections
                .SelectMany(section => section.Videos)
                .Select(video => video.Id)
                .ToList();

        var progressItems = VideoProgressHelper.CreateForVideosAndUser(videoIds, userId);

        if (progressItems.Count > 0)
        {
            await _repositoryWrapper.VideoProgressRepository.CreateRangeAsync(progressItems, cancellationToken);
        }

        course.WatchingUsersCount += 1;

        _repositoryWrapper.CoursesRepository.Update(course);
        await _repositoryWrapper.SaveChangesAsync(cancellationToken);

        var response = new CourseEnrollResponseDto()
        {
            CourseId = courseId,
            WatchingUsersCount = course.WatchingUsersCount
        };

        return Result.Ok(response);
    }

    [ExcludeFromCodeCoverage]
    private static IIncludableQueryable<Course, object> IncludeCourseRelatedEntities(IQueryable<Course> query)
    {
        return query
            .Include(c => c.Sections)
                .ThenInclude(s => s.Videos);
    }
}
