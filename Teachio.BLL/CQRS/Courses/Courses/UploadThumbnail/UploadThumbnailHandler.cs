using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Teachio.BLL.DTOs.Courses.Courses.Response;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResource;
using Teachio.BLL.Utils.Helpers;
using Teachio.DAL.Entities.Courses.ThumbnailFiles;
using Teachio.DAL.Repositories.Interfaces.Base;
using Teachio.DAL.Utils.Constants;

namespace Teachio.BLL.CQRS.Courses.Courses.UploadThumbnail;

public class UploadThumbnailHandler : IRequestHandler<UploadThumbnailCommand, Result<ThumbnailUploadResponseDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IGoogleDriveStorageService _googleDriveStorageService;
    private readonly IThumbnailMetadataService _thumbnailMetadataService;
    private readonly ILoggerService _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;
    private readonly IStringLocalizer<NoPermissionsSharedResource> _stringLocalizerNoPermissions;
    private readonly IStringLocalizer<ThumbnailUploadSharedResource> _stringLocalizerThumbnailUpload;

    public UploadThumbnailHandler(
        IRepositoryWrapper repositoryWrapper,
        IGoogleDriveStorageService googleDriveStorageService,
        IThumbnailMetadataService thumbnailMetadataService,
        ILoggerService logger,
        ICurrentUserService currentUserService,
        IStringLocalizer<CannotFindSharedResource> stringLocalizerCannotFind,
        IStringLocalizer<NoPermissionsSharedResource> stringLocalizerNoPermissions,
        IStringLocalizer<ThumbnailUploadSharedResource> stringLocalizerThumbnailUpload)
    {
        _repositoryWrapper = repositoryWrapper;
        _googleDriveStorageService = googleDriveStorageService;
        _thumbnailMetadataService = thumbnailMetadataService;
        _logger = logger;
        _currentUserService = currentUserService;
        _stringLocalizerCannotFind = stringLocalizerCannotFind;
        _stringLocalizerNoPermissions = stringLocalizerNoPermissions;
        _stringLocalizerThumbnailUpload = stringLocalizerThumbnailUpload;
    }

    public async Task<Result<ThumbnailUploadResponseDto>> Handle(UploadThumbnailCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        _logger.LogInformation($"Entered '{GetType().Name}' to upload a thumbnail for course with Id: {request.ThumbnailUploadRequestDto.CourseId} by UserId: {userId}");

        if (request.ThumbnailUploadRequestDto.ThumbnailFile.Length == 0)
        {
            var errorMessage = _stringLocalizerThumbnailUpload[
                nameof(ThumbnailUploadSharedResource_en.UploadedThumbnailFileIsEmpty)
            ].Value;
            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        if (request.ThumbnailUploadRequestDto.ThumbnailFile.Length > EntityConstants.MaxThumbnailFileSizeBytes)
        {
            var maxThumbnailFileSizeMegabytes = EntityConstants.MaxThumbnailFileSizeBytes / (1024L * 1024L);
            var errorMessage = _stringLocalizerThumbnailUpload[
                nameof(ThumbnailUploadSharedResource_en.UploadedThumbnailFileSizeExceedsLimit),
                maxThumbnailFileSizeMegabytes
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var uploadThumbnailContext = await _repositoryWrapper.CoursesRepository.GetSingleOrDefaultProjectedAsync(
            x => new UploadThumbnailContext
            {
                CourseId = x.Id,
                OwnerUserId = x.OwnerUserId,
                OwnerUserEmail = x.OwnerUser!.Email,
                CourseName = x.CourseName,
                ExistingThumbnailName = x.ThumbnailFile == null
                    ? null
                    : x.ThumbnailFile.ThumbnailName
            },
            x => x.Id == request.ThumbnailUploadRequestDto.CourseId,
            cancellationToken);

        if (uploadThumbnailContext is null)
        {
            var errorMessage = _stringLocalizerCannotFind[
                nameof(CannotFindSharedResource_en.CannotFindCourseById),
                request.ThumbnailUploadRequestDto.CourseId
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        if (uploadThumbnailContext.OwnerUserId != userId)
        {
            var logErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToUpdateCourseForUserWithId),
                request.ThumbnailUploadRequestDto.CourseId,
                userId
            ].Value;

            _logger.LogError(request, logErrorMessage);

            var responseErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToUpdateCourseForUser),
                request.ThumbnailUploadRequestDto.CourseId
            ].Value;

            return Result.Fail(responseErrorMessage);
        }

        var temporaryFilePath = CreateTemporaryFilePath(request.ThumbnailUploadRequestDto.ThumbnailFile.FileName);
        string? uploadedGoogleDriveFileId = null;

        try
        {
            await SaveUploadedFileToTemporaryStorageAsync(
                request.ThumbnailUploadRequestDto.ThumbnailFile,
                temporaryFilePath,
                cancellationToken);

            var metadataResult = await _thumbnailMetadataService.GetMetadataAsync(temporaryFilePath, cancellationToken);

            if (metadataResult.IsFailed)
            {
                var errorMessage = metadataResult.Errors[0].Message;
                _logger.LogError(request, errorMessage);

                return Result.Fail(errorMessage);
            }

            var thumbnailName = $"{uploadThumbnailContext.CourseName}.{metadataResult.Value.FileExtension}";

            if (thumbnailName.Length > EntityConstants.MaxThumbnailFileNameLength)
            {
                var errorMessage = _stringLocalizerThumbnailUpload[
                    nameof(ThumbnailUploadSharedResource_en.GeneratedThumbnailFileNameTooLong),
                    EntityConstants.MaxThumbnailFileNameLength
                ].Value;
                _logger.LogError(request, errorMessage);

                return Result.Fail(errorMessage);
            }

            if (!string.IsNullOrWhiteSpace(uploadThumbnailContext.ExistingThumbnailName))
            {
                var deleteExistingFileResult = await _googleDriveStorageService.DeleteFileByPathAsync(
                    StoragePathHelper.BuildThumbnailFolderSegments(
                        uploadThumbnailContext.OwnerUserEmail!,
                        uploadThumbnailContext.CourseName),
                    uploadThumbnailContext.ExistingThumbnailName,
                    cancellationToken);

                if (deleteExistingFileResult.IsFailed)
                {
                    var errorMessage = deleteExistingFileResult.Errors[0].Message;
                    _logger.LogError(request, errorMessage);

                    return Result.Fail(errorMessage);
                }
            }

            await using var uploadStream = new FileStream(
                temporaryFilePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 81920,
                useAsync: true);

            var uploadResult = await _googleDriveStorageService.UploadFileAsync(
                StoragePathHelper.BuildThumbnailFolderSegments(
                    uploadThumbnailContext.OwnerUserEmail!,
                    uploadThumbnailContext.CourseName),
                thumbnailName,
                metadataResult.Value.ContentType,
                uploadStream,
                cancellationToken);

            if (uploadResult.IsFailed)
            {
                var errorMessage = uploadResult.Errors[0].Message;
                _logger.LogError(request, errorMessage);

                return Result.Fail(errorMessage);
            }

            uploadedGoogleDriveFileId = uploadResult.Value.FileId;

            var course = await _repositoryWrapper.CoursesRepository.GetSingleOrDefaultAsync(
                x => x.Id == request.ThumbnailUploadRequestDto.CourseId,
                cancellationToken: cancellationToken);

            if (course is null)
            {
                var errorMessage = _stringLocalizerCannotFind[
                    nameof(CannotFindSharedResource_en.CannotFindCourseById),
                    request.ThumbnailUploadRequestDto.CourseId
                ].Value;

                _logger.LogError(request, errorMessage);

                return Result.Fail(errorMessage);
            }

            var existingThumbnailFileEntity = await _repositoryWrapper.ThumbnailFilesRepository.GetSingleOrDefaultAsync(
                x => x.CourseId == request.ThumbnailUploadRequestDto.CourseId,
                cancellationToken: cancellationToken);

            ThumbnailFile? newThumbnailFileEntity = null;
            if (existingThumbnailFileEntity is null)
            {
                newThumbnailFileEntity = new ThumbnailFile
                {
                    CourseId = course.Id,
                    ThumbnailName = thumbnailName,
                    ContentType = metadataResult.Value.ContentType,
                    Resolution = metadataResult.Value.Resolution
                };

                await _repositoryWrapper.ThumbnailFilesRepository.CreateAsync(newThumbnailFileEntity, cancellationToken);
            }
            else
            {
                existingThumbnailFileEntity.ThumbnailName = thumbnailName;
                existingThumbnailFileEntity.ContentType = metadataResult.Value.ContentType;
                existingThumbnailFileEntity.Resolution = metadataResult.Value.Resolution;

                _repositoryWrapper.ThumbnailFilesRepository.Update(existingThumbnailFileEntity);
            }

            await _repositoryWrapper.SaveChangesAsync(cancellationToken);

            var thumbnailUploadResponseDto = new ThumbnailUploadResponseDto
            {
                Id = existingThumbnailFileEntity is not null ? existingThumbnailFileEntity.Id : newThumbnailFileEntity!.Id,
                ThumbnailName = thumbnailName,
                ContentType = metadataResult.Value.ContentType,
                Resolution = metadataResult.Value.Resolution,
                CourseId = course.Id
            };

            return Result.Ok(thumbnailUploadResponseDto);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            var errorMessage = _stringLocalizerThumbnailUpload[
                nameof(ThumbnailUploadSharedResource_en.UploadThumbnailProcessingFailed)
            ].Value;
            _logger.LogError(request, errorMessage, ex.ToString());

            if (!string.IsNullOrWhiteSpace(uploadedGoogleDriveFileId))
            {
                var rollbackResult = await _googleDriveStorageService.DeleteFileAsync(uploadedGoogleDriveFileId, cancellationToken);

                if (rollbackResult.IsFailed)
                {
                    var rollbackErrorMessage = _stringLocalizerThumbnailUpload[
                        nameof(ThumbnailUploadSharedResource_en.UploadThumbnailRollbackFailed),
                        uploadedGoogleDriveFileId,
                        rollbackResult.Errors[0].Message
                    ].Value;

                    _logger.LogError(request, rollbackErrorMessage);
                }
            }

            return Result.Fail(errorMessage);
        }
        finally
        {
            DeleteTemporaryFileIfExists(temporaryFilePath);
        }
    }

    private static string CreateTemporaryFilePath(string sourceFileName)
    {
        var extension = Path.GetExtension(sourceFileName);

        return Path.Combine(Path.GetTempPath(), $"teachio-thumbnail-{Guid.NewGuid():N}{extension}");
    }

    private static async Task SaveUploadedFileToTemporaryStorageAsync(
        IFormFile uploadedFile,
        string temporaryFilePath,
        CancellationToken cancellationToken)
    {
        await using var temporaryFileStream = new FileStream(
            temporaryFilePath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 81920,
            useAsync: true);

        await uploadedFile.CopyToAsync(temporaryFileStream, cancellationToken);
    }

    private static void DeleteTemporaryFileIfExists(string temporaryFilePath)
    {
        try
        {
            if (File.Exists(temporaryFilePath))
            {
                File.Delete(temporaryFilePath);
            }
        }
        catch
        {
            // Ignore temporary cleanup errors intentionally
        }
    }

    private sealed class UploadThumbnailContext
    {
        public Guid CourseId { get; set; }

        public Guid OwnerUserId { get; set; }

        public string? OwnerUserEmail { get; set; }

        public string CourseName { get; set; } = null!;

        public string? ExistingThumbnailName { get; set; }
    }
}
