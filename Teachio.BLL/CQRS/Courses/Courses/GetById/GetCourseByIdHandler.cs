using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Localization;
using Teachio.BLL.DTOs.Courses.Courses.Response;
using Teachio.BLL.DTOs.Courses.Videos.Videos.Response;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResource;
using Teachio.BLL.Utils.Helpers;
using Teachio.DAL.Entities.Courses.Courses;
using Teachio.DAL.Entities.Courses.Videos.Videos;
using Teachio.DAL.Repositories.Interfaces.Base;

namespace Teachio.BLL.CQRS.Courses.Courses.GetById;

public class GetCourseByIdHandler : IRequestHandler<GetCourseByIdQuery, Result<CourseResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICourseAccessService _courseAccessService;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;
    private readonly IStringLocalizer<NoPermissionsSharedResource> _stringLocalizerNoPermissions;

    public GetCourseByIdHandler(
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

    public async Task<Result<CourseResponseDto>> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        _logger.LogInformation($"Entered '{GetType().Name}' to get course by Id: {request.CourseId} by UserId: {userId}");

        var course = await _repositoryWrapper.CoursesRepository.GetSingleOrDefaultAsync(
            x => x.Id == request.CourseId,
            IncludeCourseRelatedEntities,
            cancellationToken);

        if (course is null)
        {
            var errorMessage = _stringLocalizerCannotFind[
                nameof(CannotFindSharedResource_en.CannotFindCourseById),
                request.CourseId
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var hasAccess = await _courseAccessService.HasAccessToCourseAsync(course.Id, userId, cancellationToken);
        if (!hasAccess)
        {
            var errorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToGetCourseForUser),
                request.CourseId
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var selectedVideo = course.Sections
            .SelectMany(s => s.Videos)
            .FirstOrDefault(v => request.SelectedVideoId.HasValue
                ? v.Id == request.SelectedVideoId.Value
                : !(v.VideoProgresses?.FirstOrDefault(p => p.AppUserId == userId)?.IsCompleted ?? false));

        if (selectedVideo is null && !request.SelectedVideoId.HasValue)
        {
            selectedVideo = course.Sections
                .OrderBy(s => s.OrderIndex)
                .FirstOrDefault()?
                .Videos
                .OrderBy(v => v.OrderIndex)
                .FirstOrDefault();
        }

        var courseResponseDto = _mapper.Map<CourseResponseDto>(course);

        MapSectionVideosProgress(courseResponseDto, course, userId);

        courseResponseDto.SelectedVideo = selectedVideo is null
            ? null
            : MapSelectedVideo(selectedVideo, userId);

        return Result.Ok(courseResponseDto);
    }

    private VideoResponseDto MapSelectedVideo(Video selectedVideo, Guid userId)
    {
        var selectedVideoResponseDto = _mapper.Map<VideoResponseDto>(selectedVideo);
        selectedVideoResponseDto.VideoProgress = VideoProgressHelper.BuildResponse(selectedVideo, userId);

        return selectedVideoResponseDto;
    }

    private static void MapSectionVideosProgress(CourseResponseDto courseResponseDto, Course course, Guid userId)
    {
        var videosById = course.Sections
            .SelectMany(section => section.Videos)
            .ToDictionary(video => video.Id, video => video);

        foreach (var sectionDto in courseResponseDto.Sections)
        {
            foreach (var videoDto in sectionDto.Videos)
            {
                if (!videosById.TryGetValue(videoDto.Id, out var videoEntity))
                {
                    continue;
                }

                videoDto.VideoProgress = VideoProgressHelper.BuildResponse(videoEntity, userId);
            }
        }
    }

    [ExcludeFromCodeCoverage]
    private static IIncludableQueryable<Course, object> IncludeCourseRelatedEntities(IQueryable<Course> query)
    {
        return query
            .Include(c => c.OwnerUser)
            .Include(c => c.Sections)
                .ThenInclude(s => s.Videos)
                    .ThenInclude(v => v.VideoFile)
            .Include(c => c.Sections)
                .ThenInclude(s => s.Videos)
                    .ThenInclude(v => v.VideoProgresses);
    }
}
