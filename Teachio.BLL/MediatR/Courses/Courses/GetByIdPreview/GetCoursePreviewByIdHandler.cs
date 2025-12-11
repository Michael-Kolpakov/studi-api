using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Teachio.BLL.Dto.Courses.Courses.Response;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResource;
using Teachio.DAL.Repositories.Interfaces.Base;

namespace Teachio.BLL.MediatR.Courses.Courses.GetByIdPreview;

public class GetCoursePreviewByIdHandler : IRequestHandler<GetCoursePreviewByIdQuery, Result<CoursePreviewResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;

    public GetCoursePreviewByIdHandler(
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

    public async Task<Result<CoursePreviewResponseDto>> Handle(
        GetCoursePreviewByIdQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Entered '{GetType().Name}' to get course preview by Id: {request.id}");

        var coursePreview = await _repositoryWrapper.CoursesRepository.GetSingleOrDefaultAsync(
            x => x.Id == request.id,
            q => q
                .Include(c => c.OwnerUser)
                .Include(c => c.Sections)
                    .ThenInclude(v => v.Videos));

        if (coursePreview is null)
        {
            var errorMessage = _stringLocalizerCannotFind[nameof(CannotFindSharedResource_en.CannotFindCourseById), request.id].Value;
            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var coursePreviewResponseDto = _mapper.Map<CoursePreviewResponseDto>(coursePreview);

        return Result.Ok(coursePreviewResponseDto);
    }
}
