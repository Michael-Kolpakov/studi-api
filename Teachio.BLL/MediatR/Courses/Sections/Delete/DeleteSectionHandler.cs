using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Localization;
using Teachio.BLL.Dto.Courses.Sections.Response;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResource;
using Teachio.DAL.Repositories.Interfaces.Base;
using SectionEntity = Teachio.DAL.Entities.Courses.Sections.Section;

namespace Teachio.BLL.MediatR.Courses.Sections.Delete;

public class DeleteSectionHandler : IRequestHandler<DeleteSectionCommand, Result<SectionResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;
    private readonly IStringLocalizer<NoPermissionsSharedResource> _stringLocalizerNoPermissions;

    public DeleteSectionHandler(
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

    public async Task<Result<SectionResponseDto>> Handle(DeleteSectionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Entered '{GetType().Name}' to delete a section with Id: {request.sectionId}");

        var section = await _repositoryWrapper.SectionsRepository.GetSingleOrDefaultAsync(
            x => x.Id == request.sectionId,
            IncludeSectionRelatedEntities);

        if (section is null)
        {
            var errorMessage = _stringLocalizerCannotFind[nameof(CannotFindSharedResource_en.CannotFindSectionById), request.sectionId].Value;
            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        if (section.Course?.OwnerUserId != request.requestingUserId)
        {
            var logErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToDeleteSectionForUserWithId),
                request.sectionId,
                request.requestingUserId
            ].Value;

            _logger.LogError(request, logErrorMessage);

            var responseErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToDeleteSectionForUser),
                request.sectionId
            ].Value;

            return Result.Fail(responseErrorMessage);
        }

        // TODO: make sure whether we really delete all section dependent entities: Videos and VideoProgress

        _repositoryWrapper.SectionsRepository.Delete(section);
        await _repositoryWrapper.SaveChangesAsync();

        var sectionResponseDto = _mapper.Map<SectionResponseDto>(section);

        return Result.Ok(sectionResponseDto);
    }

    [ExcludeFromCodeCoverage]
    private static IIncludableQueryable<SectionEntity, object> IncludeSectionRelatedEntities(IQueryable<SectionEntity> query)
    {
        return query
            .Include(s => s.Videos)
                .ThenInclude(v => v.VideoProgress);
    }
}
