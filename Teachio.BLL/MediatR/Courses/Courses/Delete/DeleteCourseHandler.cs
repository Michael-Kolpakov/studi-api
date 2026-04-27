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
using Teachio.BLL.Utils.Helpers;
using Teachio.DAL.Repositories.Interfaces.Base;
using CourseEntity = Teachio.DAL.Entities.Courses.Courses.Course;

namespace Teachio.BLL.MediatR.Courses.Courses.Delete;

public class DeleteCourseHandler : IRequestHandler<DeleteCourseCommand, Result<CourseResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IGoogleDriveStorageService _googleDriveStorageService;
    private readonly ILoggerService _logger;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;
    private readonly IStringLocalizer<NoPermissionsSharedResource> _stringLocalizerNoPermissions;

    public DeleteCourseHandler(
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

    public async Task<Result<CourseResponseDto>> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Entered '{GetType().Name}' to delete a course with Id: {request.CourseId}");

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

        if (course.OwnerUserId != request.RequestingUserId)
        {
            var logErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToDeleteCourseForUserWithId),
                request.CourseId,
                request.RequestingUserId
            ].Value;

            _logger.LogError(request, logErrorMessage);

            var responseErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToDeleteCourseForUser),
                request.CourseId
            ].Value;

            return Result.Fail(responseErrorMessage);
        }

        // TODO: make sure whether we really delete all course dependent entities: Sections, Videos and VideoProgress

        // TODO: address Google Drive API (or CDN in the future) to delete thumbnail image
        var ownerUserEmail = course.OwnerUser.Email;

        var courseVideoFiles = course.Sections
            .SelectMany(section => section.Videos)
            .Where(video => video.VideoFile is not null)
            .Select(video => new
            {
                video.Section!.SectionName,
                VideoFileName = video.VideoFile!.VideoName
            })
            .ToList();

        foreach (var courseVideoFile in courseVideoFiles)
        {
            var deleteGoogleDriveFileResult = await _googleDriveStorageService.DeleteFileByPathAsync(
                VideoStoragePathHelper.BuildVideoFolderSegments(
                    ownerUserEmail!,
                    course.CourseName,
                    courseVideoFile.SectionName),
                courseVideoFile.VideoFileName,
                cancellationToken);

            if (deleteGoogleDriveFileResult.IsFailed)
            {
                var errorMessage = deleteGoogleDriveFileResult.Errors[0].Message;
                _logger.LogError(request, errorMessage);

                return Result.Fail(errorMessage);
            }
        }

        _repositoryWrapper.CoursesRepository.Delete(course);
        await _repositoryWrapper.SaveChangesAsync(cancellationToken);

        var courseResponseDto = _mapper.Map<CourseResponseDto>(course);

        return Result.Ok(courseResponseDto);
    }

    [ExcludeFromCodeCoverage]
    private static IIncludableQueryable<CourseEntity, object> IncludeCourseRelatedEntities(IQueryable<CourseEntity> query)
    {
        return query
            .Include(c => c.OwnerUser)
            .Include(c => c.Sections)
                .ThenInclude(s => s.Videos)
                    .ThenInclude(v => v.VideoFile)
            .Include(c => c.Sections)
                .ThenInclude(s => s.Videos)
                    .ThenInclude(v => v.VideoProgress);
    }
}
