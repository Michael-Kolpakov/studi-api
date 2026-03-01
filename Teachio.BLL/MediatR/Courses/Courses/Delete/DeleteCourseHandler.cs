using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Localization;
using Teachio.BLL.Dto.Courses.Courses.Response;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResource;
using Teachio.DAL.Repositories.Interfaces.Base;
using CourseEntity = Teachio.DAL.Entities.Courses.Courses.Course;

namespace Teachio.BLL.MediatR.Courses.Courses.Delete;

public class DeleteCourseHandler : IRequestHandler<DeleteCourseCommand, Result<CourseResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;
    private readonly IStringLocalizer<NoPermissionsSharedResource> _stringLocalizerNoPermissions;

    public DeleteCourseHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILoggerService logger,
        IStringLocalizer<CannotFindSharedResource> stringLocalizerCannotFind,
        IStringLocalizer<NoPermissionsSharedResource> stringLocalizerNoPermissions)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
        _stringLocalizerCannotFind = stringLocalizerCannotFind;
        _stringLocalizerNoPermissions = stringLocalizerNoPermissions;
    }

    public async Task<Result<CourseResponseDto>> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Entered '{GetType().Name}' to delete a course with Id: {request.courseId}");

        var course = await _repositoryWrapper.CoursesRepository.GetSingleOrDefaultAsync(
            x => x.Id == request.courseId,
            IncludeCourseRelatedEntities);

        if (course is null)
        {
            var errorMessage = _stringLocalizerCannotFind[nameof(CannotFindSharedResource_en.CannotFindCourseById), request.courseId].Value;
            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        if (course.OwnerUserId != request.requestingUserId)
        {
            var logErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToDeleteCourseForUserWithId),
                request.courseId,
                request.requestingUserId
            ].Value;

            _logger.LogError(request, logErrorMessage);

            var responseErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToDeleteCourseForUser),
                request.courseId
            ].Value;

            return Result.Fail(responseErrorMessage);
        }

        // TODO: make sure whether we really delete all course dependent entities: Sections, Videos and VideoProgress

        // TODO: address Google Drive API (or CDN in the future) to delete thumbnail image

        _repositoryWrapper.CoursesRepository.Delete(course);
        await _repositoryWrapper.SaveChangesAsync();

        var courseResponseDto = _mapper.Map<CourseResponseDto>(course);

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
