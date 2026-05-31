using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Localization;
using Studi.BLL.DTOs.Courses.Videos.VideoProgress.Response;
using Studi.BLL.Resources.SharedResource;
using Studi.BLL.Services.Interfaces;
using Studi.BLL.SharedResource;
using Studi.DAL.Repositories.Interfaces.Base;

namespace Studi.BLL.CQRS.Courses.Videos.VideoProgress.Update;

public class UpdateVideoProgressHandler : IRequestHandler<UpdateVideoProgressCommand, Result<VideoProgressResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICourseAccessService _courseAccessService;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;
    private readonly IStringLocalizer<NoPermissionsSharedResource> _stringLocalizerNoPermissions;

    public UpdateVideoProgressHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILoggerService logger,
        ICurrentUserService currentUserService,
        ICourseAccessService courseAccessService,
        IStringLocalizer<CannotFindSharedResource> stringLocalizerCannotFind,
        IStringLocalizer<NoPermissionsSharedResource> stringLocalizerNoPermissions)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
        _currentUserService = currentUserService;
        _courseAccessService = courseAccessService;
        _stringLocalizerCannotFind = stringLocalizerCannotFind;
        _stringLocalizerNoPermissions = stringLocalizerNoPermissions;
    }

    public async Task<Result<VideoProgressResponseDto>> Handle(UpdateVideoProgressCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        _logger.LogInformation($"Entered '{GetType().Name}' to update a video progress with Id: {request.VideoProgressUpdateRequestDto.Id} by UserId: {userId}");

        var existingVideoProgress = await _repositoryWrapper.VideoProgressRepository.GetSingleOrDefaultAsync(
            x => x.Id == request.VideoProgressUpdateRequestDto.Id,
            cancellationToken: cancellationToken);

        if (existingVideoProgress is null)
        {
            var errorMessage = _stringLocalizerCannotFind[
                nameof(CannotFindSharedResource_en.CannotFindVideoProgressById),
                request.VideoProgressUpdateRequestDto.Id
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        if (existingVideoProgress.AppUserId != userId)
        {
            var logErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToUpdateVideoProgressForUserWithId),
                request.VideoProgressUpdateRequestDto.Id,
                userId
            ].Value;

            _logger.LogError(request, logErrorMessage);

            var responseErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToUpdateVideoProgressForUser),
                request.VideoProgressUpdateRequestDto.Id
            ].Value;

            return Result.Fail(responseErrorMessage);
        }

        var hasAccess = await _courseAccessService.HasAccessToVideoAsync(existingVideoProgress.VideoId, userId, cancellationToken);
        if (!hasAccess)
        {
            var errorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToUpdateVideoProgressForUser),
                request.VideoProgressUpdateRequestDto.Id
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        _mapper.Map(request.VideoProgressUpdateRequestDto, existingVideoProgress);

        _repositoryWrapper.VideoProgressRepository.Update(existingVideoProgress);
        await _repositoryWrapper.SaveChangesAsync(cancellationToken);

        var videoProgressResponseDto = _mapper.Map<VideoProgressResponseDto>(existingVideoProgress);

        return Result.Ok(videoProgressResponseDto);
    }
}
