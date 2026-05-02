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

namespace Teachio.BLL.CQRS.Courses.Videos.Videos.Delete;

public class DeleteVideoHandler : IRequestHandler<DeleteVideoCommand, Result<VideoResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IGoogleDriveStorageService _googleDriveStorageService;
    private readonly ILoggerService _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;
    private readonly IStringLocalizer<NoPermissionsSharedResource> _stringLocalizerNoPermissions;

    public DeleteVideoHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IGoogleDriveStorageService googleDriveStorageService,
        ILoggerService logger,
        ICurrentUserService currentUserService,
        IStringLocalizer<CannotFindSharedResource> stringLocalizerCannotFind,
        IStringLocalizer<NoPermissionsSharedResource> stringLocalizerNoPermissions)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _googleDriveStorageService = googleDriveStorageService;
        _logger = logger;
        _currentUserService = currentUserService;
        _stringLocalizerCannotFind = stringLocalizerCannotFind;
        _stringLocalizerNoPermissions = stringLocalizerNoPermissions;
    }

    public async Task<Result<VideoResponseDto>> Handle(DeleteVideoCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        _logger.LogInformation($"Entered '{GetType().Name}' to delete a video with Id: {request.VideoId} by UserId: {userId}");

        // TODO: validate whether gained video really belongs to the user making the request (and perhaps remove check below)

        var video = await _repositoryWrapper.VideosRepository.GetSingleOrDefaultAsync(
            x => x.Id == request.VideoId,
            IncludeVideoRelatedEntities,
            cancellationToken);

        if (video is null)
        {
            var errorMessage = _stringLocalizerCannotFind[nameof(CannotFindSharedResource_en.CannotFindVideoById), request.VideoId].Value;
            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var courseOwnerUserId = video.Section!.Course!.OwnerUserId;

        if (courseOwnerUserId != userId)
        {
            var logErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToDeleteVideoForUserWithId),
                request.VideoId,
                userId
            ].Value;

            _logger.LogError(request, logErrorMessage);

            var responseErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToDeleteVideoForUser),
                request.VideoId
            ].Value;

            return Result.Fail(responseErrorMessage);
        }

        var videoDeletionResult = await DeleteVideoFromCDNAsync(video, request, cancellationToken);
        if (videoDeletionResult.IsFailed)
        {
            var errorMessage = videoDeletionResult.Errors[0].Message;

            return Result.Fail(errorMessage);
        }

        _repositoryWrapper.VideosRepository.Delete(video);

        var sectionVideos = (await _repositoryWrapper.VideosRepository.GetAllAsync(
                v => v.SectionId == video.SectionId && v.Id != video.Id,
                cancellationToken: cancellationToken))
            .OrderBy(v => v.OrderIndex)
            .ToList();

        PrepareOrderIndexesForDelete(sectionVideos);

        if (sectionVideos.Count > 0)
        {
            _repositoryWrapper.VideosRepository.UpdateRange(sectionVideos);
        }

        video.Section.VideosCount = sectionVideos.Count;

        _repositoryWrapper.SectionsRepository.Update(video.Section);
        await _repositoryWrapper.SaveChangesAsync(cancellationToken);

        var videoResponseDto = _mapper.Map<VideoResponseDto>(video);

        return Result.Ok(videoResponseDto);
    }

    [ExcludeFromCodeCoverage]
    private static IIncludableQueryable<VideoEntity, object> IncludeVideoRelatedEntities(IQueryable<VideoEntity> query)
    {
        return query
            .Include(v => v.Section)
                .ThenInclude(s => s!.Course)
                    .ThenInclude(c => c!.OwnerUser)
            .Include(v => v.VideoFile)
            .Include(v => v.VideoProgress);
    }

    private static void PrepareOrderIndexesForDelete(List<VideoEntity> sectionVideos)
    {
        VideoOrderIndexHelper.NormalizeOrderIndexes(sectionVideos);
    }

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "CDN is a constant abbreviation and it's ok to use it in the method name for better readability and understanding of the method's purpose.")]
    private async Task<Result> DeleteVideoFromCDNAsync(
        VideoEntity video,
        DeleteVideoCommand request,
        CancellationToken cancellationToken)
    {
        if (video.VideoFile is null)
        {
            return Result.Ok();
        }

        var ownerUserEmail = video.Section!.Course!.OwnerUser.Email;

        var deleteGoogleDriveFileResult = await _googleDriveStorageService.DeleteFileByPathAsync(
            VideoStoragePathHelper.BuildVideoFolderSegments(
                ownerUserEmail!,
                video.Section.Course.CourseName,
                video.Section.SectionName),
            video.VideoFile.VideoName,
            cancellationToken);

        if (deleteGoogleDriveFileResult.IsFailed)
        {
            var errorMessage = deleteGoogleDriveFileResult.Errors[0].Message;
            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        return Result.Ok();
    }
}
