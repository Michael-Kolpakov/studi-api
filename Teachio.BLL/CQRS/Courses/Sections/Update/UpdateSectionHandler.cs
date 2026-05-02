using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Localization;
using Teachio.BLL.DTOs.Courses.Sections.Response;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResources;
using Teachio.DAL.Repositories.Interfaces.Base;

namespace Teachio.BLL.CQRS.Courses.Sections.Update;

public class UpdateSectionHandler : IRequestHandler<UpdateSectionCommand, Result<SectionResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;
    private readonly IStringLocalizer<NoPermissionsSharedResource> _stringLocalizerNoPermissions;
    private readonly IStringLocalizer<AlreadyExistsSharedResource> _stringLocalizerAlreadyExists;

    public UpdateSectionHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILoggerService logger,
        ICurrentUserService currentUserService,
        IStringLocalizer<CannotFindSharedResource> stringLocalizerCannotFind,
        IStringLocalizer<NoPermissionsSharedResource> stringLocalizerNoPermissions,
        IStringLocalizer<AlreadyExistsSharedResource> stringLocalizerAlreadyExists)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
        _currentUserService = currentUserService;
        _stringLocalizerCannotFind = stringLocalizerCannotFind;
        _stringLocalizerNoPermissions = stringLocalizerNoPermissions;
        _stringLocalizerAlreadyExists = stringLocalizerAlreadyExists;
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

        var sectionWithSameOrderIndexExists = await _repositoryWrapper.SectionsRepository.GetSingleOrDefaultAsync(
            s => s.CourseId == existingSection.CourseId
                 && s.OrderIndex == request.SectionUpdateRequestDto.OrderIndex
                 && s.Id != request.SectionUpdateRequestDto.Id,
            cancellationToken: cancellationToken);

        if (sectionWithSameOrderIndexExists is not null)
        {
            var errorMessage = _stringLocalizerAlreadyExists[
                nameof(AlreadyExistsSharedResource_en.SectionAlreadyExistsForCourse),
                sectionWithSameOrderIndexExists.Id,
                sectionWithSameOrderIndexExists.CourseId,
                sectionWithSameOrderIndexExists.OrderIndex
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        _mapper.Map(request.SectionUpdateRequestDto, existingSection);

        // TODO: if user updated title of a section update a section folder name on Google Drive (or CDN in the future)

        _repositoryWrapper.SectionsRepository.Update(existingSection);
        await _repositoryWrapper.SaveChangesAsync(cancellationToken);

        var sectionResponseDto = _mapper.Map<SectionResponseDto>(existingSection);

        return Result.Ok(sectionResponseDto);
    }
}
