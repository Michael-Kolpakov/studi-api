using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Localization;
using Teachio.BLL.DTOs.Courses.Videos.Videos.Response;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResource;
using Teachio.BLL.Utils.Helpers;
using Teachio.DAL.Entities.Courses.Videos.Videos;
using Teachio.DAL.Repositories.Interfaces.Base;

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
    private readonly IStringLocalizer<BllSharedResource> _stringLocalizerBll;

    public CreateVideoHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IEntityExistenceService entityExistenceService,
        ILoggerService logger,
        ICurrentUserService currentUserService,
        IStringLocalizer<CannotMapSharedResource> stringLocalizerFailedToMap,
        IStringLocalizer<NoPermissionsSharedResource> stringLocalizerNoPermissions,
        IStringLocalizer<BllSharedResource> stringLocalizerBll)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _entityExistenceService = entityExistenceService;
        _logger = logger;
        _currentUserService = currentUserService;
        _stringLocalizerFailedToMap = stringLocalizerFailedToMap;
        _stringLocalizerNoPermissions = stringLocalizerNoPermissions;
        _stringLocalizerBll = stringLocalizerBll;
    }

    public async Task<Result<VideoResponseDto>> Handle(CreateVideoCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        _logger.LogInformation($"Entered '{GetType().Name}' to create a new video by UserId: {userId}");

        var newVideo = _mapper.Map<Video>(request.VideoCreateRequestDto);

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

        var sectionVideosCount = await _repositoryWrapper.VideosRepository.GetSelfCountAsync(
            v => v.SectionId == newVideo.SectionId,
            cancellationToken: cancellationToken);

        if (sectionVideosCount >= DAL.Utils.Constants.EntityConstants.MaxVideosPerSection)
        {
            var limitErrorMessage = _stringLocalizerBll[
                nameof(BllSharedResource_en.VideosCountExceedsLimit),
                newVideo.SectionId,
                DAL.Utils.Constants.EntityConstants.MaxVideosPerSection
            ].Value;

            _logger.LogError(request, limitErrorMessage);

            return Result.Fail(limitErrorMessage);
        }

        newVideo.OrderIndex = sectionVideosCount;

        await _repositoryWrapper.VideosRepository.CreateAsync(newVideo, cancellationToken);

        var enrolledUserIds = await _repositoryWrapper.UserCoursesRepository.GetProjectedListAsync(
            uc => uc.AppUserId,
            uc => uc.CourseId == section.CourseId,
            cancellationToken);

        var progressItems = VideoProgressHelper.CreateForVideoAndUsers(newVideo.Id, enrolledUserIds);

        if (progressItems.Count > 0)
        {
            await _repositoryWrapper.VideoProgressRepository.CreateRangeAsync(progressItems, cancellationToken);
        }

        section.VideosCount = sectionVideosCount + 1;

        _repositoryWrapper.SectionsRepository.Update(section);
        await _repositoryWrapper.SaveChangesAsync(cancellationToken);

        var videoResponseDto = _mapper.Map<VideoResponseDto>(newVideo);

        return Result.Ok(videoResponseDto);
    }
}
