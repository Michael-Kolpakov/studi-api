using AutoMapper;
using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Sections;
using Teachio.BLL.Services.Interfaces;
using Teachio.DAL.Repositories.Interfaces.Base;
using SectionEntity = Teachio.DAL.Entities.Courses.Sections.Section;

namespace Teachio.BLL.MediatR.Courses.Sections.Create;

public class CreateSectionHandler : IRequestHandler<CreateSectionCommand, Result<SectionResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public CreateSectionHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILoggerService logger)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    public async Task<Result<SectionResponseDto>> Handle(CreateSectionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Entered 'CreateSectionHandler' to create a new course");

        var course = await _repositoryWrapper.CoursesRepository.GetSingleOrDefaultAsync(x =>
            x.Id == request.sectionCreateDto.CourseId);

        if (course is null)
        {
            var errorMessage = $"There is no course with such Id: {request.sectionCreateDto.CourseId}";
            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var newSection = _mapper.Map<SectionEntity>(request.sectionCreateDto);

        await _repositoryWrapper.SectionsRepository.CreateAsync(newSection);
        await _repositoryWrapper.SaveChangesAsync();

        var sectionResponseDto = _mapper.Map<SectionResponseDto>(newSection);

        return Result.Ok(sectionResponseDto);
    }
}
