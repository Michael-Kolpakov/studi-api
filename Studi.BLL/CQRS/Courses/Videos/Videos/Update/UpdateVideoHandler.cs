using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Localization;
using Studi.BLL.DTOs.Courses.Videos.Videos.Response;
using Studi.BLL.Resources.SharedResource;
using Studi.BLL.Services.Interfaces;
using Studi.BLL.SharedResource;
using Studi.BLL.Utils.Helpers;
using Studi.BLL.Utils.MappingResolvers;
using Studi.DAL.Entities.Courses.Videos.Videos;
using Studi.DAL.Repositories.Interfaces.Base;

namespace Studi.BLL.CQRS.Courses.Videos.Videos.Update;

public class UpdateVideoHandler : IRequestHandler<UpdateVideoCommand, Result<VideoResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;
    private readonly IStringLocalizer<NoPermissionsSharedResource> _stringLocalizerNoPermissions;
    private readonly IStringLocalizer<AlreadyExistsSharedResource> _stringLocalizerAlreadyExists;

    public UpdateVideoHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILoggerService logger,
        ICurrentUserService currentUserService,
        IStringLocalizer<CannotFindSharedResource> stringLocalizerCannotFind,
        IStringLocalizer<NoPermissionsSharedResource> stringLocalizerNoPermissions,
        IStringLocalizer<AlreadyExistsSharedResource> stringLocalizerAlreadyExists)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
        _currentUserService = currentUserService;
        _stringLocalizerCannotFind = stringLocalizerCannotFind;
        _stringLocalizerNoPermissions = stringLocalizerNoPermissions;
        _stringLocalizerAlreadyExists = stringLocalizerAlreadyExists;
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

        var updatedVideoName = NameFromTitleResolver.CreateNameFromTitle(request.VideoUpdateRequestDto.Title);
        var existingVideoNames = await _repositoryWrapper.VideosRepository.GetProjectedListAsync(
            v => v.VideoFile == null ? null : v.VideoFile.VideoName,
            v => v.SectionId == existingVideo.SectionId && v.Id != existingVideo.Id && v.VideoFile != null,
            cancellationToken: cancellationToken);

        var videoWithSameNameExists = existingVideoNames.FirstOrDefault(
            existingVideoName => VideoNameHelper.HasSameBaseName(existingVideoName, updatedVideoName));

        if (videoWithSameNameExists is not null)
        {
            var errorMessage = _stringLocalizerAlreadyExists[
                nameof(AlreadyExistsSharedResource_en.VideoAlreadyExistsForSection),
                updatedVideoName,
                existingVideo.SectionId
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        _mapper.Map(request.VideoUpdateRequestDto, existingVideo);

        _repositoryWrapper.VideosRepository.Update(existingVideo);
        await _repositoryWrapper.SaveChangesAsync(cancellationToken);

        var videoResponseDto = _mapper.Map<VideoResponseDto>(existingVideo);

        return Result.Ok(videoResponseDto);
    }

    [ExcludeFromCodeCoverage]
    private static IIncludableQueryable<Video, object> IncludeVideoRelatedEntities(IQueryable<Video> query)
    {
        return query
            .Include(v => v.VideoFile)
            .Include(v => v.VideoProgresses);
    }
}
