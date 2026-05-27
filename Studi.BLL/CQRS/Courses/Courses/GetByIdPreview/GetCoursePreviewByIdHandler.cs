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
using Studi.DAL.Repositories.Interfaces.Base;
using CourseEntity = Studi.DAL.Entities.Courses.Courses.Course;

namespace Studi.BLL.CQRS.Courses.Courses.GetByIdPreview;

public class GetCoursePreviewByIdHandler : IRequestHandler<GetCoursePreviewByIdQuery, Result<CoursePreviewResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;

    public GetCoursePreviewByIdHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILoggerService logger,
        ICurrentUserService currentUserService,
        IStringLocalizer<CannotFindSharedResource> stringLocalizerCannotFind)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
        _currentUserService = currentUserService;
        _stringLocalizerCannotFind = stringLocalizerCannotFind;
    }

    public async Task<Result<CoursePreviewResponseDto>> Handle(GetCoursePreviewByIdQuery request, CancellationToken cancellationToken)
    {
        var userInfo = _currentUserService.TryGetUserId(out var userId)
            ? userId.ToString()
            : "anonymous";

        _logger.LogInformation($"Entered '{GetType().Name}' to get course preview by Id: {request.CourseId} by UserId: {userInfo}");

        var course = await _repositoryWrapper.CoursesRepository.GetSingleOrDefaultAsync(
            x => x.Id == request.CourseId,
            IncludeCourseRelatedEntities,
            cancellationToken);

        if (course is null)
        {
            var errorMessage = _stringLocalizerCannotFind[
                nameof(CannotFindSharedResource_en.CannotFindCourseById),
                request.CourseId
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var coursePreviewResponseDto = _mapper.Map<CoursePreviewResponseDto>(course);

        return Result.Ok(coursePreviewResponseDto);
    }

    [ExcludeFromCodeCoverage]
    private static IIncludableQueryable<CourseEntity, object> IncludeCourseRelatedEntities(IQueryable<CourseEntity> query)
    {
        return query
            .Include(c => c.OwnerUser)
            .Include(c => c.Sections)
                .ThenInclude(v => v.Videos)
                    .ThenInclude(v => v.VideoFile!);
    }
}
