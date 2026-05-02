using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Localization;
using Teachio.BLL.DTOs.Courses.Videos.Videos.Response;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResources;
using Teachio.BLL.Utils.Helpers;
using Teachio.DAL.Repositories.Interfaces.Base;
using VideoEntity = Teachio.DAL.Entities.Courses.Videos.Videos.Video;
using VideoProgressEntity = Teachio.DAL.Entities.Courses.Videos.VideoProgress.VideoProgress;

namespace Teachio.BLL.CQRS.Courses.Videos.Videos.Create;

public class CreateVideoHandler : IRequestHandler<CreateVideoCommand, Result<VideoResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IEntityExistenceService _entityExistenceService;
    private readonly ILoggerService _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<CannotMapSharedResource> _stringLocalizerFailedToMap;
    private readonly IStringLocalizer<NoPermissionsSharedResource> _stringLocalizerNoPermissions;

    public CreateVideoHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IEntityExistenceService entityExistenceService,
        ILoggerService logger,
        ICurrentUserService currentUserService,
        IStringLocalizer<CannotMapSharedResource> stringLocalizerFailedToMap,
        IStringLocalizer<NoPermissionsSharedResource> stringLocalizerNoPermissions)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _entityExistenceService = entityExistenceService;
        _logger = logger;
        _currentUserService = currentUserService;
        _stringLocalizerFailedToMap = stringLocalizerFailedToMap;
        _stringLocalizerNoPermissions = stringLocalizerNoPermissions;
    }

    public async Task<Result<VideoResponseDto>> Handle(CreateVideoCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        _logger.LogInformation($"Entered '{GetType().Name}' to create a new video by UserId: {userId}");

        // TODO: validate whether OwnerUserId really belongs to the user making the request

        // TODO: validate whether video file exists (database relationships and ownership)

        var newVideo = _mapper.Map<VideoEntity>(request.VideoCreateRequestDto);

        if (newVideo is null)
        {
            var errorMessage = _stringLocalizerFailedToMap[nameof(CannotMapSharedResource_en.CannotMapNullToVideo)].Value;
            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var (section, existenceErrorMessage) = await _entityExistenceService.CheckSectionExistenceAsync(
            request.VideoCreateRequestDto.SectionId,
            nameof(request.VideoCreateRequestDto.SectionId),
            cancellationToken);

        if (section is null)
        {
            _logger.LogError(request, existenceErrorMessage!);

            return Result.Fail(existenceErrorMessage);
        }

        if (section.Course!.OwnerUserId != userId)
        {
            var logErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToCreateVideoForSectionOfCourseOfAnotherUserWithId),
                request.VideoCreateRequestDto.SectionId,
                section.CourseId,
                userId,
                section.Course.OwnerUserId
            ].Value;

            _logger.LogError(request, logErrorMessage);

            var responseErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToCreateVideoForSectionOfCourseOfAnotherUser)
            ].Value;

            return Result.Fail(responseErrorMessage);
        }

        var sectionVideos = (await _repositoryWrapper.VideosRepository.GetAllAsync(
                v => v.SectionId == newVideo.SectionId,
                cancellationToken: cancellationToken))
            .OrderBy(v => v.OrderIndex)
            .ToList();

        var targetOrderIndex = PrepareOrderIndexForCreate(sectionVideos, request.VideoCreateRequestDto.OrderIndex);
        newVideo.OrderIndex = targetOrderIndex;

        var newVideoProgress = new VideoProgressEntity()
        {
            Video = newVideo
        };

        // TODO: validate whether video was created successfully, if not - address Google Drive API (or CDN in the future) to delete video file

        await _repositoryWrapper.VideosRepository.CreateAsync(newVideo, cancellationToken);
        await _repositoryWrapper.VideoProgressRepository.CreateAsync(newVideoProgress, cancellationToken);

        if (sectionVideos.Count > 0)
        {
            _repositoryWrapper.VideosRepository.UpdateRange(sectionVideos);
        }

        section.VideosCount = sectionVideos.Count + 1;

        _repositoryWrapper.SectionsRepository.Update(section);
        await _repositoryWrapper.SaveChangesAsync(cancellationToken);

        var videoResponseDto = _mapper.Map<VideoResponseDto>(newVideo);

        return Result.Ok(videoResponseDto);
    }

    private static int PrepareOrderIndexForCreate(List<VideoEntity> sectionVideos, int requestedOrderIndex)
    {
        OrderIndexHelper.NormalizeOrderIndexes(sectionVideos);

        var targetOrderIndex = Math.Min(requestedOrderIndex, sectionVideos.Count);

        foreach (var video in sectionVideos.Where(v => v.OrderIndex >= targetOrderIndex))
        {
            video.OrderIndex++;
        }

        return targetOrderIndex;
    }
}
