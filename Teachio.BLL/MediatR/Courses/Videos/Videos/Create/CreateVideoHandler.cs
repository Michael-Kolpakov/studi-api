using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Localization;
using Teachio.BLL.Dto.Courses.Videos.Videos.Response;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResource;
using Teachio.DAL.Repositories.Interfaces.Base;
using VideoEntity = Teachio.DAL.Entities.Courses.Videos.Videos.Video;
using VideoProgressEntity = Teachio.DAL.Entities.Courses.Videos.VideoProgress.VideoProgress;

namespace Teachio.BLL.MediatR.Courses.Videos.Videos.Create;

public class CreateVideoHandler : IRequestHandler<CreateVideoCommand, Result<VideoResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IEntityExistenceService _entityExistenceService;
    private readonly ILoggerService _logger;
    private readonly IStringLocalizer<CannotMapSharedResource> _stringLocalizerFailedToMap;
    private readonly IStringLocalizer<NoPermissionsSharedResource> _stringLocalizerNoPermissions;
    private readonly IStringLocalizer<AlreadyExistsSharedResource> _stringLocalizerAlreadyExists;

    public CreateVideoHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IEntityExistenceService entityExistenceService,
        ILoggerService logger,
        IStringLocalizer<CannotMapSharedResource> stringLocalizerFailedToMap,
        IStringLocalizer<NoPermissionsSharedResource> stringLocalizerNoPermissions,
        IStringLocalizer<AlreadyExistsSharedResource> stringLocalizerAlreadyExists)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _entityExistenceService = entityExistenceService;
        _logger = logger;
        _stringLocalizerFailedToMap = stringLocalizerFailedToMap;
        _stringLocalizerNoPermissions = stringLocalizerNoPermissions;
        _stringLocalizerAlreadyExists = stringLocalizerAlreadyExists;
    }

    public async Task<Result<VideoResponseDto>> Handle(CreateVideoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Entered '{GetType().Name}' to create a new video");

        // TODO: validate whether OwnerUserId really belongs to the user making the request

        // TODO: validate whether video file exists (database relationships and ownership)

        var newVideo = _mapper.Map<VideoEntity>(request.videoCreateRequestDto);

        if (newVideo is null)
        {
            var errorMessage = _stringLocalizerFailedToMap[nameof(CannotMapSharedResource_en.CannotMapNullToVideo)].Value;
            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var (section, existenceErrorMessage) = await _entityExistenceService.CheckSectionExistenceAsync(
            request.videoCreateRequestDto.SectionId,
            nameof(request.videoCreateRequestDto.SectionId),
            cancellationToken);

        if (section is null)
        {
            _logger.LogError(request, existenceErrorMessage!);

            return Result.Fail(existenceErrorMessage);
        }

        if (section.Course!.OwnerUserId != request.requestingUserId)
        {
            var logErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToCreateVideoForSectionOfCourseOfAnotherUserWithId),
                request.videoCreateRequestDto.SectionId,
                section.CourseId,
                request.requestingUserId,
                section.Course.OwnerUserId
            ].Value;

            _logger.LogError(request, logErrorMessage);

            var responseErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToCreateVideoForSectionOfCourseOfAnotherUser)
            ].Value;

            return Result.Fail(responseErrorMessage);
        }

        var videoWithSameOrderIndexExists = await _repositoryWrapper.VideosRepository.GetSingleOrDefaultAsync(
            v => v.SectionId == newVideo.SectionId && v.OrderIndex == newVideo.OrderIndex,
            cancellationToken: cancellationToken);

        if (videoWithSameOrderIndexExists is not null)
        {
            var errorMessage = _stringLocalizerAlreadyExists[
                nameof(AlreadyExistsSharedResource_en.VideoAlreadyExistsForSection),
                videoWithSameOrderIndexExists.Id,
                videoWithSameOrderIndexExists.SectionId,
                videoWithSameOrderIndexExists.OrderIndex
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var newVideoProgress = new VideoProgressEntity()
        {
            Video = newVideo
        };

        // TODO: validate whether video was created successfully, if not - address Google Drive API (or CDN in the future) to delete video file

        await _repositoryWrapper.VideosRepository.CreateAsync(newVideo, cancellationToken);
        await _repositoryWrapper.VideoProgressRepository.CreateAsync(newVideoProgress, cancellationToken);
        await _repositoryWrapper.SaveChangesAsync(cancellationToken);

        var videoResponseDto = _mapper.Map<VideoResponseDto>(newVideo);

        return Result.Ok(videoResponseDto);
    }
}
