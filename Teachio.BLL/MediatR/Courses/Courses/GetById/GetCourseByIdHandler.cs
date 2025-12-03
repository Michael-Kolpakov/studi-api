using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Teachio.BLL.Dto.Courses.Courses.Response;
using Teachio.BLL.Dto.Courses.Videos.Videos.Response;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResource;
using Teachio.DAL.Repositories.Interfaces.Base;

namespace Teachio.BLL.MediatR.Courses.Courses.GetById;

public class GetCourseByIdHandler : IRequestHandler<GetCourseByIdQuery, Result<CourseResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;

    public GetCourseByIdHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILoggerService logger,
        IStringLocalizer<CannotFindSharedResource> stringLocalizerCannotFind)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
        _stringLocalizerCannotFind = stringLocalizerCannotFind;
    }

    public async Task<Result<CourseResponseDto>> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Entered '{GetType().Name}' to get course by Id: {request.id}");

        // TODO: validate whether the user has access to the course

        var course = await _repositoryWrapper.CoursesRepository.GetSingleOrDefaultAsync(
            x => x.Id == request.id,
            q => q
                .Include(c => c.Sections)
                    .ThenInclude(s => s.Videos)
                        .ThenInclude(v => v.VideoProgress));

        if (course is null)
        {
            var errorMessage = _stringLocalizerCannotFind[nameof(CannotFindSharedResource_en.CannotFindCourseById), request.id].Value;
            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var watchingUsersCount = await _repositoryWrapper.CoursesRepository.GetNavigationCollectionCountAsync(
            x => x.WatchingUsers,
            x => x.Id == request.id);

        var selectedVideo = course.Sections
            .SelectMany(s => s.Videos)
            .FirstOrDefault(v => request.SelectedVideoId.HasValue
                ? v.Id == request.SelectedVideoId.Value
                : !v.VideoProgress.IsCompleted);

        var courseResponseDto = _mapper.Map<CourseResponseDto>(course);
        courseResponseDto.SelectedVideo = _mapper.Map<VideoResponseDto?>(selectedVideo);
        courseResponseDto.WatchingUsersCount = watchingUsersCount;

        return Result.Ok(courseResponseDto);
    }
}
