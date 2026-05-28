using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Studi.BLL.DTOs.Courses.Videos.Videos.Response;
using Studi.BLL.Resources.SharedResource;
using Studi.BLL.Services.Interfaces;
using Studi.BLL.SharedResource;
using Studi.BLL.Utils.Helpers;
using Studi.BLL.Utils.MappingResolvers;
using Studi.DAL.Entities.Courses.Videos.VideoFiles;
using Studi.DAL.Repositories.Interfaces.Base;
using Studi.DAL.Utils.Constants;

namespace Studi.BLL.CQRS.Courses.Videos.Videos.UploadVideo;

public class UploadVideoHandler : IRequestHandler<UploadVideoCommand, Result<VideoUploadResponseDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IGoogleDriveStorageService _googleDriveStorageService;
    private readonly IVideoMetadataService _videoMetadataService;
    private readonly ILoggerService _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;
    private readonly IStringLocalizer<NoPermissionsSharedResource> _stringLocalizerNoPermissions;
    private readonly IStringLocalizer<VideoUploadSharedResource> _stringLocalizerVideoUpload;

    public UploadVideoHandler(
        IRepositoryWrapper repositoryWrapper,
        IGoogleDriveStorageService googleDriveStorageService,
        IVideoMetadataService videoMetadataService,
        ILoggerService logger,
        ICurrentUserService currentUserService,
        IStringLocalizer<CannotFindSharedResource> stringLocalizerCannotFind,
        IStringLocalizer<NoPermissionsSharedResource> stringLocalizerNoPermissions,
        IStringLocalizer<VideoUploadSharedResource> stringLocalizerVideoUpload)
    {
        _repositoryWrapper = repositoryWrapper;
        _googleDriveStorageService = googleDriveStorageService;
        _videoMetadataService = videoMetadataService;
        _logger = logger;
        _currentUserService = currentUserService;
        _stringLocalizerCannotFind = stringLocalizerCannotFind;
        _stringLocalizerNoPermissions = stringLocalizerNoPermissions;
        _stringLocalizerVideoUpload = stringLocalizerVideoUpload;
    }

    public async Task<Result<VideoUploadResponseDto>> Handle(UploadVideoCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        _logger.LogInformation($"Entered '{GetType().Name}' to upload a media file for video with Id: {request.VideoUploadRequestDto.VideoId} by UserId: {userId}");

        var uploadVideoContext = await _repositoryWrapper.VideosRepository.GetSingleOrDefaultProjectedAsync(
            x => new UploadVideoContext
            {
                VideoId = x.Id,
                OwnerUserId = x.Section!.Course!.OwnerUserId,
                OwnerUserEmail = x.Section.Course.OwnerUser!.Email,
                CourseName = x.Section.Course.CourseName,
                SectionName = x.Section.SectionName,
                Title = x.Title,
                ExistingVideoFileName = x.VideoFile == null
                    ? null
                    : x.VideoFile.VideoName
            },
            x => x.Id == request.VideoUploadRequestDto.VideoId,
            cancellationToken);

        if (uploadVideoContext is null)
        {
            var errorMessage = _stringLocalizerCannotFind[
                nameof(CannotFindSharedResource_en.CannotFindVideoById),
                request.VideoUploadRequestDto.VideoId
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        if (uploadVideoContext.OwnerUserId != userId)
        {
            var logErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToUpdateVideoForUserWithId),
                request.VideoUploadRequestDto.VideoId,
                userId
            ].Value;

            _logger.LogError(request, logErrorMessage);

            var responseErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToUpdateVideoForUser),
                request.VideoUploadRequestDto.VideoId
            ].Value;

            return Result.Fail(responseErrorMessage);
        }

        var temporaryFilePath = CreateTemporaryFilePath(request.VideoUploadRequestDto.VideoFile.FileName);
        string? uploadedGoogleDriveFileId = null;

        try
        {
            await SaveUploadedFileToTemporaryStorageAsync(
                request.VideoUploadRequestDto.VideoFile,
                temporaryFilePath,
                cancellationToken);

            var metadataResult = await _videoMetadataService.GetMetadataAsync(temporaryFilePath, cancellationToken);

            if (metadataResult.IsFailed)
            {
                var errorMessage = metadataResult.Errors[0].Message;
                _logger.LogError(request, errorMessage);

                return Result.Fail(errorMessage);
            }

            var videoName = BuildVideoName(uploadVideoContext.Title, metadataResult.Value.FileExtension);

            if (videoName.Length > EntityConstants.MaxVideoFileNameLength)
            {
                var errorMessage = _stringLocalizerVideoUpload[
                    nameof(VideoUploadSharedResource_en.GeneratedVideoFileNameTooLong),
                    EntityConstants.MaxVideoFileNameLength
                ].Value;
                _logger.LogError(request, errorMessage);

                return Result.Fail(errorMessage);
            }

            if (!string.IsNullOrWhiteSpace(uploadVideoContext.ExistingVideoFileName))
            {
                var deleteExistingFileResult = await _googleDriveStorageService.DeleteFileByPathAsync(
                    StoragePathHelper.BuildSectionFolderSegments(
                        uploadVideoContext.OwnerUserEmail!,
                        uploadVideoContext.CourseName,
                        uploadVideoContext.SectionName),
                    uploadVideoContext.ExistingVideoFileName,
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
                StoragePathHelper.BuildSectionFolderSegments(
                    uploadVideoContext.OwnerUserEmail!,
                    uploadVideoContext.CourseName,
                    uploadVideoContext.SectionName),
                videoName,
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

            var video = await _repositoryWrapper.VideosRepository.GetSingleOrDefaultAsync(
                x => x.Id == request.VideoUploadRequestDto.VideoId,
                include: q => q.Include(x => x.VideoProgresses),
                cancellationToken: cancellationToken);

            if (video is null)
            {
                var errorMessage = _stringLocalizerCannotFind[
                    nameof(CannotFindSharedResource_en.CannotFindVideoById),
                    request.VideoUploadRequestDto.VideoId
                ].Value;

                _logger.LogError(request, errorMessage);

                return Result.Fail(errorMessage);
            }

            var existingVideoFileEntity = await _repositoryWrapper.VideoFilesRepository.GetSingleOrDefaultAsync(
                x => x.VideoId == request.VideoUploadRequestDto.VideoId,
                cancellationToken: cancellationToken);

            VideoFile? newVideoFileEntity = null;
            if (existingVideoFileEntity is null)
            {
                newVideoFileEntity = new VideoFile
                {
                    VideoId = video.Id,
                    VideoName = videoName,
                    ContentType = metadataResult.Value.ContentType,
                    DurationSeconds = metadataResult.Value.DurationSeconds,
                    Resolution = metadataResult.Value.Resolution
                };

                await _repositoryWrapper.VideoFilesRepository.CreateAsync(newVideoFileEntity, cancellationToken);
            }
            else
            {
                existingVideoFileEntity.VideoName = videoName;
                existingVideoFileEntity.ContentType = metadataResult.Value.ContentType;
                existingVideoFileEntity.DurationSeconds = metadataResult.Value.DurationSeconds;
                existingVideoFileEntity.Resolution = metadataResult.Value.Resolution;

                _repositoryWrapper.VideoFilesRepository.Update(existingVideoFileEntity);
            }

            if (video.VideoProgresses is not null)
            {
                foreach (var vp in video.VideoProgresses)
                {
                    vp.PositionSeconds = 0;
                }
            }

            _repositoryWrapper.VideosRepository.Update(video);
            await _repositoryWrapper.SaveChangesAsync(cancellationToken);

            var videoUploadResponseDto = new VideoUploadResponseDto
            {
                Id = existingVideoFileEntity is not null ? existingVideoFileEntity.Id : newVideoFileEntity!.Id,
                VideoName = videoName,
                ContentType = metadataResult.Value.ContentType,
                DurationSeconds = metadataResult.Value.DurationSeconds,
                Resolution = metadataResult.Value.Resolution,
                VideoId = video.Id
            };

            return Result.Ok(videoUploadResponseDto);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            var errorMessage = _stringLocalizerVideoUpload[
                nameof(VideoUploadSharedResource_en.UploadVideoProcessingFailed)
            ].Value;
            _logger.LogError(request, errorMessage, ex.ToString());

            if (!string.IsNullOrWhiteSpace(uploadedGoogleDriveFileId))
            {
                var rollbackResult = await _googleDriveStorageService.DeleteFileAsync(uploadedGoogleDriveFileId, cancellationToken);

                if (rollbackResult.IsFailed)
                {
                    var rollbackErrorMessage = _stringLocalizerVideoUpload[
                        nameof(VideoUploadSharedResource_en.UploadVideoRollbackFailed),
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

        return Path.Combine(Path.GetTempPath(), $"studi-video-{Guid.NewGuid():N}{extension}");
    }

    private static string BuildVideoName(string videoTitle, string fileExtension)
    {
        var normalizedName = NameFromTitleResolver.CreateNameFromTitle(videoTitle);

        return $"{normalizedName}.{fileExtension}";
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

    private sealed class UploadVideoContext
    {
        public Guid VideoId { get; set; }

        public Guid OwnerUserId { get; set; }

        public string? OwnerUserEmail { get; set; }

        public string CourseName { get; set; } = null!;

        public string SectionName { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string? ExistingVideoFileName { get; set; }
    }
}
