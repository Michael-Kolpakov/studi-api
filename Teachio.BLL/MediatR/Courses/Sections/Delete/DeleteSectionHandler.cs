using AutoMapper;
using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Sections;
using Teachio.BLL.Services.Interfaces;
using Teachio.DAL.Repositories.Interfaces.Base;

namespace Teachio.BLL.MediatR.Courses.Sections.Delete;

public class DeleteSectionHandler : IRequestHandler<DeleteSectionCommand, Result<SectionResponseDto>>
{
    private const string _notFoundErrorMessage = "There is no section with such Id";

    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public DeleteSectionHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILoggerService logger)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    public async Task<Result<SectionResponseDto>> Handle(DeleteSectionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Entered 'DeleteSectionHandler' to delete a course with Id: {request.id}");

        var section = await _repositoryWrapper.SectionsRepository.GetSingleOrDefaultAsync(x => x.Id == request.id);

        if (section is null)
        {
            _logger.LogError(request, _notFoundErrorMessage);

            return Result.Fail(_notFoundErrorMessage);
        }

        _repositoryWrapper.SectionsRepository.Delete(section);
        await _repositoryWrapper.SaveChangesAsync();

        var sectionResponseDto = _mapper.Map<SectionResponseDto>(section);

        return Result.Ok(sectionResponseDto);
    }
}
