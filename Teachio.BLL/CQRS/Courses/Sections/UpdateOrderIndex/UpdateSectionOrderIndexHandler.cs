using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Localization;
using Teachio.BLL.DTOs.Courses.Sections.Response;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResource;
using Teachio.BLL.Utils.Helpers;
using Teachio.DAL.Repositories.Interfaces.Base;
using SectionEntity = Teachio.DAL.Entities.Courses.Sections.Section;

namespace Teachio.BLL.CQRS.Courses.Sections.UpdateOrderIndex;

public class UpdateSectionOrderIndexHandler : IRequestHandler<UpdateSectionOrderIndexCommand, Result<SectionResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;
    private readonly IStringLocalizer<NoPermissionsSharedResource> _stringLocalizerNoPermissions;

    public UpdateSectionOrderIndexHandler(
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

    public async Task<Result<SectionResponseDto>> Handle(UpdateSectionOrderIndexCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        _logger.LogInformation($"Entered '{GetType().Name}' to update a section order index with Id: {request.SectionUpdateOrderIndexRequestDto.Id} by UserId: {userId}");

        var existingSection = await _repositoryWrapper.SectionsRepository.GetSingleOrDefaultAsync(
            x => x.Id == request.SectionUpdateOrderIndexRequestDto.Id,
            cancellationToken: cancellationToken);

        if (existingSection is null)
        {
            var errorMessage = _stringLocalizerCannotFind[
                nameof(CannotFindSharedResource_en.CannotFindSectionById),
                request.SectionUpdateOrderIndexRequestDto.Id
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var courseOwnerUserId = await _repositoryWrapper.SectionsRepository.GetSingleOrDefaultProjectedAsync(
            s => s.Course!.OwnerUserId,
            s => s.Id == request.SectionUpdateOrderIndexRequestDto.Id,
            cancellationToken);

        if (courseOwnerUserId != userId)
        {
            var logErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToUpdateSectionForUserWithId),
                request.SectionUpdateOrderIndexRequestDto.Id,
                userId
            ].Value;

            _logger.LogError(request, logErrorMessage);

            var responseErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToUpdateSectionForUser),
                request.SectionUpdateOrderIndexRequestDto.Id
            ].Value;

            return Result.Fail(responseErrorMessage);
        }

        var sectionsCount = await _repositoryWrapper.SectionsRepository.GetSelfCountAsync(
            s => s.CourseId == existingSection.CourseId,
            cancellationToken: cancellationToken);

        var targetOrderIndex = PrepareOrderIndexForUpdate(
            existingSection.OrderIndex,
            sectionsCount,
            request.SectionUpdateOrderIndexRequestDto.OrderIndex);

        if (targetOrderIndex != existingSection.OrderIndex)
        {
            await OrderIndexShiftHelper.ShiftOrderIndexesForUpdateAsync(
                _repositoryWrapper.SectionsRepository,
                $"{nameof(SectionEntity)}s",
                nameof(SectionEntity.CourseId),
                existingSection.CourseId,
                nameof(SectionEntity.Id),
                existingSection.Id,
                existingSection.OrderIndex,
                targetOrderIndex,
                cancellationToken);
        }

        existingSection.OrderIndex = targetOrderIndex;

        _repositoryWrapper.SectionsRepository.Update(existingSection);
        await _repositoryWrapper.SaveChangesAsync(cancellationToken);

        var sectionResponseDto = _mapper.Map<SectionResponseDto>(existingSection);

        return Result.Ok(sectionResponseDto);
    }

    private static int PrepareOrderIndexForUpdate(int currentOrderIndex, int sectionsCount, int requestedOrderIndex)
    {
        if (sectionsCount <= 0)
        {
            return currentOrderIndex;
        }

        var maxAllowedOrderIndex = sectionsCount - 1;

        return Math.Min(requestedOrderIndex, maxAllowedOrderIndex);
    }
}
