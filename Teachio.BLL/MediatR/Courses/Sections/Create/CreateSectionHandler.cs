using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Localization;
using Teachio.BLL.Dto.Courses.Sections.Response;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResource;
using Teachio.DAL.Repositories.Interfaces.Base;
using SectionEntity = Teachio.DAL.Entities.Courses.Sections.Section;

namespace Teachio.BLL.MediatR.Courses.Sections.Create;

public class CreateSectionHandler : IRequestHandler<CreateSectionCommand, Result<SectionResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IEntityExistenceService _entityExistenceService;
    private readonly ILoggerService _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<CannotMapSharedResource> _stringLocalizerFailedToMap;
    private readonly IStringLocalizer<NoPermissionsSharedResource> _stringLocalizerNoPermissions;
    private readonly IStringLocalizer<AlreadyExistsSharedResource> _stringLocalizerAlreadyExists;

    public CreateSectionHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IEntityExistenceService entityExistenceService,
        ILoggerService logger,
        ICurrentUserService currentUserService,
        IStringLocalizer<CannotMapSharedResource> stringLocalizerFailedToMap,
        IStringLocalizer<NoPermissionsSharedResource> stringLocalizerNoPermissions,
        IStringLocalizer<AlreadyExistsSharedResource> stringLocalizerAlreadyExists)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _entityExistenceService = entityExistenceService;
        _logger = logger;
        _currentUserService = currentUserService;
        _stringLocalizerFailedToMap = stringLocalizerFailedToMap;
        _stringLocalizerNoPermissions = stringLocalizerNoPermissions;
        _stringLocalizerAlreadyExists = stringLocalizerAlreadyExists;
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

        var sectionWithSameOrderIndexExists = await _repositoryWrapper.SectionsRepository.GetSingleOrDefaultAsync(
            s => s.CourseId == newSection.CourseId && s.OrderIndex == newSection.OrderIndex,
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

        await _repositoryWrapper.SectionsRepository.CreateAsync(newSection, cancellationToken);
        await _repositoryWrapper.SaveChangesAsync(cancellationToken);

        var sectionResponseDto = _mapper.Map<SectionResponseDto>(newSection);

        return Result.Ok(sectionResponseDto);
    }
}
