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
using Studi.DAL.Entities.Courses.Sections;
using Studi.DAL.Repositories.Interfaces.Base;

namespace Studi.BLL.CQRS.Courses.Sections.GetById;

public class GetSectionByIdHandler : IRequestHandler<GetSectionByIdQuery, Result<SectionResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICourseAccessService _courseAccessService;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;
    private readonly IStringLocalizer<NoPermissionsSharedResource> _stringLocalizerNoPermissions;

    public GetSectionByIdHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILoggerService logger,
        ICurrentUserService currentUserService,
        ICourseAccessService courseAccessService,
        IStringLocalizer<CannotFindSharedResource> stringLocalizerCannotFind,
        IStringLocalizer<NoPermissionsSharedResource> stringLocalizerNoPermissions)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
        _currentUserService = currentUserService;
        _courseAccessService = courseAccessService;
        _stringLocalizerCannotFind = stringLocalizerCannotFind;
        _stringLocalizerNoPermissions = stringLocalizerNoPermissions;
    }

    public async Task<Result<SectionResponseDto>> Handle(GetSectionByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        _logger.LogInformation($"Entered '{GetType().Name}' to get section by Id: {request.SectionId} by UserId: {userId}");

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

        var hasAccess = await _courseAccessService.HasAccessToSectionAsync(section.Id, userId, cancellationToken);
        if (!hasAccess)
        {
            var errorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToGetSectionForUser),
                request.SectionId
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var sectionResponseDto = _mapper.Map<SectionResponseDto>(section);

        foreach (var videoResponseDto in sectionResponseDto.Videos)
        {
            var video = section.Videos.FirstOrDefault(v => v.Id == videoResponseDto.Id);
            videoResponseDto.VideoProgress = VideoProgressHelper.BuildResponse(video, userId);
        }

        return Result.Ok(sectionResponseDto);
    }

    [ExcludeFromCodeCoverage]
    private static IIncludableQueryable<Section, object> IncludeSectionRelatedEntities(IQueryable<Section> query)
    {
        return query
            .Include(s => s.Videos)
                .ThenInclude(v => v.VideoProgresses)
            .Include(s => s.Videos)
                .ThenInclude(v => v.VideoFile!);
    }
}
