using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Localization;
using Teachio.BLL.DTOs.Courses.Sections.Response;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResource;
using Teachio.BLL.Utils.Helpers;
using Teachio.DAL.Entities.Courses.Sections;
using Teachio.DAL.Repositories.Interfaces.Base;
using SectionEntity = Teachio.DAL.Entities.Courses.Sections.Section;

namespace Teachio.BLL.CQRS.Courses.Sections.Delete;

public class DeleteSectionHandler : IRequestHandler<DeleteSectionCommand, Result<SectionResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IGoogleDriveStorageService _googleDriveStorageService;
    private readonly ILoggerService _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;
    private readonly IStringLocalizer<NoPermissionsSharedResource> _stringLocalizerNoPermissions;

    public DeleteSectionHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IGoogleDriveStorageService googleDriveStorageService,
        ILoggerService logger,
        ICurrentUserService currentUserService,
        IStringLocalizer<CannotFindSharedResource> stringLocalizerCannotFind,
        IStringLocalizer<NoPermissionsSharedResource> stringLocalizerNoPermissions)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _googleDriveStorageService = googleDriveStorageService;
        _logger = logger;
        _currentUserService = currentUserService;
        _stringLocalizerCannotFind = stringLocalizerCannotFind;
        _stringLocalizerNoPermissions = stringLocalizerNoPermissions;
    }

    public async Task<Result<SectionResponseDto>> Handle(DeleteSectionCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        _logger.LogInformation($"Entered '{GetType().Name}' to delete a section with Id: {request.SectionId} by UserId: {userId}");

        var section = await _repositoryWrapper.SectionsRepository.GetSingleOrDefaultAsync(
            x => x.Id == request.SectionId,
            IncludeSectionRelatedEntities,
            cancellationToken);

        if (section is null)
        {
            var errorMessage = _stringLocalizerCannotFind[
                nameof(CannotFindSharedResource_en.CannotFindSectionById),
                request.SectionId
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var courseOwnerUserId = section.Course!.OwnerUserId;

        if (courseOwnerUserId != userId)
        {
            var logErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToDeleteSectionForUserWithId),
                request.SectionId,
                userId
            ].Value;

            _logger.LogError(request, logErrorMessage);

            var responseErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToDeleteSectionForUser),
                request.SectionId
            ].Value;

            return Result.Fail(responseErrorMessage);
        }

        var ownerUserEmail = section.Course.OwnerUser.Email;

        var sectionFolderDeletionResult = await DeleteSectionFolderFromDriveAsync(section, ownerUserEmail!, request, cancellationToken);
        if (sectionFolderDeletionResult.IsFailed)
        {
            var errorMessage = sectionFolderDeletionResult.Errors[0].Message;

            return Result.Fail(errorMessage);
        }

        _repositoryWrapper.SectionsRepository.Delete(section);
        await _repositoryWrapper.SaveChangesAsync(cancellationToken);

        await OrderIndexShiftHelper.ShiftOrderIndexesForDeleteAsync(
            _repositoryWrapper.SectionsRepository,
            $"{nameof(Section)}s",
            nameof(SectionEntity.CourseId),
            section.CourseId,
            section.OrderIndex,
            cancellationToken);

        section.Course.SectionsCount = await _repositoryWrapper.SectionsRepository.GetSelfCountAsync(
            s => s.CourseId == section.CourseId,
            cancellationToken: cancellationToken);

        _repositoryWrapper.CoursesRepository.Update(section.Course);
        await _repositoryWrapper.SaveChangesAsync(cancellationToken);

        var sectionResponseDto = _mapper.Map<SectionResponseDto>(section);

        return Result.Ok(sectionResponseDto);
    }

    [ExcludeFromCodeCoverage]
    private static IIncludableQueryable<SectionEntity, object> IncludeSectionRelatedEntities(IQueryable<SectionEntity> query)
    {
        return query
            .Include(s => s.Course)
                .ThenInclude(c => c!.OwnerUser)
            .Include(s => s.Videos)
                .ThenInclude(v => v.VideoFile)
            .Include(s => s.Videos)
                .ThenInclude(v => v.VideoProgress);
    }

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "CDN is a constant abbreviation and it's ok to use it in the method name for better readability and understanding of the method's purpose.")]
    private async Task<Result> DeleteSectionFolderFromDriveAsync(
        SectionEntity section,
        string ownerUserEmail,
        DeleteSectionCommand request,
        CancellationToken cancellationToken)
    {
        var sectionFolderDeletionResult = await _googleDriveStorageService.DeleteFolderByPathAsync(
            StoragePathHelper.BuildSectionFolderSegments(
                ownerUserEmail,
                section.Course!.CourseName,
                section.SectionName),
            cancellationToken);

        if (sectionFolderDeletionResult.IsFailed)
        {
            var errorMessage = sectionFolderDeletionResult.Errors[0].Message;
            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        return Result.Ok();
    }
}
