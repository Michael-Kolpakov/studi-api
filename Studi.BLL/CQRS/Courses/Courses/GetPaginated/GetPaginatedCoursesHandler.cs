using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Localization;
using Studi.BLL.DTOs.Courses.Courses.Response;
using Studi.BLL.Resources.SharedResource;
using Studi.BLL.Services.Interfaces;
using Studi.BLL.SharedResource;
using Studi.BLL.Utils.Helpers;
using Studi.DAL.Entities.Courses.Courses;
using Studi.DAL.Repositories.Interfaces.Base;
using Studi.DAL.Utils.Constants;
using Studi.DAL.Utils.Helpers;

namespace Studi.BLL.CQRS.Courses.Courses.GetPaginated;

public class GetPaginatedCoursesHandler : IRequestHandler<GetPaginatedCoursesQuery, Result<PaginatedCoursesResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<BllSharedResource> _stringLocalizerBll;

    public GetPaginatedCoursesHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILoggerService logger,
        ICurrentUserService currentUserService,
        IStringLocalizer<BllSharedResource> stringLocalizerBll)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
        _currentUserService = currentUserService;
        _stringLocalizerBll = stringLocalizerBll;
    }

    public async Task<Result<PaginatedCoursesResponseDto>> Handle(GetPaginatedCoursesQuery request, CancellationToken cancellationToken)
    {
        var isAuthenticated = _currentUserService.TryGetUserId(out var userId);
        var normalizedTitleFilter = NormalizeTitleFilter(request.TitleFilter);

        if (!IsTitleFilterValid(normalizedTitleFilter, out var validationErrorMessage))
        {
            _logger.LogError(request, validationErrorMessage);

            return Result.Fail(validationErrorMessage);
        }

        var titleFilterInfo = normalizedTitleFilter ?? "none";
        var resolvedSortDirection = CourseSortResolver.ResolveSortDirection(request.SortBy, request.SortDirection);

        var userInfo = isAuthenticated ? userId.ToString() : "anonymous";

        _logger.LogInformation($"Entered '{GetType().Name}' to get paginated courses (page number: {request.PageNumber}, page size: {request.PageSize}, mode: {request.Mode}, title filter: {titleFilterInfo}, sort by: {request.SortBy}, sort direction: {resolvedSortDirection}) by UserId: {userInfo}");

        if (!isAuthenticated && request.Mode == CoursesPaginationMode.InProgress)
        {
            return Result.Ok(new PaginatedCoursesResponseDto()
            {
                TotalAmount = 0,
                Courses = []
            });
        }

        var predicate = isAuthenticated
            ? request.Mode switch
            {
                CoursesPaginationMode.Available => CourseFilterHelper.BuildAvailableCoursesPredicate(
                    userId,
                    normalizedTitleFilter),
                CoursesPaginationMode.InProgress => CourseFilterHelper.BuildInProgressCoursesPredicate(
                    userId,
                    normalizedTitleFilter),
                CoursesPaginationMode.Personal => CourseFilterHelper.BuildPersonalCoursesPredicate(
                    userId,
                    normalizedTitleFilter),
                _ => CourseFilterHelper.BuildAvailableCoursesPredicate(userId, normalizedTitleFilter)
            }
            : CourseFilterHelper.BuildAnonymousCoursesPredicate(normalizedTitleFilter);

        var (primaryAscending, primaryDescending, secondaryAscending, secondaryDescending) =
            CourseSortResolver.BuildSortSelectors(request.SortBy, resolvedSortDirection);

        var paginatedCourses = await _repositoryWrapper.CoursesRepository.GetAllPaginatedAsync(
            request.PageNumber,
            request.PageSize,
            predicate: predicate,
            include: IncludeCourseRelatedEntities,
            ascendingSortKeySelector: primaryAscending,
            descendingSortKeySelector: primaryDescending,
            secondaryAscendingSortKeySelector: secondaryAscending,
            secondaryDescendingSortKeySelector: secondaryDescending,
            cancellationToken: cancellationToken);

        var getAllCoursesResponseDto = new PaginatedCoursesResponseDto()
        {
            TotalAmount = paginatedCourses.TotalItems,
            Courses = _mapper.Map<IEnumerable<CoursePreviewShortResponseDto>>(paginatedCourses.Entities)
        };

        return Result.Ok(getAllCoursesResponseDto);
    }

    [ExcludeFromCodeCoverage]
    private static IIncludableQueryable<Course, object> IncludeCourseRelatedEntities(IQueryable<Course> query)
    {
        return query
            .Include(c => c.OwnerUser)
            .Include(s => s.Sections)
                .ThenInclude(v => v.Videos)
                    .ThenInclude(v => v.VideoFile)
                .Include(s => s.Sections)
                    .ThenInclude(v => v.Videos)
                        .ThenInclude(v => v.VideoProgresses);
    }

    private static string? NormalizeTitleFilter(string? titleFilter)
    {
        if (string.IsNullOrWhiteSpace(titleFilter))
        {
            return null;
        }

        return titleFilter.Trim();
    }

    private bool IsTitleFilterValid(string? titleFilter, out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(titleFilter))
        {
            errorMessage = string.Empty;

            return true;
        }

        if (titleFilter.Length > EntityConstants.MaxCourseTitleLength)
        {
            errorMessage = _stringLocalizerBll[
                nameof(BllSharedResource_en.TitleFilterLengthTooLong),
                EntityConstants.MaxCourseTitleLength
            ].Value;

            return false;
        }

        errorMessage = string.Empty;

        return true;
    }
}
