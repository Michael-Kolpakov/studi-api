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
using Teachio.BLL.SharedResource;
using Teachio.DAL.Entities.Courses.Videos.Videos;
using Teachio.DAL.Repositories.Interfaces.Base;
using Teachio.DAL.Utils.Constants;
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

        var sectionVideosCount = await _repositoryWrapper.VideosRepository.GetSelfCountAsync(
            v => v.SectionId == existingVideo.SectionId,
            cancellationToken: cancellationToken);

        var targetOrderIndex = PrepareOrderIndexForUpdate(
            existingVideo.OrderIndex,
            sectionVideosCount,
            request.VideoUpdateRequestDto.OrderIndex);

        if (targetOrderIndex != existingVideo.OrderIndex)
        {
            await ShiftOrderIndexesForUpdateAsync(
                existingVideo.SectionId,
                existingVideo.Id,
                existingVideo.OrderIndex,
                targetOrderIndex,
                cancellationToken);
        }

        _mapper.Map(request.VideoUpdateRequestDto, existingVideo);
        existingVideo.OrderIndex = targetOrderIndex;

        _repositoryWrapper.VideosRepository.Update(existingVideo);
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

    private async Task ShiftOrderIndexesForUpdateAsync(
        Guid sectionId,
        Guid videoId,
        int currentOrderIndex,
        int targetOrderIndex,
        CancellationToken cancellationToken)
    {
        var table = $"[{DatabaseConstants.CoursesSchema}].[{nameof(Video)}s]";

        var sql = $"""
            UPDATE {table}
            SET {nameof(VideoEntity.OrderIndex)} =
                CASE
                    WHEN {nameof(VideoEntity.Id)} = '{videoId}'
                        THEN {targetOrderIndex}
                    WHEN {targetOrderIndex} < {currentOrderIndex}
                        AND {nameof(VideoEntity.OrderIndex)} >= {targetOrderIndex}
                        AND {nameof(VideoEntity.OrderIndex)} < {currentOrderIndex}
                        THEN {nameof(VideoEntity.OrderIndex)} + 1
                    WHEN {targetOrderIndex} > {currentOrderIndex}
                        AND {nameof(VideoEntity.OrderIndex)} <= {targetOrderIndex}
                        AND {nameof(VideoEntity.OrderIndex)} > {currentOrderIndex}
                        THEN {nameof(VideoEntity.OrderIndex)} - 1
                    ELSE {nameof(VideoEntity.OrderIndex)}
                END
            WHERE {nameof(VideoEntity.SectionId)} = '{sectionId}'
                AND (
                    {nameof(VideoEntity.Id)} = '{videoId}'
                    OR (
                        {targetOrderIndex} < {currentOrderIndex}
                        AND {nameof(VideoEntity.OrderIndex)} >= {targetOrderIndex}
                        AND {nameof(VideoEntity.OrderIndex)} < {currentOrderIndex}
                    )
                    OR (
                        {targetOrderIndex} > {currentOrderIndex}
                        AND {nameof(VideoEntity.OrderIndex)} <= {targetOrderIndex}
                        AND {nameof(VideoEntity.OrderIndex)} > {currentOrderIndex}
                    )
                )
        """;

        await _repositoryWrapper.VideosRepository.ExecuteSqlRaw(sql, cancellationToken);
    }

    private static int PrepareOrderIndexForUpdate(
        int currentOrderIndex,
        int sectionVideosCount,
        int requestedOrderIndex)
    {
        if (sectionVideosCount <= 0)
        {
            return currentOrderIndex;
        }

        return Math.Min(requestedOrderIndex, sectionVideosCount - 1);
    }
}
