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
using Teachio.DAL.Repositories.Interfaces.Base;
using VideoEntity = Teachio.DAL.Entities.Courses.Videos.Videos.Video;

namespace Teachio.BLL.MediatR.Courses.Videos.Videos.Update;

public class UpdateVideoHandler : IRequestHandler<UpdateVideoCommand, Result<VideoResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;
    private readonly IStringLocalizer<NoPermissionsSharedResource> _stringLocalizerNoPermissions;
    private readonly IStringLocalizer<AlreadyExistsSharedResource> _stringLocalizerAlreadyExists;

    public UpdateVideoHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILoggerService logger,
        IStringLocalizer<CannotFindSharedResource> stringLocalizerCannotFind,
        IStringLocalizer<NoPermissionsSharedResource> stringLocalizerNoPermissions,
        IStringLocalizer<AlreadyExistsSharedResource> stringLocalizerAlreadyExists)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
        _stringLocalizerCannotFind = stringLocalizerCannotFind;
        _stringLocalizerNoPermissions = stringLocalizerNoPermissions;
        _stringLocalizerAlreadyExists = stringLocalizerAlreadyExists;
    }

    public async Task<Result<VideoResponseDto>> Handle(UpdateVideoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Entered '{GetType().Name}' to update a video with Id: {request.VideoUpdateRequestDto.Id}");

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

        if (courseOwnerUserId != request.RequestingUserId)
        {
            var logErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToUpdateVideoForUserWithId),
                request.VideoUpdateRequestDto.Id,
                request.RequestingUserId
            ].Value;

            _logger.LogError(request, logErrorMessage);

            var responseErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToUpdateVideoForUser),
                request.VideoUpdateRequestDto.Id
            ].Value;

            return Result.Fail(responseErrorMessage);
        }

        var videoWithSameOrderIndexExists = await _repositoryWrapper.VideosRepository.GetSingleOrDefaultAsync(
            v => v.SectionId == existingVideo.SectionId
                 && v.OrderIndex == request.VideoUpdateRequestDto.OrderIndex
                 && v.Id != request.VideoUpdateRequestDto.Id,
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

        // TODO: validate whether course was updated successfully, if YES - address Google Drive API (or CDN in the future) to delete old video file and set new one

        // TODO: validate whether course was updated successfully, if NO - address Google Drive API (or CDN in the future) to delete current video file

        _mapper.Map(request.VideoUpdateRequestDto, existingVideo);

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
}
