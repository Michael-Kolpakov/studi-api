using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Localization;
using Teachio.BLL.Dto.Courses.Courses.Response;
using Teachio.BLL.Dto.Courses.Videos.Videos.Response;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResource;
using Teachio.DAL.Repositories.Interfaces.Base;
using CourseEntity = Teachio.DAL.Entities.Courses.Courses.Course;

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
        _logger.LogInformation($"Entered '{GetType().Name}' to get course by Id: {request.courseId}");

        // TODO: validate whether the user has access to the course

        var course = await _repositoryWrapper.CoursesRepository.GetSingleOrDefaultAsync(
            x => x.Id == request.courseId,
            IncludeCourseRelatedEntities);

        if (course is null)
        {
            var errorMessage = _stringLocalizerCannotFind[nameof(CannotFindSharedResource_en.CannotFindCourseById), request.courseId].Value;
            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var selectedVideo = course.Sections
            .SelectMany(s => s.Videos)
            .FirstOrDefault(v => request.selectedVideoId.HasValue
                ? v.Id == request.selectedVideoId.Value
                : !v.VideoProgress.IsCompleted);

        if (selectedVideo is null && !request.selectedVideoId.HasValue)
        {
            selectedVideo = course.Sections
                .OrderBy(s => s.OrderIndex)
                .FirstOrDefault()?
                .Videos
                .OrderBy(v => v.OrderIndex)
                .FirstOrDefault();
        }

        var courseResponseDto = _mapper.Map<CourseResponseDto>(course);
        courseResponseDto.SelectedVideo = _mapper.Map<VideoResponseDto?>(selectedVideo);

        return Result.Ok(courseResponseDto);
    }

    [ExcludeFromCodeCoverage]
    private static IIncludableQueryable<CourseEntity, object> IncludeCourseRelatedEntities(IQueryable<CourseEntity> query)
    {
        return query
            .Include(c => c.Sections)
                .ThenInclude(s => s.Videos)
                    .ThenInclude(v => v.VideoProgress);
    }
}
