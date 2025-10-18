using AutoMapper;
using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Sections;
using Teachio.BLL.Services.Interfaces;
using Teachio.DAL.Repositories.Interfaces.Base;
using SectionEntity = Teachio.DAL.Entities.Courses.Sections.Section;

namespace Teachio.BLL.MediatR.Courses.Sections.Update;

public class UpdateSectionHandler : IRequestHandler<UpdateSectionCommand, Result<SectionResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IEntityExistenceService _entityExistenceService;
    private readonly ILoggerService _logger;

    public UpdateSectionHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        IEntityExistenceService entityExistenceService,
        ILoggerService logger)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _entityExistenceService = entityExistenceService;
        _logger = logger;
    }

    public async Task<Result<SectionResponseDto>> Handle(UpdateSectionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Entered 'UpdateSectionHandler' to update a course with Id: {request.sectionUpdateDto.Id}");

        var (courseExists, errorMessage) = await _entityExistenceService.CheckCourseExistenceAsync(
            request.sectionUpdateDto.CourseId,
            request);

        if (courseExists)
        {
            return Result.Fail(errorMessage);
        }

        var section = _mapper.Map<SectionEntity>(request.sectionUpdateDto);

        _repositoryWrapper.SectionsRepository.Update(section);
        await _repositoryWrapper.SaveChangesAsync();

        var sectionResponseDto = _mapper.Map<SectionResponseDto>(section);

        return Result.Ok(sectionResponseDto);
    }
}
