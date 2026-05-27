using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Localization;
using Studi.BLL.DTOs.Courses.Sections.Response;
using Studi.BLL.Resources.SharedResource;
using Studi.BLL.Services.Interfaces;
using Studi.BLL.SharedResource;
using Studi.DAL.Repositories.Interfaces.Base;
using Studi.DAL.Utils.Constants;
using SectionEntity = Studi.DAL.Entities.Courses.Sections.Section;

namespace Studi.BLL.CQRS.Courses.Sections.Create;

public class CreateSectionHandler : IRequestHandler<CreateSectionCommand, Result<SectionResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IEntityExistenceService _entityExistenceService;
    private readonly ILoggerService _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<CannotMapSharedResource> _stringLocalizerFailedToMap;
    private readonly IStringLocalizer<NoPermissionsSharedResource> _stringLocalizerNoPermissions;
    private readonly IStringLocalizer<BllSharedResource> _stringLocalizerBll;

    public CreateSectionHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IEntityExistenceService entityExistenceService,
        ILoggerService logger,
        ICurrentUserService currentUserService,
        IStringLocalizer<CannotMapSharedResource> stringLocalizerFailedToMap,
        IStringLocalizer<NoPermissionsSharedResource> stringLocalizerNoPermissions,
        IStringLocalizer<BllSharedResource> stringLocalizerBll)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _entityExistenceService = entityExistenceService;
        _logger = logger;
        _currentUserService = currentUserService;
        _stringLocalizerFailedToMap = stringLocalizerFailedToMap;
        _stringLocalizerNoPermissions = stringLocalizerNoPermissions;
        _stringLocalizerBll = stringLocalizerBll;
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

        var courseSectionsCount = await _repositoryWrapper.SectionsRepository.GetSelfCountAsync(
            s => s.CourseId == newSection.CourseId,
            cancellationToken: cancellationToken);

        if (courseSectionsCount >= EntityConstants.MaxSectionsPerCourse)
        {
            var errorMessage = _stringLocalizerBll[
                nameof(BllSharedResource_en.SectionsCountExceedsLimit),
                newSection.CourseId,
                EntityConstants.MaxSectionsPerCourse
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        newSection.OrderIndex = courseSectionsCount;

        await _repositoryWrapper.SectionsRepository.CreateAsync(newSection, cancellationToken);

        course.SectionsCount = courseSectionsCount + 1;

        _repositoryWrapper.CoursesRepository.Update(course);
        await _repositoryWrapper.SaveChangesAsync(cancellationToken);

        var sectionResponseDto = _mapper.Map<SectionResponseDto>(newSection);

        return Result.Ok(sectionResponseDto);
    }
}
