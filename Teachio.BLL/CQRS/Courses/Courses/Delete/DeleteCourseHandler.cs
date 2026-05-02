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
using Teachio.BLL.SharedResources;
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

        // TODO: make sure whether we really delete all course dependent entities: Sections, Videos and VideoProgress

        // TODO: address Google Drive API (or CDN in the future) to delete thumbnail image
        var ownerUserEmail = course.OwnerUser.Email;

        var thumbnailDeletionResult = await DeleteCourseThumbnailFromCDNAsync(course, ownerUserEmail!, request, cancellationToken);
        if (thumbnailDeletionResult.IsFailed)
        {
            var errorMessage = thumbnailDeletionResult.Errors[0].Message;

            return Result.Fail(errorMessage);
        }

        var videosDeletionResult = await DeleteCourseVideosFromCDNAsync(course, ownerUserEmail!, request, cancellationToken);
        if (videosDeletionResult.IsFailed)
        {
            var errorMessage = videosDeletionResult.Errors[0].Message;

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
            .Include(c => c.Sections)
                .ThenInclude(s => s.Videos)
                    .ThenInclude(v => v.VideoFile)
            .Include(c => c.Sections)
                .ThenInclude(s => s.Videos)
                    .ThenInclude(v => v.VideoProgress);
    }

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "CDN is a constant abbreviation and it's ok to use it in the method name for better readability and understanding of the method's purpose.")]
    private async Task<Result> DeleteCourseThumbnailFromCDNAsync(
        CourseEntity course,
        string ownerUserEmail,
        DeleteCourseCommand request,
        CancellationToken cancellationToken)
    {
        var courseThumbnailFile = course.ThumbnailFile;
        if (courseThumbnailFile is null)
        {
            return Result.Ok();
        }

        var deleteGoogleDriveFileResult = await _googleDriveStorageService.DeleteFileByPathAsync(
            StoragePathHelper.BuildThumbnailFolderSegments(ownerUserEmail, course.CourseName),
            courseThumbnailFile.ThumbnailName,
            cancellationToken);

        if (deleteGoogleDriveFileResult.IsFailed)
        {
            var errorMessage = deleteGoogleDriveFileResult.Errors[0].Message;
            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        return Result.Ok();
    }

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "CDN is a constant abbreviation and it's ok to use it in the method name for better readability and understanding of the method's purpose.")]
    private async Task<Result> DeleteCourseVideosFromCDNAsync(
        CourseEntity course,
        string ownerUserEmail,
        DeleteCourseCommand request,
        CancellationToken cancellationToken)
    {
        var courseVideoFiles = course.Sections
            .SelectMany(section => section.Videos)
            .Where(video => video.VideoFile is not null)
            .Select(video => new
            {
                video.Section!.SectionName,
                VideoFileName = video.VideoFile!.VideoName
            })
            .ToList();

        foreach (var courseVideoFile in courseVideoFiles)
        {
            var deleteGoogleDriveFileResult = await _googleDriveStorageService.DeleteFileByPathAsync(
                StoragePathHelper.BuildVideoFolderSegments(
                    ownerUserEmail,
                    course.CourseName,
                    courseVideoFile.SectionName),
                courseVideoFile.VideoFileName,
                cancellationToken);

            if (deleteGoogleDriveFileResult.IsFailed)
            {
                var errorMessage = deleteGoogleDriveFileResult.Errors[0].Message;
                _logger.LogError(request, errorMessage);

                return Result.Fail(errorMessage);
            }
        }

        return Result.Ok();
    }
}
