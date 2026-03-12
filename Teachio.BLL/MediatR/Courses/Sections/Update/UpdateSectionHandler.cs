using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Localization;
using Teachio.BLL.Dto.Courses.Sections.Response;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResource;
using Teachio.DAL.Repositories.Interfaces.Base;

namespace Teachio.BLL.MediatR.Courses.Sections.Update;

public class UpdateSectionHandler : IRequestHandler<UpdateSectionCommand, Result<SectionResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;
    private readonly IStringLocalizer<NoPermissionsSharedResource> _stringLocalizerNoPermissions;
    private readonly IStringLocalizer<AlreadyExistsSharedResource> _stringLocalizerAlreadyExists;

    public UpdateSectionHandler(
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

    public async Task<Result<SectionResponseDto>> Handle(UpdateSectionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Entered '{GetType().Name}' to update a section with Id: {request.sectionUpdateRequestDto.Id}");

        var existingSection = await _repositoryWrapper.SectionsRepository.GetSingleOrDefaultAsync(
            x => x.Id == request.sectionUpdateRequestDto.Id,
            cancellationToken: cancellationToken);

        if (existingSection is null)
        {
            var errorMessage = _stringLocalizerCannotFind[
                nameof(CannotFindSharedResource_en.CannotFindSectionById),
                request.sectionUpdateRequestDto.Id
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var courseOwnerUserId = await _repositoryWrapper.SectionsRepository.GetSingleOrDefaultProjectedAsync(
            s => s.Course!.OwnerUserId,
            s => s.Id == request.sectionUpdateRequestDto.Id,
            cancellationToken);

        if (courseOwnerUserId != request.requestingUserId)
        {
            var logErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToUpdateSectionForUserWithId),
                request.sectionUpdateRequestDto.Id,
                request.requestingUserId
            ].Value;

            _logger.LogError(request, logErrorMessage);

            var responseErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToUpdateSectionForUser),
                request.sectionUpdateRequestDto.Id
            ].Value;

            return Result.Fail(responseErrorMessage);
        }

        var sectionWithSameOrderIndexExists = await _repositoryWrapper.SectionsRepository.GetSingleOrDefaultAsync(
            s => s.CourseId == existingSection.CourseId
                 && s.OrderIndex == request.sectionUpdateRequestDto.OrderIndex
                 && s.Id != request.sectionUpdateRequestDto.Id,
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

        _mapper.Map(request.sectionUpdateRequestDto, existingSection);

        // TODO: if user updated title of a section update a section folder name on Google Drive (or CDN in the future)

        _repositoryWrapper.SectionsRepository.Update(existingSection);
        await _repositoryWrapper.SaveChangesAsync(cancellationToken);

        var sectionResponseDto = _mapper.Map<SectionResponseDto>(existingSection);

        return Result.Ok(sectionResponseDto);
    }
}
