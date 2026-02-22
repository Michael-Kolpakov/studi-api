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

namespace Teachio.BLL.MediatR.Courses.Sections.Update;

public class UpdateSectionHandler : IRequestHandler<UpdateSectionCommand, Result<SectionResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IEntityExistenceService _entityExistenceService;
    private readonly ILoggerService _logger;
    private readonly IStringLocalizer<CannotMapSharedResource> _stringLocalizerFailedToMap;

    public UpdateSectionHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IEntityExistenceService entityExistenceService,
        ILoggerService logger,
        IStringLocalizer<CannotMapSharedResource> stringLocalizerFailedToMap)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _entityExistenceService = entityExistenceService;
        _logger = logger;
        _stringLocalizerFailedToMap = stringLocalizerFailedToMap;
    }

    public async Task<Result<SectionResponseDto>> Handle(UpdateSectionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Entered '{GetType().Name}' to update a section with Id: {request.SectionUpdateRequestDto.Id}");

        var section = _mapper.Map<SectionEntity>(request.SectionUpdateRequestDto);

        if (section is null)
        {
            var mappingErrorMessage = _stringLocalizerFailedToMap[nameof(CannotMapSharedResource_en.CannotMapNullToSection)].Value;
            _logger.LogError(request, mappingErrorMessage);

            return Result.Fail(mappingErrorMessage);
        }

        var (courseExists, existenceErrorMessage) = await _entityExistenceService.CheckCourseExistenceAsync(
            request.SectionUpdateRequestDto.CourseId,
            nameof(request.SectionUpdateRequestDto.CourseId),
            request);

        if (courseExists)
        {
            return Result.Fail(existenceErrorMessage);
        }

        _repositoryWrapper.SectionsRepository.Update(section);
        await _repositoryWrapper.SaveChangesAsync();

        var sectionResponseDto = _mapper.Map<SectionResponseDto>(section);

        return Result.Ok(sectionResponseDto);
    }
}
