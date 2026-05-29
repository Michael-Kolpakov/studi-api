using System.Diagnostics.CodeAnalysis;
using AutoMapper;
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
using Studi.BLL.Utils.MappingResolvers;
using Studi.DAL.Entities.Courses.Courses;
using Studi.DAL.Repositories.Interfaces.Base;

namespace Studi.BLL.CQRS.Courses.Courses.Update;

public class UpdateCourseHandler : IRequestHandler<UpdateCourseCommand, Result<CourseResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IGoogleDriveStorageService _googleDriveStorageService;
    private readonly ILoggerService _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;
    private readonly IStringLocalizer<NoPermissionsSharedResource> _stringLocalizerNoPermissions;
    private readonly IStringLocalizer<AlreadyExistsSharedResource> _stringLocalizerAlreadyExists;
    private readonly IStringLocalizer<BllSharedResource> _stringLocalizerBll;

    public UpdateCourseHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IGoogleDriveStorageService googleDriveStorageService,
        ILoggerService logger,
        ICurrentUserService currentUserService,
        IStringLocalizer<CannotFindSharedResource> stringLocalizerCannotFind,
        IStringLocalizer<NoPermissionsSharedResource> stringLocalizerNoPermissions,
        IStringLocalizer<AlreadyExistsSharedResource> stringLocalizerAlreadyExists,
        IStringLocalizer<BllSharedResource> stringLocalizerBll)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _googleDriveStorageService = googleDriveStorageService;
        _logger = logger;
        _currentUserService = currentUserService;
        _stringLocalizerCannotFind = stringLocalizerCannotFind;
        _stringLocalizerNoPermissions = stringLocalizerNoPermissions;
        _stringLocalizerAlreadyExists = stringLocalizerAlreadyExists;
        _stringLocalizerBll = stringLocalizerBll;
    }

    public async Task<Result<CourseResponseDto>> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        _logger.LogInformation($"Entered '{GetType().Name}' to update a course with Id: {request.CourseUpdateRequestDto.Id} by UserId: {userId}");

        var existingCourse = await _repositoryWrapper.CoursesRepository.GetSingleOrDefaultAsync(
            x => x.Id == request.CourseUpdateRequestDto.Id,
            include: IncludeCourseRelatedEntities,
            cancellationToken: cancellationToken);

        if (existingCourse is null)
        {
            var errorMessage = _stringLocalizerCannotFind[
                nameof(CannotFindSharedResource_en.CannotFindCourseById),
                request.CourseUpdateRequestDto.Id
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        if (existingCourse.OwnerUserId != userId)
        {
            var logErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToUpdateCourseForUserWithId),
                request.CourseUpdateRequestDto.Id,
                userId
            ].Value;

            _logger.LogError(request, logErrorMessage);

            var responseErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToUpdateCourseForUser),
                request.CourseUpdateRequestDto.Id
            ].Value;

            return Result.Fail(responseErrorMessage);
        }

        var courseWithSameNameExists = await _repositoryWrapper.CoursesRepository.GetSingleOrDefaultAsync(
            c => c.OwnerUserId == existingCourse.OwnerUserId
                 && c.CourseName == NameFromTitleResolver.CreateNameFromTitle(request.CourseUpdateRequestDto.Title)
                 && c.Id != request.CourseUpdateRequestDto.Id,
            cancellationToken: cancellationToken);

        if (courseWithSameNameExists is not null)
        {
            var logErrorMessage = _stringLocalizerAlreadyExists[
                nameof(AlreadyExistsSharedResource_en.CourseAlreadyExistsForUserWithId),
                existingCourse.CourseName,
                existingCourse.OwnerUserId
            ].Value;

            _logger.LogError(request, logErrorMessage);

            var responseErrorMessage = _stringLocalizerAlreadyExists[
                nameof(AlreadyExistsSharedResource_en.CourseAlreadyExistsForUser),
                existingCourse.Title
            ].Value;

            return Result.Fail(responseErrorMessage);
        }

        var currentCourseName = existingCourse.CourseName;
        var updatedCourseName = NameFromTitleResolver.CreateNameFromTitle(request.CourseUpdateRequestDto.Title);
        var ownerUserEmail = existingCourse.OwnerUser?.Email;

        var shouldRenameCourseFolder = !string.Equals(currentCourseName, updatedCourseName, StringComparison.Ordinal);

        if (shouldRenameCourseFolder)
        {
            var renameResult = await _googleDriveStorageService.RenameFolderByPathAsync(
                StoragePathHelper.BuildCourseFolderSegments(ownerUserEmail!, currentCourseName),
                updatedCourseName,
                cancellationToken);

            if (renameResult.IsFailed)
            {
                var errorMessage = renameResult.Errors[0].Message;
                _logger.LogError(request, errorMessage);

                return Result.Fail(errorMessage);
            }
        }

        _mapper.Map(request.CourseUpdateRequestDto, existingCourse);

        try
        {
            _repositoryWrapper.CoursesRepository.Update(existingCourse);
            await _repositoryWrapper.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            if (shouldRenameCourseFolder)
            {
                var rollbackResult = await _googleDriveStorageService.RenameFolderByPathAsync(
                    StoragePathHelper.BuildCourseFolderSegments(ownerUserEmail!, updatedCourseName),
                    currentCourseName,
                    cancellationToken);

                if (rollbackResult.IsFailed)
                {
                    _logger.LogError(request, rollbackResult.Errors[0].Message);
                }
            }

            var errorMessage = _stringLocalizerBll[BllSharedResource_en.CourseUpdateFailed].Value;
            _logger.LogError(request, errorMessage, ex.ToString());

            return Result.Fail(errorMessage);
        }

        var courseResponseDto = _mapper.Map<CourseResponseDto>(existingCourse);

        return Result.Ok(courseResponseDto);
    }

    [ExcludeFromCodeCoverage]
    private static IIncludableQueryable<Course, object> IncludeCourseRelatedEntities(IQueryable<Course> query)
    {
        return query.Include(c => c.OwnerUser);
    }
}
