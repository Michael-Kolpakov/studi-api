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
using Teachio.DAL.Repositories.Interfaces.Base;
using SectionEntity = Teachio.DAL.Entities.Courses.Sections.Section;

namespace Teachio.BLL.CQRS.Courses.Sections.GetByCourseId;

public class GetSectionsByCourseIdHandler : IRequestHandler<GetSectionsByCourseIdQuery, Result<CourseSectionsResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICourseAccessService _courseAccessService;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;
    private readonly IStringLocalizer<NoPermissionsSharedResource> _stringLocalizerNoPermissions;

    public GetSectionsByCourseIdHandler(
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

    public async Task<Result<CourseSectionsResponseDto>> Handle(GetSectionsByCourseIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        _logger.LogInformation($"Entered '{GetType().Name}' to get sections by CourseId: {request.CourseId} by UserId: {userId}");

        var course = await _repositoryWrapper.CoursesRepository.GetSingleOrDefaultAsync(
            x => x.Id == request.CourseId,
            cancellationToken: cancellationToken);

        if (course is null)
        {
            var errorMessage = _stringLocalizerCannotFind[
                nameof(CannotFindSharedResource_en.CannotFindCourseById),
                request.CourseId
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var hasAccess = await _courseAccessService.HasAccessToCourseAsync(course.Id, userId, cancellationToken);
        if (!hasAccess)
        {
            var errorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToGetCourseForUser),
                request.CourseId
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var sections = await _repositoryWrapper.SectionsRepository.GetAllAsync(
            section => section.CourseId == request.CourseId,
            IncludeSectionRelatedEntities,
            cancellationToken: cancellationToken);

        var orderedSections = sections
            .OrderBy(section => section.OrderIndex)
            .ToList();

        var sectionsResponseDto = new CourseSectionsResponseDto
        {
            Sections = _mapper.Map<List<SectionEditPreviewResponseDto>>(orderedSections)
        };

        return Result.Ok(sectionsResponseDto);
    }

    [ExcludeFromCodeCoverage]
    private static IIncludableQueryable<SectionEntity, object> IncludeSectionRelatedEntities(IQueryable<SectionEntity> query)
    {
        return query
            .Include(section => section.Videos)
                .ThenInclude(video => video.VideoFile!);
    }
}
