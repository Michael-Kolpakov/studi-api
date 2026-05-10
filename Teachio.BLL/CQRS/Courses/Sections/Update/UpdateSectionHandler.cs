using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Localization;
using Teachio.BLL.DTOs.Courses.Sections.Response;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResource;
using Teachio.BLL.Utils.Helpers;
using Teachio.DAL.Entities.Courses.Sections;
using Teachio.DAL.Repositories.Interfaces.Base;
using SectionEntity = Teachio.DAL.Entities.Courses.Sections.Section;

namespace Teachio.BLL.CQRS.Courses.Sections.Update;

public class UpdateSectionHandler : IRequestHandler<UpdateSectionCommand, Result<SectionResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;
    private readonly IStringLocalizer<NoPermissionsSharedResource> _stringLocalizerNoPermissions;

    public UpdateSectionHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILoggerService logger,
        ICurrentUserService currentUserService,
        IStringLocalizer<CannotFindSharedResource> stringLocalizerCannotFind,
        IStringLocalizer<NoPermissionsSharedResource> stringLocalizerNoPermissions)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
        _currentUserService = currentUserService;
        _stringLocalizerCannotFind = stringLocalizerCannotFind;
        _stringLocalizerNoPermissions = stringLocalizerNoPermissions;
    }

    public async Task<Result<SectionResponseDto>> Handle(UpdateSectionCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        _logger.LogInformation($"Entered '{GetType().Name}' to update a section with Id: {request.SectionUpdateRequestDto.Id} by UserId: {userId}");

        var existingSection = await _repositoryWrapper.SectionsRepository.GetSingleOrDefaultAsync(
            x => x.Id == request.SectionUpdateRequestDto.Id,
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

        var courseOwnerUserId = await _repositoryWrapper.SectionsRepository.GetSingleOrDefaultProjectedAsync(
            s => s.Course!.OwnerUserId,
            s => s.Id == request.SectionUpdateRequestDto.Id,
            cancellationToken);

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

        var sectionsCount = await _repositoryWrapper.SectionsRepository.GetSelfCountAsync(
            s => s.CourseId == existingSection.CourseId,
            cancellationToken: cancellationToken);

        var targetOrderIndex = PrepareOrderIndexForUpdate(
            existingSection.OrderIndex,
            sectionsCount,
            request.SectionUpdateRequestDto.OrderIndex);

        if (targetOrderIndex != existingSection.OrderIndex)
        {
            await OrderIndexShiftHelper.ShiftOrderIndexesForUpdateAsync(
                _repositoryWrapper.SectionsRepository,
                $"{nameof(Section)}s",
                nameof(SectionEntity.CourseId),
                existingSection.CourseId,
                nameof(SectionEntity.Id),
                existingSection.Id,
                existingSection.OrderIndex,
                targetOrderIndex,
                cancellationToken);
        }

        _mapper.Map(request.SectionUpdateRequestDto, existingSection);
        existingSection.OrderIndex = targetOrderIndex;

        // TODO: if user updated title of a section update a section folder name on Google Drive (or CDN in the future)

        _repositoryWrapper.SectionsRepository.Update(existingSection);
        await _repositoryWrapper.SaveChangesAsync(cancellationToken);

        var sectionResponseDto = _mapper.Map<SectionResponseDto>(existingSection);

        return Result.Ok(sectionResponseDto);
    }

    private static int PrepareOrderIndexForUpdate(
        int currentOrderIndex,
        int sectionsCount,
        int requestedOrderIndex)
    {
        if (sectionsCount <= 0)
        {
            return currentOrderIndex;
        }

        return Math.Min(requestedOrderIndex, sectionsCount - 1);
    }
}
