using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Localization;
using Teachio.BLL.Dto.Courses.Sections.Response;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResource;
using Teachio.DAL.Repositories.Interfaces.Base;

namespace Teachio.BLL.MediatR.Courses.Sections.Delete;

public class DeleteSectionHandler : IRequestHandler<DeleteSectionCommand, Result<SectionResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;

    public DeleteSectionHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILoggerService logger,
        IStringLocalizer<CannotFindSharedResource> stringLocalizerCannotFind)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
        _stringLocalizerCannotFind = stringLocalizerCannotFind;
    }

    public async Task<Result<SectionResponseDto>> Handle(DeleteSectionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Entered '{GetType().Name}' to delete a section with Id: {request.id}");

        var section = await _repositoryWrapper.SectionsRepository.GetSingleOrDefaultAsync(x => x.Id == request.id);

        if (section is null)
        {
            var errorMessage = _stringLocalizerCannotFind[nameof(CannotFindSharedResource_en.CannotFindSectionById), request.id].Value;
            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        // TODO: make sure whether we really delete all section dependent entities: Videos and VideoProgress

        _repositoryWrapper.SectionsRepository.Delete(section);
        await _repositoryWrapper.SaveChangesAsync();

        var sectionResponseDto = _mapper.Map<SectionResponseDto>(section);

        return Result.Ok(sectionResponseDto);
    }
}
