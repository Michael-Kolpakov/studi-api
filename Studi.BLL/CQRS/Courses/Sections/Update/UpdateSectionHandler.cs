using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Localization;
using Studi.BLL.DTOs.Courses.Sections.Response;
using Studi.BLL.Resources.SharedResource;
using Studi.BLL.Services.Interfaces;
using Studi.BLL.SharedResource;
using Studi.BLL.Utils.Helpers;
using Studi.BLL.Utils.MappingResolvers;
using Studi.DAL.Entities.Courses.Sections;
using Studi.DAL.Repositories.Interfaces.Base;

namespace Studi.BLL.CQRS.Courses.Sections.Update;

public class UpdateSectionHandler : IRequestHandler<UpdateSectionCommand, Result<SectionResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IGoogleDriveStorageService _googleDriveStorageService;
    private readonly ILoggerService _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;
    private readonly IStringLocalizer<NoPermissionsSharedResource> _stringLocalizerNoPermissions;
    private readonly IStringLocalizer<BllSharedResource> _stringLocalizerBll;

    public UpdateSectionHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IGoogleDriveStorageService googleDriveStorageService,
        ILoggerService logger,
        ICurrentUserService currentUserService,
        IStringLocalizer<CannotFindSharedResource> stringLocalizerCannotFind,
        IStringLocalizer<NoPermissionsSharedResource> stringLocalizerNoPermissions,
        IStringLocalizer<BllSharedResource> stringLocalizerBll)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _googleDriveStorageService = googleDriveStorageService;
        _logger = logger;
        _currentUserService = currentUserService;
        _stringLocalizerCannotFind = stringLocalizerCannotFind;
        _stringLocalizerNoPermissions = stringLocalizerNoPermissions;
        _stringLocalizerBll = stringLocalizerBll;
    }

    public async Task<Result<SectionResponseDto>> Handle(UpdateSectionCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        _logger.LogInformation($"Entered '{GetType().Name}' to update a section with Id: {request.SectionUpdateRequestDto.Id} by UserId: {userId}");

        var existingSection = await _repositoryWrapper.SectionsRepository.GetSingleOrDefaultAsync(
            x => x.Id == request.SectionUpdateRequestDto.Id,
            include: IncludeSectionRelatedEntities,
            cancellationToken: cancellationToken);

        if (existingSection is null)
        {
            var errorMessage = _stringLocalizerCannotFind[
                nameof(CannotFindSharedResource_en.CannotFindSectionById),
                request.SectionUpdateRequestDto.Id
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var courseOwnerUserId = existingSection.Course!.OwnerUserId;

        if (courseOwnerUserId != userId)
        {
            var logErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToUpdateSectionForUserWithId),
                request.SectionUpdateRequestDto.Id,
                userId
            ].Value;

            _logger.LogError(request, logErrorMessage);

            var responseErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToUpdateSectionForUser),
                request.SectionUpdateRequestDto.Id
            ].Value;

            return Result.Fail(responseErrorMessage);
        }

        var currentSectionName = existingSection.SectionName;
        var updatedSectionName = NameFromTitleResolver.CreateNameFromTitle(request.SectionUpdateRequestDto.Title);
        var ownerUserEmail = existingSection.Course.OwnerUser?.Email;

        var courseName = existingSection.Course.CourseName;
        var shouldRenameSectionFolder = !string.Equals(currentSectionName, updatedSectionName, StringComparison.Ordinal);

        if (shouldRenameSectionFolder)
        {
            var renameResult = await _googleDriveStorageService.RenameFolderByPathAsync(
                StoragePathHelper.BuildSectionFolderSegments(ownerUserEmail!, courseName, currentSectionName),
                updatedSectionName,
                cancellationToken);

            if (renameResult.IsFailed)
            {
                var errorMessage = renameResult.Errors[0].Message;
                _logger.LogError(request, errorMessage);

                return Result.Fail(errorMessage);
            }
        }

        _mapper.Map(request.SectionUpdateRequestDto, existingSection);

        try
        {
            _repositoryWrapper.SectionsRepository.Update(existingSection);
            await _repositoryWrapper.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            if (shouldRenameSectionFolder)
            {
                var rollbackResult = await _googleDriveStorageService.RenameFolderByPathAsync(
                    StoragePathHelper.BuildSectionFolderSegments(ownerUserEmail!, courseName, updatedSectionName),
                    currentSectionName,
                    cancellationToken);

                if (rollbackResult.IsFailed)
                {
                    _logger.LogError(request, rollbackResult.Errors[0].Message);
                }
            }

            var errorMessage = _stringLocalizerBll[BllSharedResource_en.SectionUpdateFailed].Value;
            _logger.LogError(request, errorMessage, ex.ToString());

            return Result.Fail(errorMessage);
        }

        var sectionResponseDto = _mapper.Map<SectionResponseDto>(existingSection);

        return Result.Ok(sectionResponseDto);
    }

    [ExcludeFromCodeCoverage]
    private static IIncludableQueryable<Section, object> IncludeSectionRelatedEntities(IQueryable<Section> query)
    {
        return query
            .Include(s => s.Course)
                .ThenInclude(c => c!.OwnerUser);
    }
}
