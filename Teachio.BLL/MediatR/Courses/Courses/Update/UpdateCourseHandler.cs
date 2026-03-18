using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Localization;
using Teachio.BLL.Dto.Courses.Courses.Response;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResource;
using Teachio.BLL.Utils.MappingResolvers;
using Teachio.DAL.Repositories.Interfaces.Base;

namespace Teachio.BLL.MediatR.Courses.Courses.Update;

/// <summary>
/// Represents the <see cref="UpdateCourseHandler"/> type.
/// </summary>
public class UpdateCourseHandler : IRequestHandler<UpdateCourseCommand, Result<CourseResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;
    private readonly IStringLocalizer<NoPermissionsSharedResource> _stringLocalizerNoPermissions;
    private readonly IStringLocalizer<AlreadyExistsSharedResource> _stringLocalizerAlreadyExists;

    public UpdateCourseHandler(
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

    /// <summary>
    /// Handles the incoming request.
    /// </summary>
    /// <param name="request">The request payload in <paramref name="request"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The result produced by this operation.</returns>
    public async Task<Result<CourseResponseDto>> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Entered '{GetType().Name}' to update a course with Id: {request.CourseUpdateRequestDto.Id}");

        // TODO: validate whether course thumbnail exists (database relationships and ownership)

        var existingCourse = await _repositoryWrapper.CoursesRepository.GetSingleOrDefaultAsync(
            x => x.Id == request.CourseUpdateRequestDto.Id,
            cancellationToken: cancellationToken);

        if (existingCourse is null)
        {
            var errorMessage = _stringLocalizerCannotFind[
                nameof(CannotFindSharedResource_en.CannotFindCourseById),
                request.CourseUpdateRequestDto.Id
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        if (existingCourse.OwnerUserId != request.RequestingUserId)
        {
            var logErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToUpdateCourseForUserWithId),
                request.CourseUpdateRequestDto.Id,
                request.RequestingUserId
            ].Value;

            _logger.LogError(request, logErrorMessage);

            var responseErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToUpdateCourseForUser),
                request.CourseUpdateRequestDto.Id
            ].Value;

            return Result.Fail(responseErrorMessage);
        }

        var courseWithSameNameExists = await _repositoryWrapper.CoursesRepository.GetSingleOrDefaultAsync(
            c => c.OwnerUserId == existingCourse.OwnerUserId
                 && c.CourseName == NameFromTitleResolver.CreateNameFromTitle(request.CourseUpdateRequestDto.Title)
                 && c.Id != request.CourseUpdateRequestDto.Id,
            cancellationToken: cancellationToken);

        if (courseWithSameNameExists is not null)
        {
            var logErrorMessage = _stringLocalizerAlreadyExists[
                nameof(AlreadyExistsSharedResource_en.CourseAlreadyExistsForUserWithId),
                existingCourse.CourseName,
                existingCourse.OwnerUserId
            ].Value;

            _logger.LogError(request, logErrorMessage);

            var responseErrorMessage = _stringLocalizerAlreadyExists[
                nameof(AlreadyExistsSharedResource_en.CourseAlreadyExistsForUser),
                existingCourse.Title
            ].Value;

            return Result.Fail(responseErrorMessage);
        }

        // TODO: validate whether course was updated successfully, if YES - address Google Drive API (or CDN in the future) to delete old thumbnail image and set new one

        // TODO: validate whether course was updated successfully, if NO - address Google Drive API (or CDN in the future) to delete current thumbnail image

        _mapper.Map(request.CourseUpdateRequestDto, existingCourse);

        // TODO: if user updated title of a course update a course folder name on Google Drive (or CDN in the future)

        _repositoryWrapper.CoursesRepository.Update(existingCourse);
        await _repositoryWrapper.SaveChangesAsync(cancellationToken);

        var courseResponseDto = _mapper.Map<CourseResponseDto>(existingCourse);

        return Result.Ok(courseResponseDto);
    }
}
