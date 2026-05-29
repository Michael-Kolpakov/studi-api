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
using Studi.DAL.Entities.Courses.Videos.Videos;
using Studi.DAL.Repositories.Interfaces.Base;

namespace Studi.BLL.CQRS.Courses.Videos.Videos.GetBySectionId;

public class GetVideosBySectionIdHandler : IRequestHandler<GetVideosBySectionIdQuery, Result<SectionVideosResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;

    public GetVideosBySectionIdHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILoggerService logger,
        ICurrentUserService currentUserService,
        IStringLocalizer<CannotFindSharedResource> stringLocalizerCannotFind)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
        _currentUserService = currentUserService;
        _stringLocalizerCannotFind = stringLocalizerCannotFind;
    }

    public async Task<Result<SectionVideosResponseDto>> Handle(GetVideosBySectionIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        _logger.LogInformation($"Entered '{GetType().Name}' to get videos by SectionId: {request.SectionId} by UserId: {userId}");

        var section = await _repositoryWrapper.SectionsRepository.GetSingleOrDefaultAsync(
            x => x.Id == request.SectionId,
            cancellationToken: cancellationToken);

        if (section is null)
        {
            var errorMessage = _stringLocalizerCannotFind[
                nameof(CannotFindSharedResource_en.CannotFindSectionById),
                request.SectionId
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var videos = await _repositoryWrapper.VideosRepository.GetAllAsync(
            video => video.SectionId == request.SectionId,
            IncludeVideoRelatedEntities,
            cancellationToken);

        var orderedVideos = videos
            .OrderBy(video => video.OrderIndex)
            .ToList();

        var videosResponseDto = new SectionVideosResponseDto
        {
            Videos = _mapper.Map<List<VideoEditShortResponseDto>>(orderedVideos)
        };

        return Result.Ok(videosResponseDto);
    }

    [ExcludeFromCodeCoverage]
    private static IIncludableQueryable<Video, object> IncludeVideoRelatedEntities(IQueryable<Video> query)
    {
        return query
            .Include(v => v.VideoFile!);
    }
}
