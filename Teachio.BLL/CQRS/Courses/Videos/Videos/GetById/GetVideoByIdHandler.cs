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
using Teachio.DAL.Repositories.Interfaces.Base;
using VideoEntity = Teachio.DAL.Entities.Courses.Videos.Videos.Video;

namespace Teachio.BLL.CQRS.Courses.Videos.Videos.GetById;

public class GetVideoByIdHandler : IRequestHandler<GetVideoByIdQuery, Result<VideoResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;

    public GetVideoByIdHandler(
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

    public async Task<Result<VideoResponseDto>> Handle(GetVideoByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        _logger.LogInformation($"Entered '{GetType().Name}' to get video by Id: {request.VideoId} by UserId: {userId}");

        // TODO: validate whether the user has access to the course with this video

        var video = await _repositoryWrapper.VideosRepository.GetSingleOrDefaultAsync(
            x => x.Id == request.VideoId,
            IncludeVideoRelatedEntities,
            cancellationToken);

        if (video is null)
        {
            var errorMessage = _stringLocalizerCannotFind[
                nameof(CannotFindSharedResource_en.CannotFindVideoById),
                request.VideoId
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

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
