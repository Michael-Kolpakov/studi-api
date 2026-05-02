using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Localization;
using Teachio.BLL.DTOs.Courses.Videos.Videos.Response;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResources;
using Teachio.BLL.Utils.Helpers;
using Teachio.DAL.Repositories.Interfaces.Base;
using VideoEntity = Teachio.DAL.Entities.Courses.Videos.Videos.Video;

namespace Teachio.BLL.CQRS.Courses.Videos.Videos.Update;

public class UpdateVideoHandler : IRequestHandler<UpdateVideoCommand, Result<VideoResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;
    private readonly IStringLocalizer<NoPermissionsSharedResource> _stringLocalizerNoPermissions;

    public UpdateVideoHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILoggerService logger,
        ICurrentUserService currentUserService,
        IStringLocalizer<CannotFindSharedResource> stringLocalizerCannotFind,
        IStringLocalizer<NoPermissionsSharedResource> stringLocalizerNoPermissions)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
        _currentUserService = currentUserService;
        _stringLocalizerCannotFind = stringLocalizerCannotFind;
        _stringLocalizerNoPermissions = stringLocalizerNoPermissions;
    }

    public async Task<Result<VideoResponseDto>> Handle(UpdateVideoCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        _logger.LogInformation($"Entered '{GetType().Name}' to update a video with Id: {request.VideoUpdateRequestDto.Id} by UserId: {userId}");

        var existingVideo = await _repositoryWrapper.VideosRepository.GetSingleOrDefaultAsync(
            x => x.Id == request.VideoUpdateRequestDto.Id,
            IncludeVideoRelatedEntities,
            cancellationToken: cancellationToken);

        if (existingVideo is null)
        {
            var errorMessage = _stringLocalizerCannotFind[
                nameof(CannotFindSharedResource_en.CannotFindVideoById),
                request.VideoUpdateRequestDto.Id
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var courseOwnerUserId = await _repositoryWrapper.VideosRepository.GetSingleOrDefaultProjectedAsync(
            v => v.Section!.Course!.OwnerUserId,
            v => v.Id == request.VideoUpdateRequestDto.Id,
            cancellationToken);

        if (courseOwnerUserId != userId)
        {
            var logErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToUpdateVideoForUserWithId),
                request.VideoUpdateRequestDto.Id,
                userId
            ].Value;

            _logger.LogError(request, logErrorMessage);

            var responseErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToUpdateVideoForUser),
                request.VideoUpdateRequestDto.Id
            ].Value;

            return Result.Fail(responseErrorMessage);
        }

        var sectionVideos = (await _repositoryWrapper.VideosRepository.GetAllAsync(
                v => v.SectionId == existingVideo.SectionId,
                cancellationToken: cancellationToken))
            .OrderBy(v => v.OrderIndex)
            .ToList();

        var targetOrderIndex = PrepareOrderIndexForUpdate(
            sectionVideos,
            existingVideo,
            request.VideoUpdateRequestDto.OrderIndex);

        // TODO: validate whether course was updated successfully, if YES - address Google Drive API (or CDN in the future) to delete old video file and set new one

        // TODO: validate whether course was updated successfully, if NO - address Google Drive API (or CDN in the future) to delete current video file

        _mapper.Map(request.VideoUpdateRequestDto, existingVideo);
        existingVideo.OrderIndex = targetOrderIndex;

        _repositoryWrapper.VideosRepository.UpdateRange(sectionVideos);
        await _repositoryWrapper.SaveChangesAsync(cancellationToken);

        var videoResponseDto = _mapper.Map<VideoResponseDto>(existingVideo);

        return Result.Ok(videoResponseDto);
    }

    [ExcludeFromCodeCoverage]
    private static IIncludableQueryable<VideoEntity, object> IncludeVideoRelatedEntities(IQueryable<VideoEntity> query)
    {
        return query
            .Include(v => v.VideoFile)
            .Include(v => v.VideoProgress);
    }

    private static int PrepareOrderIndexForUpdate(
        List<VideoEntity> sectionVideos,
        VideoEntity existingVideo,
        int requestedOrderIndex)
    {
        VideoOrderIndexHelper.NormalizeOrderIndexes(sectionVideos);

        var currentOrderIndex = existingVideo.OrderIndex;
        var targetOrderIndex = Math.Min(requestedOrderIndex, sectionVideos.Count - 1);

        if (targetOrderIndex < currentOrderIndex)
        {
            foreach (var video in sectionVideos.Where(v =>
                         v.Id != existingVideo.Id
                         && v.OrderIndex >= targetOrderIndex
                         && v.OrderIndex < currentOrderIndex))
            {
                video.OrderIndex++;
            }
        }
        else if (targetOrderIndex > currentOrderIndex)
        {
            foreach (var video in sectionVideos.Where(v =>
                         v.Id != existingVideo.Id
                         && v.OrderIndex <= targetOrderIndex
                         && v.OrderIndex > currentOrderIndex))
            {
                video.OrderIndex--;
            }
        }

        return targetOrderIndex;
    }
}
