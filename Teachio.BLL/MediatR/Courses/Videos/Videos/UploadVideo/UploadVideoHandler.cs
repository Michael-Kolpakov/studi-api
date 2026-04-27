using System.Diagnostics.CodeAnalysis;
using System.Text;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Localization;
using Teachio.BLL.Dto.Courses.Videos.Videos.Response;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResource;
using Teachio.BLL.Utils.Helpers;
using Teachio.BLL.Utils.MappingResolvers;
using Teachio.DAL.Entities.Courses.Videos.VideoFiles;
using Teachio.DAL.Entities.Courses.Videos.Videos;
using Teachio.DAL.Repositories.Interfaces.Base;
using Teachio.DAL.Utils.Constants;
using VideoEntity = Teachio.DAL.Entities.Courses.Videos.Videos.Video;

namespace Teachio.BLL.MediatR.Courses.Videos.Videos.UploadVideo;

public class UploadVideoHandler : IRequestHandler<UploadVideoCommand, Result<VideoResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IGoogleDriveStorageService _googleDriveStorageService;
    private readonly IVideoMetadataService _videoMetadataService;
    private readonly ILoggerService _logger;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;
    private readonly IStringLocalizer<NoPermissionsSharedResource> _stringLocalizerNoPermissions;
    private readonly IStringLocalizer<VideoUploadSharedResource> _stringLocalizerVideoUpload;

    public UploadVideoHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IGoogleDriveStorageService googleDriveStorageService,
        IVideoMetadataService videoMetadataService,
        ILoggerService logger,
        IStringLocalizer<CannotFindSharedResource> stringLocalizerCannotFind,
        IStringLocalizer<NoPermissionsSharedResource> stringLocalizerNoPermissions,
        IStringLocalizer<VideoUploadSharedResource> stringLocalizerVideoUpload)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _googleDriveStorageService = googleDriveStorageService;
        _videoMetadataService = videoMetadataService;
        _logger = logger;
        _stringLocalizerCannotFind = stringLocalizerCannotFind;
        _stringLocalizerNoPermissions = stringLocalizerNoPermissions;
        _stringLocalizerVideoUpload = stringLocalizerVideoUpload;
    }

    public async Task<Result<VideoResponseDto>> Handle(UploadVideoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Entered '{GetType().Name}' to upload a media file for video with Id: {request.VideoUploadRequestDto.VideoId}");

        if (request.VideoUploadRequestDto.VideoFile.Length == 0)
        {
            var errorMessage = _stringLocalizerVideoUpload[
                nameof(VideoUploadSharedResource_en.UploadedVideoFileIsEmpty)
            ].Value;
            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        if (request.VideoUploadRequestDto.VideoFile.Length > EntityConstants.MaxVideoFileSizeBytes)
        {
            var maxVideoFileSizeMegabytes = EntityConstants.MaxVideoFileSizeBytes / (1024L * 1024L);
            var errorMessage = _stringLocalizerVideoUpload[
                nameof(VideoUploadSharedResource_en.UploadedVideoFileSizeExceedsLimit),
                maxVideoFileSizeMegabytes
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var uploadedFileTitle = Path.GetFileNameWithoutExtension(request.VideoUploadRequestDto.VideoFile.FileName);

        if (string.IsNullOrWhiteSpace(uploadedFileTitle))
        {
            var errorMessage = _stringLocalizerVideoUpload[
                nameof(VideoUploadSharedResource_en.UploadedVideoFileNameIsEmpty)
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var normalizedFileTitle = NormalizeVideoTitle(uploadedFileTitle);

        if (string.IsNullOrWhiteSpace(normalizedFileTitle))
        {
            var errorMessage = _stringLocalizerVideoUpload[
                nameof(VideoUploadSharedResource_en.UploadedVideoFileTitleIsEmpty)
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        if (normalizedFileTitle.Length > EntityConstants.MaxVideoTitleLength)
        {
            var errorMessage = _stringLocalizerVideoUpload[
                nameof(VideoUploadSharedResource_en.UploadedVideoFileTitleTooLong),
                EntityConstants.MaxVideoTitleLength
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var uploadVideoContext = await _repositoryWrapper.VideosRepository.GetSingleOrDefaultProjectedAsync(
            x => new UploadVideoContext
            {
                VideoId = x.Id,
                OwnerUserId = x.Section!.Course!.OwnerUserId,
                OwnerUserEmail = x.Section.Course.OwnerUser!.Email,
                CourseName = x.Section.Course.CourseName,
                SectionName = x.Section.SectionName,
                ExistingVideoFileName = x.VideoFile!.VideoName
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

        if (uploadVideoContext.OwnerUserId != request.RequestingUserId)
        {
            var logErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToUpdateVideoForUserWithId),
                request.VideoUploadRequestDto.VideoId,
                request.RequestingUserId
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

            var videoName = BuildVideoName(normalizedFileTitle, metadataResult.Value.FileExtension);

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
                    VideoStoragePathHelper.BuildVideoFolderSegments(
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
                VideoStoragePathHelper.BuildVideoFolderSegments(
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

            if (existingVideoFileEntity is null)
            {
                var newVideoFileEntity = new VideoFile
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

            video.Status = VideoStatus.Ready;
            video.ProcessingError = null;

            _repositoryWrapper.VideosRepository.Update(video);
            await _repositoryWrapper.SaveChangesAsync(cancellationToken);

            var updatedVideo = await _repositoryWrapper.VideosRepository.GetSingleOrDefaultAsync(
                x => x.Id == request.VideoUploadRequestDto.VideoId,
                IncludeVideoResponseRelatedEntities,
                cancellationToken);

            if (updatedVideo is null)
            {
                var errorMessage = _stringLocalizerCannotFind[
                    nameof(CannotFindSharedResource_en.CannotFindVideoById),
                    request.VideoUploadRequestDto.VideoId
                ].Value;

                _logger.LogError(request, errorMessage);

                return Result.Fail(errorMessage);
            }

            var responseDto = _mapper.Map<VideoResponseDto>(updatedVideo);

            return Result.Ok(responseDto);
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

    [ExcludeFromCodeCoverage]
    private static IIncludableQueryable<VideoEntity, object> IncludeVideoResponseRelatedEntities(IQueryable<VideoEntity> query)
    {
        return query
            .Include(v => v.Section)
                .ThenInclude(s => s!.Course)
                    .ThenInclude(c => c!.OwnerUser)
            .Include(v => v.VideoProgress)
            .Include(v => v.VideoFile!);
    }

    private static string CreateTemporaryFilePath(string sourceFileName)
    {
        var extension = Path.GetExtension(sourceFileName);

        return Path.Combine(Path.GetTempPath(), $"teachio-video-{Guid.NewGuid():N}{extension}");
    }

    private static string BuildVideoName(string videoTitle, string fileExtension)
    {
        var normalizedName = NameFromTitleResolver.CreateNameFromTitle(videoTitle);

        return $"{normalizedName}.{fileExtension}";
    }

    private static string NormalizeVideoTitle(string rawTitle)
    {
        var builder = new StringBuilder(rawTitle.Length);

        foreach (var character in rawTitle)
        {
            if (IsAsciiLetterOrDigit(character)
                || character == ' '
                || character == ','
                || character == '!'
                || character == '?'
                || character == '-')
            {
                builder.Append(character);
            }
            else
            {
                builder.Append(' ');
            }
        }

        return string.Join(" ", builder.ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }

    private static bool IsAsciiLetterOrDigit(char character)
    {
         return character is (>= 'A' and <= 'Z')
             or (>= 'a' and <= 'z')
             or (>= '0' and <= '9');
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

        public string? ExistingVideoFileName { get; set; }
    }
}
