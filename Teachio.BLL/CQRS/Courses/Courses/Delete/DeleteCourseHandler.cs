using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Localization;
using Teachio.BLL.DTOs.Courses.Courses.Response;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResource;
using Teachio.BLL.Utils.Helpers;
using Teachio.DAL.Repositories.Interfaces.Base;
using CourseEntity = Teachio.DAL.Entities.Courses.Courses.Course;

namespace Teachio.BLL.CQRS.Courses.Courses.Delete;

public class DeleteCourseHandler : IRequestHandler<DeleteCourseCommand, Result<CourseResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IGoogleDriveStorageService _googleDriveStorageService;
    private readonly ILoggerService _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;
    private readonly IStringLocalizer<NoPermissionsSharedResource> _stringLocalizerNoPermissions;

    public DeleteCourseHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IGoogleDriveStorageService googleDriveStorageService,
        ILoggerService logger,
        ICurrentUserService currentUserService,
        IStringLocalizer<CannotFindSharedResource> stringLocalizerCannotFind,
        IStringLocalizer<NoPermissionsSharedResource> stringLocalizerNoPermissions)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _googleDriveStorageService = googleDriveStorageService;
        _logger = logger;
        _currentUserService = currentUserService;
        _stringLocalizerCannotFind = stringLocalizerCannotFind;
        _stringLocalizerNoPermissions = stringLocalizerNoPermissions;
    }

    public async Task<Result<CourseResponseDto>> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        _logger.LogInformation($"Entered '{GetType().Name}' to delete a course with Id: {request.CourseId} by UserId: {userId}");

        var course = await _repositoryWrapper.CoursesRepository.GetSingleOrDefaultAsync(
            x => x.Id == request.CourseId,
            IncludeCourseRelatedEntities,
            cancellationToken);

        if (course is null)
        {
            var errorMessage = _stringLocalizerCannotFind[
                nameof(CannotFindSharedResource_en.CannotFindCourseById),
                request.CourseId
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        if (course.OwnerUserId != userId)
        {
            var logErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToDeleteCourseForUserWithId),
                request.CourseId,
                userId
            ].Value;

            _logger.LogError(request, logErrorMessage);

            var responseErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToDeleteCourseForUser),
                request.CourseId
            ].Value;

            return Result.Fail(responseErrorMessage);
        }

        var ownerUserEmail = course.OwnerUser.Email;

        var courseFolderDeletionResult = await DeleteCourseFolderFromCDNAsync(course, ownerUserEmail!, request, cancellationToken);
        if (courseFolderDeletionResult.IsFailed)
        {
            var errorMessage = courseFolderDeletionResult.Errors[0].Message;

            return Result.Fail(errorMessage);
        }

        _repositoryWrapper.CoursesRepository.Delete(course);
        await _repositoryWrapper.SaveChangesAsync(cancellationToken);

        var courseResponseDto = _mapper.Map<CourseResponseDto>(course);

        return Result.Ok(courseResponseDto);
    }

    [ExcludeFromCodeCoverage]
    private static IIncludableQueryable<CourseEntity, object> IncludeCourseRelatedEntities(IQueryable<CourseEntity> query)
    {
        return query
            .Include(c => c.OwnerUser)
            .Include(c => c.ThumbnailFile)
            .Include(c => c.Sections)
                .ThenInclude(s => s.Videos)
                    .ThenInclude(v => v.VideoFile)
            .Include(c => c.Sections)
                .ThenInclude(s => s.Videos)
                    .ThenInclude(v => v.VideoProgress);
    }

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "CDN is a constant abbreviation and it's ok to use it in the method name for better readability and understanding of the method's purpose.")]
    private async Task<Result> DeleteCourseFolderFromCDNAsync(
        CourseEntity course,
        string ownerUserEmail,
        DeleteCourseCommand request,
        CancellationToken cancellationToken)
    {
        var courseFolderDeletionResult = await _googleDriveStorageService.DeleteFolderByPathAsync(
            StoragePathHelper.BuildThumbnailFolderSegments(ownerUserEmail, course.CourseName),
            cancellationToken);

        if (courseFolderDeletionResult.IsFailed)
        {
            var errorMessage = courseFolderDeletionResult.Errors[0].Message;
            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        return Result.Ok();
    }
}
