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
using Teachio.DAL.Utils.Constants;
using SectionEntity = Teachio.DAL.Entities.Courses.Sections.Section;

namespace Teachio.BLL.CQRS.Courses.Sections.Create;

public class CreateSectionHandler : IRequestHandler<CreateSectionCommand, Result<SectionResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IEntityExistenceService _entityExistenceService;
    private readonly ILoggerService _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<CannotMapSharedResource> _stringLocalizerFailedToMap;
    private readonly IStringLocalizer<NoPermissionsSharedResource> _stringLocalizerNoPermissions;

    public CreateSectionHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IEntityExistenceService entityExistenceService,
        ILoggerService logger,
        ICurrentUserService currentUserService,
        IStringLocalizer<CannotMapSharedResource> stringLocalizerFailedToMap,
        IStringLocalizer<NoPermissionsSharedResource> stringLocalizerNoPermissions)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _entityExistenceService = entityExistenceService;
        _logger = logger;
        _currentUserService = currentUserService;
        _stringLocalizerFailedToMap = stringLocalizerFailedToMap;
        _stringLocalizerNoPermissions = stringLocalizerNoPermissions;
    }

    public async Task<Result<SectionResponseDto>> Handle(CreateSectionCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        _logger.LogInformation($"Entered '{GetType().Name}' to create a new section by UserId: {userId}");

        var newSection = _mapper.Map<SectionEntity>(request.SectionCreateRequestDto);

        if (newSection is null)
        {
            var mappingErrorMessage = _stringLocalizerFailedToMap[nameof(CannotMapSharedResource_en.CannotMapNullToSection)].Value;
            _logger.LogError(request, mappingErrorMessage);

            return Result.Fail(mappingErrorMessage);
        }

        var (course, existenceErrorMessage) = await _entityExistenceService.CheckCourseExistenceAsync(
            request.SectionCreateRequestDto.CourseId,
            nameof(request.SectionCreateRequestDto.CourseId),
            cancellationToken);

        if (course is null)
        {
            _logger.LogError(request, existenceErrorMessage!);

            return Result.Fail(existenceErrorMessage);
        }

        if (course.OwnerUserId != userId)
        {
            var logErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToCreateSectionForCourseOfAnotherUserWithId),
                request.SectionCreateRequestDto.CourseId,
                userId,
                course.OwnerUserId
            ].Value;

            _logger.LogError(request, logErrorMessage);

            var responseErrorMessage = _stringLocalizerNoPermissions[
                nameof(NoPermissionsSharedResource_en.NoPermissionsToCreateSectionForCourseOfAnotherUser)
            ].Value;

            return Result.Fail(responseErrorMessage);
        }

        var courseSections = (await _repositoryWrapper.SectionsRepository.GetAllAsync(
                s => s.CourseId == newSection.CourseId,
                cancellationToken: cancellationToken))
            .OrderBy(s => s.OrderIndex)
            .ToList();

        var targetOrderIndex = PrepareOrderIndexForCreate(courseSections, request.SectionCreateRequestDto.OrderIndex);
        newSection.OrderIndex = targetOrderIndex;

        if (courseSections.Count > 0)
        {
            await ShiftOrderIndexesForCreateAsync(
                request.SectionCreateRequestDto.CourseId,
                targetOrderIndex,
                cancellationToken);
        }

        await _repositoryWrapper.SectionsRepository.CreateAsync(newSection, cancellationToken);

        course.SectionsCount = courseSections.Count + 1;

        _repositoryWrapper.CoursesRepository.Update(course);
        await _repositoryWrapper.SaveChangesAsync(cancellationToken);

        var sectionResponseDto = _mapper.Map<SectionResponseDto>(newSection);

        return Result.Ok(sectionResponseDto);
    }

    private async Task ShiftOrderIndexesForCreateAsync(
        Guid courseId,
        int targetOrderIndex,
        CancellationToken cancellationToken)
    {
        var table = $"[{DatabaseConstants.CoursesSchema}].[{nameof(Section)}s]";

        var sql = $"""
            UPDATE {table}
            SET {nameof(SectionEntity.OrderIndex)} = {nameof(SectionEntity.OrderIndex)} + 1
            WHERE {nameof(SectionEntity.CourseId)} = '{courseId}'
                AND {nameof(SectionEntity.OrderIndex)} >= {targetOrderIndex}
        """;

        await _repositoryWrapper.SectionsRepository.ExecuteSqlRaw(sql, cancellationToken);
    }

    private static int PrepareOrderIndexForCreate(List<SectionEntity> courseSections, int requestedOrderIndex)
    {
        OrderIndexHelper.NormalizeOrderIndexes(courseSections);

        var targetOrderIndex = Math.Min(requestedOrderIndex, courseSections.Count);

        foreach (var section in courseSections.Where(s => s.OrderIndex >= targetOrderIndex))
        {
            section.OrderIndex++;
        }

        return targetOrderIndex;
    }
}
