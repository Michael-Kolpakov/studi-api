using FluentResults;
using MediatR;
using Microsoft.Extensions.Localization;
using Teachio.BLL.Models.Storage;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResource;
using Teachio.BLL.Utils.Helpers;
using Teachio.DAL.Repositories.Interfaces.Base;

namespace Teachio.BLL.CQRS.Courses.Videos.Videos.Stream;

public class StreamVideoHandler : IRequestHandler<StreamVideoQuery, Result<GoogleDriveStreamResult>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IGoogleDriveStorageService _googleDriveStorageService;
    private readonly ILoggerService _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICourseAccessService _courseAccessService;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;
    private readonly IStringLocalizer<NoPermissionsSharedResource> _stringLocalizerNoPermissions;
    private readonly IStringLocalizer<VideoStreamSharedResource> _stringLocalizerVideoStream;

    public StreamVideoHandler(
        IRepositoryWrapper repositoryWrapper,
        IGoogleDriveStorageService googleDriveStorageService,
        ILoggerService logger,
        ICurrentUserService currentUserService,
        ICourseAccessService courseAccessService,
        IStringLocalizer<CannotFindSharedResource> stringLocalizerCannotFind,
        IStringLocalizer<NoPermissionsSharedResource> stringLocalizerNoPermissions,
        IStringLocalizer<VideoStreamSharedResource> stringLocalizerVideoStream)
    {
        _repositoryWrapper = repositoryWrapper;
        _googleDriveStorageService = googleDriveStorageService;
        _logger = logger;
        _currentUserService = currentUserService;
        _courseAccessService = courseAccessService;
        _stringLocalizerCannotFind = stringLocalizerCannotFind;
        _stringLocalizerNoPermissions = stringLocalizerNoPermissions;
        _stringLocalizerVideoStream = stringLocalizerVideoStream;
    }

    public async Task<Result<GoogleDriveStreamResult>> Handle(StreamVideoQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        var rangeInfo = string.IsNullOrWhiteSpace(request.RangeHeader)
            ? string.Empty
            : $" with Range: {request.RangeHeader.Trim()}";

        _logger.LogInformation(
            $"Entered '{GetType().Name}' to stream video with Id: {request.VideoId} by UserId: {userId}{rangeInfo}");

        var streamContext = await _repositoryWrapper.VideosRepository.GetSingleOrDefaultProjectedAsync(
            x => new StreamVideoContext
            {
                VideoName = x.VideoFile == null ? null : x.VideoFile.VideoName,
                VideoContentType = x.VideoFile == null ? null : x.VideoFile.ContentType,
                OwnerUserEmail = x.Section!.Course!.OwnerUser!.Email,
                CourseName = x.Section.Course.CourseName,
                SectionName = x.Section.SectionName
            },
            x => x.Id == request.VideoId,
            cancellationToken);

        if (streamContext is null)
        {
            var errorMessage = _stringLocalizerCannotFind[
                nameof(CannotFindSharedResource_en.CannotFindVideoById),
                request.VideoId
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var hasAccess = await _courseAccessService.HasAccessToVideoAsync(request.VideoId, userId, cancellationToken);
        if (!hasAccess)
        {
            var errorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToGetVideoForUser),
                request.VideoId
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        if (string.IsNullOrWhiteSpace(streamContext.VideoName)
            || string.IsNullOrWhiteSpace(streamContext.OwnerUserEmail)
            || string.IsNullOrWhiteSpace(streamContext.CourseName)
            || string.IsNullOrWhiteSpace(streamContext.SectionName))
        {
            var errorMessage = _stringLocalizerVideoStream[
                nameof(VideoStreamSharedResource_en.VideoFileNotAvailable)
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var normalizedRangeHeader = string.IsNullOrWhiteSpace(request.RangeHeader)
            ? null
            : request.RangeHeader.Trim();

        var downloadResult = await _googleDriveStorageService.OpenReadFileByPathAsync(
            StoragePathHelper.BuildSectionFolderSegments(
                streamContext.OwnerUserEmail,
                streamContext.CourseName,
                streamContext.SectionName),
            streamContext.VideoName,
            normalizedRangeHeader,
            cancellationToken);

        if (downloadResult.IsFailed)
        {
            var errorMessage = downloadResult.Errors[0].Message;
            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var streamResult = downloadResult.Value;

        if (!string.IsNullOrWhiteSpace(streamContext.VideoContentType))
        {
            streamResult.ContentType = streamContext.VideoContentType;
        }

        return Result.Ok(streamResult);
    }

    private sealed class StreamVideoContext
    {
        public string? VideoName { get; set; }

        public string? VideoContentType { get; set; }

        public string? OwnerUserEmail { get; set; }

        public string? CourseName { get; set; }

        public string? SectionName { get; set; }
    }
}
