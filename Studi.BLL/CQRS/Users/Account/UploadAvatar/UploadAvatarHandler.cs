using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Studi.BLL.DTOs.Users.Account.Response;
using Studi.BLL.Resources.SharedResource;
using Studi.BLL.Services.Interfaces;
using Studi.BLL.SharedResource;
using Studi.BLL.Utils.Helpers;
using Studi.DAL.Entities.Users.AvatarFiles;
using Studi.DAL.Repositories.Interfaces.Base;
using Studi.DAL.Utils.Constants;

namespace Studi.BLL.CQRS.Users.Account.UploadAvatar;

public class UploadAvatarHandler : IRequestHandler<UploadAvatarCommand, Result<AvatarUploadResponseDto>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IGoogleDriveStorageService _googleDriveStorageService;
    private readonly IAvatarMetadataService _avatarMetadataService;
    private readonly ILoggerService _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;
    private readonly IStringLocalizer<AvatarUploadSharedResource> _stringLocalizerAvatarUpload;

    public UploadAvatarHandler(
        IRepositoryWrapper repositoryWrapper,
        IGoogleDriveStorageService googleDriveStorageService,
        IAvatarMetadataService avatarMetadataService,
        ILoggerService logger,
        ICurrentUserService currentUserService,
        IStringLocalizer<CannotFindSharedResource> stringLocalizerCannotFind,
        IStringLocalizer<AvatarUploadSharedResource> stringLocalizerAvatarUpload)
    {
        _repositoryWrapper = repositoryWrapper;
        _googleDriveStorageService = googleDriveStorageService;
        _avatarMetadataService = avatarMetadataService;
        _logger = logger;
        _currentUserService = currentUserService;
        _stringLocalizerCannotFind = stringLocalizerCannotFind;
        _stringLocalizerAvatarUpload = stringLocalizerAvatarUpload;
    }

    public async Task<Result<AvatarUploadResponseDto>> Handle(UploadAvatarCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        _logger.LogInformation($"Entered '{GetType().Name}' to upload avatar for UserId: {userId}");

        var uploadAvatarContext = await _repositoryWrapper.AppUsersRepository.GetSingleOrDefaultProjectedAsync(
            x => new UploadAvatarContext
            {
                AppUserId = x.Id,
                Name = x.Name,
                Surname = x.Surname,
                OwnerUserEmail = x.Email!,
                ExistingAvatarName = x.AvatarFile == null
                    ? null
                    : x.AvatarFile.AvatarName
            },
            x => x.Id == userId,
            cancellationToken);

        if (uploadAvatarContext is null)
        {
            var errorMessage = _stringLocalizerCannotFind[
                nameof(CannotFindSharedResource_en.CannotFindUserById),
                userId
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var temporaryFilePath = CreateTemporaryFilePath(request.AvatarUploadRequestDto.AvatarFile.FileName);
        string? uploadedGoogleDriveFileId = null;

        try
        {
            await SaveUploadedFileToTemporaryStorageAsync(
                request.AvatarUploadRequestDto.AvatarFile,
                temporaryFilePath,
                cancellationToken);

            var metadataResult = await _avatarMetadataService.GetMetadataAsync(temporaryFilePath, cancellationToken);

            if (metadataResult.IsFailed)
            {
                var errorMessage = metadataResult.Errors[0].Message;
                _logger.LogError(request, errorMessage);

                return Result.Fail(errorMessage);
            }

            var namePart = uploadAvatarContext.Name.ToLowerInvariant();
            var surnamePart = uploadAvatarContext.Surname.ToLowerInvariant();
            var avatarName = $"{namePart}-{surnamePart}.{metadataResult.Value.FileExtension}";

            if (avatarName.Length > EntityConstants.MaxAvatarFileNameLength)
            {
                var errorMessage = _stringLocalizerAvatarUpload[
                    nameof(AvatarUploadSharedResource_en.GeneratedAvatarFileNameTooLong),
                    EntityConstants.MaxAvatarFileNameLength
                ].Value;
                _logger.LogError(request, errorMessage);

                return Result.Fail(errorMessage);
            }

            if (!string.IsNullOrWhiteSpace(uploadAvatarContext.ExistingAvatarName))
            {
                var deleteExistingFileResult = await _googleDriveStorageService.DeleteFileByPathAsync(
                    StoragePathHelper.BuildUserFolderSegments(uploadAvatarContext.OwnerUserEmail),
                    uploadAvatarContext.ExistingAvatarName,
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
                StoragePathHelper.BuildUserFolderSegments(uploadAvatarContext.OwnerUserEmail),
                avatarName,
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

            var user = await _repositoryWrapper.AppUsersRepository.GetSingleOrDefaultAsync(
                x => x.Id == uploadAvatarContext.AppUserId,
                cancellationToken: cancellationToken);

            if (user is null)
            {
                var errorMessage = _stringLocalizerCannotFind[
                    nameof(CannotFindSharedResource_en.CannotFindUserById),
                    uploadAvatarContext.AppUserId
                ].Value;

                _logger.LogError(request, errorMessage);

                return Result.Fail(errorMessage);
            }

            var existingAvatarFileEntity = await _repositoryWrapper.AvatarFilesRepository.GetSingleOrDefaultAsync(
                x => x.AppUserId == uploadAvatarContext.AppUserId,
                cancellationToken: cancellationToken);

            AvatarFile? newAvatarFileEntity = null;
            if (existingAvatarFileEntity is null)
            {
                newAvatarFileEntity = new AvatarFile
                {
                    AppUserId = user.Id,
                    AvatarName = avatarName,
                    ContentType = metadataResult.Value.ContentType,
                    Resolution = metadataResult.Value.Resolution
                };

                await _repositoryWrapper.AvatarFilesRepository.CreateAsync(newAvatarFileEntity, cancellationToken);
            }
            else
            {
                existingAvatarFileEntity.AvatarName = avatarName;
                existingAvatarFileEntity.ContentType = metadataResult.Value.ContentType;
                existingAvatarFileEntity.Resolution = metadataResult.Value.Resolution;

                _repositoryWrapper.AvatarFilesRepository.Update(existingAvatarFileEntity);
            }

            await _repositoryWrapper.SaveChangesAsync(cancellationToken);

            var avatarUploadResponseDto = new AvatarUploadResponseDto
            {
                Id = existingAvatarFileEntity is not null ? existingAvatarFileEntity.Id : newAvatarFileEntity!.Id,
                AvatarName = avatarName,
                ContentType = metadataResult.Value.ContentType,
                Resolution = metadataResult.Value.Resolution,
                AppUserId = user.Id
            };

            return Result.Ok(avatarUploadResponseDto);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            var errorMessage = _stringLocalizerAvatarUpload[
                nameof(AvatarUploadSharedResource_en.UploadAvatarProcessingFailed)
            ].Value;
            _logger.LogError(request, errorMessage, ex.ToString());

            if (!string.IsNullOrWhiteSpace(uploadedGoogleDriveFileId))
            {
                var rollbackResult = await _googleDriveStorageService.DeleteFileAsync(uploadedGoogleDriveFileId, cancellationToken);

                if (rollbackResult.IsFailed)
                {
                    var rollbackErrorMessage = _stringLocalizerAvatarUpload[
                        nameof(AvatarUploadSharedResource_en.UploadAvatarRollbackFailed),
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

        return Path.Combine(Path.GetTempPath(), $"studi-avatar-{Guid.NewGuid():N}{extension}");
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

    private sealed class UploadAvatarContext
    {
        public Guid AppUserId { get; set; }

        public string Name { get; set; } = null!;

        public string Surname { get; set; } = null!;

        public string OwnerUserEmail { get; set; } = null!;

        public string? ExistingAvatarName { get; set; }
    }
}
