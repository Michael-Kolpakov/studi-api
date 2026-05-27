using FluentResults;
using MediatR;
using Microsoft.Extensions.Localization;
using Studi.BLL.Models.Storage;
using Studi.BLL.Resources.SharedResource;
using Studi.BLL.Services.Interfaces;
using Studi.BLL.SharedResource;
using Studi.BLL.Utils.Constants;
using Studi.BLL.Utils.Helpers;
using Studi.DAL.Repositories.Interfaces.Base;

namespace Studi.BLL.CQRS.Users.Account.StreamAvatar;

public class StreamAvatarHandler : IRequestHandler<StreamAvatarQuery, Result<GoogleDriveStreamResult>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IGoogleDriveStorageService _googleDriveStorageService;
    private readonly ILoggerService _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;
    private readonly IStringLocalizer<GoogleDriveStorageSharedResource> _stringLocalizerGoogleDriveStorage;

    public StreamAvatarHandler(
        IRepositoryWrapper repositoryWrapper,
        IGoogleDriveStorageService googleDriveStorageService,
        ILoggerService logger,
        ICurrentUserService currentUserService,
        IStringLocalizer<CannotFindSharedResource> stringLocalizerCannotFind,
        IStringLocalizer<GoogleDriveStorageSharedResource> stringLocalizerGoogleDriveStorage)
    {
        _repositoryWrapper = repositoryWrapper;
        _googleDriveStorageService = googleDriveStorageService;
        _logger = logger;
        _currentUserService = currentUserService;
        _stringLocalizerCannotFind = stringLocalizerCannotFind;
        _stringLocalizerGoogleDriveStorage = stringLocalizerGoogleDriveStorage;
    }

    public async Task<Result<GoogleDriveStreamResult>> Handle(StreamAvatarQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        var rangeInfo = string.IsNullOrWhiteSpace(request.RangeHeader)
            ? string.Empty
            : $" with Range: {request.RangeHeader.Trim()}";
        var targetInfo = request.CourseId.HasValue
            ? $" for CourseId: {request.CourseId.Value}"
            : $" for UserId: {userId}";

        _logger.LogInformation($"Entered '{GetType().Name}' to stream avatar{targetInfo}{rangeInfo}");

        var streamContextResult = request.CourseId.HasValue
            ? await GetCourseOwnerStreamContextAsync(request.CourseId.Value, request, cancellationToken)
            : await GetCurrentUserStreamContextAsync(userId, request, cancellationToken);

        if (streamContextResult.IsFailed)
        {
            return Result.Fail(streamContextResult.Errors);
        }

        var streamContext = streamContextResult.Value;

        var normalizedRangeHeader = string.IsNullOrWhiteSpace(request.RangeHeader)
            ? null
            : request.RangeHeader.Trim();

        if (string.IsNullOrWhiteSpace(streamContext.AvatarName))
        {
            return await StreamDefaultAvatarAsync(normalizedRangeHeader, cancellationToken);
        }

        var downloadResult = await _googleDriveStorageService.OpenReadFileByPathAsync(
            StoragePathHelper.BuildUserFolderSegments(streamContext.OwnerUserEmail!),
            streamContext.AvatarName,
            normalizedRangeHeader,
            cancellationToken);

        if (downloadResult.IsFailed)
        {
            var errorMessage = downloadResult.Errors[0].Message;

            if (IsMissingAvatarFileError(errorMessage, streamContext.AvatarName))
            {
                return await StreamDefaultAvatarAsync(normalizedRangeHeader, cancellationToken);
            }

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var streamResult = downloadResult.Value;

        if (!string.IsNullOrWhiteSpace(streamContext.AvatarContentType))
        {
            streamResult.ContentType = streamContext.AvatarContentType;
        }

        return Result.Ok(streamResult);
    }

    private async Task<Result<StreamAvatarContext>> GetCurrentUserStreamContextAsync(
        Guid userId,
        StreamAvatarQuery request,
        CancellationToken cancellationToken)
    {
        var streamContext = await _repositoryWrapper.AppUsersRepository.GetSingleOrDefaultProjectedAsync(
            x => new StreamAvatarContext
            {
                AvatarName = x.AvatarFile == null ? null : x.AvatarFile.AvatarName,
                AvatarContentType = x.AvatarFile == null ? null : x.AvatarFile.ContentType,
                OwnerUserEmail = x.Email
            },
            x => x.Id == userId,
            cancellationToken);

        if (streamContext is not null)
        {
            return Result.Ok(streamContext);
        }

        var errorMessage = _stringLocalizerCannotFind[
            nameof(CannotFindSharedResource_en.CannotFindUserById),
            userId
        ].Value;

        _logger.LogError(request, errorMessage);

        return Result.Fail(errorMessage);
    }

    private async Task<Result<StreamAvatarContext>> GetCourseOwnerStreamContextAsync(
        Guid courseId,
        StreamAvatarQuery request,
        CancellationToken cancellationToken)
    {
        var streamContext = await _repositoryWrapper.CoursesRepository.GetSingleOrDefaultProjectedAsync(
            x => new StreamAvatarContext
            {
                AvatarName = x.OwnerUser.AvatarFile == null ? null : x.OwnerUser.AvatarFile.AvatarName,
                AvatarContentType = x.OwnerUser.AvatarFile == null ? null : x.OwnerUser.AvatarFile.ContentType,
                OwnerUserEmail = x.OwnerUser.Email
            },
            x => x.Id == courseId,
            cancellationToken);

        if (streamContext is not null)
        {
            return Result.Ok(streamContext);
        }

        var errorMessage = _stringLocalizerCannotFind[
            nameof(CannotFindSharedResource_en.CannotFindCourseById),
            courseId
        ].Value;

        _logger.LogError(request, errorMessage);

        return Result.Fail(errorMessage);
    }

    private async Task<Result<GoogleDriveStreamResult>> StreamDefaultAvatarAsync(
        string? rangeHeader,
        CancellationToken cancellationToken)
    {
        var downloadResult = await _googleDriveStorageService.OpenReadFileByPathAsync(
            StoragePathHelper.BuildDefaultAvatarFolderSegments(),
            HandlerConstants.DefaultAvatarFileName,
            rangeHeader,
            cancellationToken);

        if (downloadResult.IsFailed)
        {
            var errorMessage = downloadResult.Errors[0].Message;
            _logger.LogError(null, errorMessage);

            return Result.Fail(errorMessage);
        }

        var streamResult = downloadResult.Value;
        streamResult.ContentType = HandlerConstants.DefaultAvatarContentType;

        return Result.Ok(streamResult);
    }

    private bool IsMissingAvatarFileError(string errorMessage, string fileName)
    {
        var expectedErrorMessage = _stringLocalizerGoogleDriveStorage[
            nameof(GoogleDriveStorageSharedResource_en.FileNotFoundByPath),
            fileName
        ].Value;

        return string.Equals(errorMessage, expectedErrorMessage, StringComparison.Ordinal);
    }

    private sealed class StreamAvatarContext
    {
        public string? AvatarName { get; set; }

        public string? AvatarContentType { get; set; }

        public string? OwnerUserEmail { get; set; }
    }
}
