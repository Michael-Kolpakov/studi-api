using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Localization;
using Teachio.BLL.Dto.Courses.Videos.Videos.Response;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResource;
using Teachio.BLL.Utils.Helpers;
using Teachio.DAL.Repositories.Interfaces.Base;
using VideoEntity = Teachio.DAL.Entities.Courses.Videos.Videos.Video;

namespace Teachio.BLL.MediatR.Courses.Videos.Videos.Delete;

public class DeleteVideoHandler : IRequestHandler<DeleteVideoCommand, Result<VideoResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IGoogleDriveStorageService _googleDriveStorageService;
    private readonly ILoggerService _logger;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;
    private readonly IStringLocalizer<NoPermissionsSharedResource> _stringLocalizerNoPermissions;

    public DeleteVideoHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IGoogleDriveStorageService googleDriveStorageService,
        ILoggerService logger,
        IStringLocalizer<CannotFindSharedResource> stringLocalizerCannotFind,
        IStringLocalizer<NoPermissionsSharedResource> stringLocalizerNoPermissions)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _googleDriveStorageService = googleDriveStorageService;
        _logger = logger;
        _stringLocalizerCannotFind = stringLocalizerCannotFind;
        _stringLocalizerNoPermissions = stringLocalizerNoPermissions;
    }

    public async Task<Result<VideoResponseDto>> Handle(DeleteVideoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Entered '{GetType().Name}' to delete a video with Id: {request.VideoId}");

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

        if (courseOwnerUserId != request.RequestingUserId)
        {
            var logErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToDeleteVideoForUserWithId),
                request.VideoId,
                request.RequestingUserId
            ].Value;

            _logger.LogError(request, logErrorMessage);

            var responseErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToDeleteVideoForUser),
                request.VideoId
            ].Value;

            return Result.Fail(responseErrorMessage);
        }

        if (video.VideoFile is not null)
        {
            var ownerUserEmail = video.Section.Course.OwnerUser.Email;

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
        }

        _repositoryWrapper.VideosRepository.Delete(video);
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
}
