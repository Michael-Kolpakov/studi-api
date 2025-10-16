using AutoMapper;
using FluentResults;
using MediatR;
using Teachio.BLL.Dto.Courses.Sections;
using Teachio.BLL.Services.Interfaces;
using Teachio.DAL.Repositories.Interfaces.Base;

namespace Teachio.BLL.MediatR.Courses.Sections.GetById;

public class GetSectionByIdHandler : IRequestHandler<GetSectionByIdQuery, Result<SectionResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public GetSectionByIdHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILoggerService logger)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    public async Task<Result<SectionResponseDto>> Handle(GetSectionByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Entered 'GetSectionByIdHandler' to get course by Id: {request.id}");

        var section = await _repositoryWrapper.SectionsRepository.GetSingleOrDefaultAsync(x => x.Id == request.id);

        if (section is null)
        {
            var errorMessage = $"There is no section with such Id: {request.id}";
            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var sectionResponseDto = _mapper.Map<SectionResponseDto>(section);

        return Result.Ok(sectionResponseDto);
    }
}
