using FluentResults;
using MediatR;
using Microsoft.Extensions.Localization;
using Teachio.BLL.Models.Storage;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResource;
using Teachio.BLL.Utils.Constants;
using Teachio.BLL.Utils.Helpers;
using Teachio.DAL.Repositories.Interfaces.Base;

namespace Teachio.BLL.CQRS.Courses.Courses.StreamThumbnail;

public class StreamCourseThumbnailHandler : IRequestHandler<StreamCourseThumbnailQuery, Result<GoogleDriveStreamResult>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IGoogleDriveStorageService _googleDriveStorageService;
    private readonly ILoggerService _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;
    private readonly IStringLocalizer<ThumbnailStreamSharedResource> _stringLocalizerThumbnailStream;

    public StreamCourseThumbnailHandler(
        IRepositoryWrapper repositoryWrapper,
        IGoogleDriveStorageService googleDriveStorageService,
        ILoggerService logger,
        ICurrentUserService currentUserService,
        IStringLocalizer<CannotFindSharedResource> stringLocalizerCannotFind,
        IStringLocalizer<ThumbnailStreamSharedResource> stringLocalizerThumbnailStream)
    {
        _repositoryWrapper = repositoryWrapper;
        _googleDriveStorageService = googleDriveStorageService;
        _logger = logger;
        _currentUserService = currentUserService;
        _stringLocalizerCannotFind = stringLocalizerCannotFind;
        _stringLocalizerThumbnailStream = stringLocalizerThumbnailStream;
    }

    public async Task<Result<GoogleDriveStreamResult>> Handle(StreamCourseThumbnailQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        var rangeInfo = string.IsNullOrWhiteSpace(request.RangeHeader)
            ? string.Empty
            : $" with Range: {request.RangeHeader.Trim()}";

        _logger.LogInformation(
            $"Entered '{GetType().Name}' to stream course thumbnail for CourseId: {request.CourseId} by UserId: {userId}{rangeInfo}");

        var streamContext = await _repositoryWrapper.CoursesRepository.GetSingleOrDefaultProjectedAsync(
            x => new StreamThumbnailContext
            {
                ThumbnailName = x.ThumbnailFile == null ? null : x.ThumbnailFile.ThumbnailName,
                ThumbnailContentType = x.ThumbnailFile == null ? null : x.ThumbnailFile.ContentType,
                OwnerUserEmail = x.OwnerUser.Email,
                CourseName = x.CourseName
            },
            x => x.Id == request.CourseId,
            cancellationToken);

        if (streamContext is null)
        {
            var errorMessage = _stringLocalizerCannotFind[
                nameof(CannotFindSharedResource_en.CannotFindCourseById),
                request.CourseId
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var normalizedRangeHeader = string.IsNullOrWhiteSpace(request.RangeHeader)
            ? null
            : request.RangeHeader.Trim();

        if (string.IsNullOrWhiteSpace(streamContext.ThumbnailName))
        {
            return await StreamDefaultThumbnailAsync(normalizedRangeHeader, cancellationToken);
        }

        if (string.IsNullOrWhiteSpace(streamContext.OwnerUserEmail)
            || string.IsNullOrWhiteSpace(streamContext.CourseName))
        {
            var errorMessage = _stringLocalizerThumbnailStream[
                nameof(ThumbnailStreamSharedResource_en.ThumbnailFileNotAvailable)
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var downloadResult = await _googleDriveStorageService.OpenReadFileByPathAsync(
            StoragePathHelper.BuildThumbnailFolderSegments(
                streamContext.OwnerUserEmail,
                streamContext.CourseName),
            streamContext.ThumbnailName,
            normalizedRangeHeader,
            cancellationToken);

        if (downloadResult.IsFailed)
        {
            var errorMessage = downloadResult.Errors[0].Message;
            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var streamResult = downloadResult.Value;

        if (!string.IsNullOrWhiteSpace(streamContext.ThumbnailContentType))
        {
            streamResult.ContentType = streamContext.ThumbnailContentType;
        }

        return Result.Ok(streamResult);
    }

    private async Task<Result<GoogleDriveStreamResult>> StreamDefaultThumbnailAsync(
        string? rangeHeader,
        CancellationToken cancellationToken)
    {
        var downloadResult = await _googleDriveStorageService.OpenReadFileByPathAsync(
            StoragePathHelper.BuildDefaultThumbnailFolderSegments(),
            HandlerConstants.DefaultThumbnailFileName,
            rangeHeader,
            cancellationToken);

        if (downloadResult.IsFailed)
        {
            var errorMessage = downloadResult.Errors[0].Message;
            _logger.LogError(null, errorMessage);

            return Result.Fail(errorMessage);
        }

        var streamResult = downloadResult.Value;
        streamResult.ContentType = HandlerConstants.DefaultThumbnailContentType;

        return Result.Ok(streamResult);
    }

    private sealed class StreamThumbnailContext
    {
        public string? ThumbnailName { get; set; }

        public string? ThumbnailContentType { get; set; }

        public string? OwnerUserEmail { get; set; }

        public string? CourseName { get; set; }
    }
}
