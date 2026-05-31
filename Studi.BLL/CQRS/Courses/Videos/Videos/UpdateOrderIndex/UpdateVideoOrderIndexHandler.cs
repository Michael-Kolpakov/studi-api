using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Localization;
using Studi.BLL.DTOs.Courses.Videos.Videos.Response;
using Studi.BLL.Resources.SharedResource;
using Studi.BLL.Services.Interfaces;
using Studi.BLL.SharedResource;
using Studi.BLL.Utils.Helpers;
using Studi.DAL.Entities.Courses.Videos.Videos;
using Studi.DAL.Repositories.Interfaces.Base;

namespace Studi.BLL.CQRS.Courses.Videos.Videos.UpdateOrderIndex;

public class UpdateVideoOrderIndexHandler : IRequestHandler<UpdateVideoOrderIndexCommand, Result<VideoResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;
    private readonly IStringLocalizer<NoPermissionsSharedResource> _stringLocalizerNoPermissions;

    public UpdateVideoOrderIndexHandler(
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

    public async Task<Result<VideoResponseDto>> Handle(UpdateVideoOrderIndexCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        _logger.LogInformation($"Entered '{GetType().Name}' to update a video order index with Id: {request.VideoUpdateOrderIndexRequestDto.Id} by UserId: {userId}");

        var existingVideo = await _repositoryWrapper.VideosRepository.GetSingleOrDefaultAsync(
            x => x.Id == request.VideoUpdateOrderIndexRequestDto.Id,
            cancellationToken: cancellationToken);

        if (existingVideo is null)
        {
            var errorMessage = _stringLocalizerCannotFind[
                nameof(CannotFindSharedResource_en.CannotFindVideoById),
                request.VideoUpdateOrderIndexRequestDto.Id
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var courseOwnerUserId = await _repositoryWrapper.VideosRepository.GetSingleOrDefaultProjectedAsync(
            v => v.Section!.Course!.OwnerUserId,
            v => v.Id == request.VideoUpdateOrderIndexRequestDto.Id,
            cancellationToken);

        if (courseOwnerUserId != userId)
        {
            var logErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToUpdateVideoForUserWithId),
                request.VideoUpdateOrderIndexRequestDto.Id,
                userId
            ].Value;

            _logger.LogError(request, logErrorMessage);

            var responseErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToUpdateVideoForUser),
                request.VideoUpdateOrderIndexRequestDto.Id
            ].Value;

            return Result.Fail(responseErrorMessage);
        }

        var sectionVideosCount = await _repositoryWrapper.VideosRepository.GetSelfCountAsync(
            v => v.SectionId == existingVideo.SectionId,
            cancellationToken: cancellationToken);

        var targetOrderIndex = PrepareOrderIndexForUpdate(
            existingVideo.OrderIndex,
            sectionVideosCount,
            request.VideoUpdateOrderIndexRequestDto.OrderIndex);

        if (targetOrderIndex != existingVideo.OrderIndex)
        {
            await OrderIndexShiftHelper.ShiftOrderIndexesForUpdateAsync(
                _repositoryWrapper.VideosRepository,
                $"{nameof(Video)}s",
                nameof(Video.SectionId),
                existingVideo.SectionId,
                nameof(Video.Id),
                existingVideo.Id,
                existingVideo.OrderIndex,
                targetOrderIndex,
                cancellationToken);
        }

        existingVideo.OrderIndex = targetOrderIndex;

        _repositoryWrapper.VideosRepository.Update(existingVideo);
        await _repositoryWrapper.SaveChangesAsync(cancellationToken);

        var videoResponseDto = _mapper.Map<VideoResponseDto>(existingVideo);

        return Result.Ok(videoResponseDto);
    }

    private static int PrepareOrderIndexForUpdate(int currentOrderIndex, int sectionVideosCount, int requestedOrderIndex)
    {
        if (sectionVideosCount <= 0)
        {
            return currentOrderIndex;
        }

        var maxAllowedOrderIndex = sectionVideosCount - 1;

        return Math.Min(requestedOrderIndex, maxAllowedOrderIndex);
    }
}
