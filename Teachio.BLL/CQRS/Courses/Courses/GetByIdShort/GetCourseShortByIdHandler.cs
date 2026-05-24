using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Localization;
using Teachio.BLL.DTOs.Courses.Courses.Response;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResource;
using Teachio.DAL.Repositories.Interfaces.Base;

namespace Teachio.BLL.CQRS.Courses.Courses.GetByIdShort;

public class GetCourseShortByIdHandler : IRequestHandler<GetCourseShortByIdQuery, Result<CourseEditShortResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;

    public GetCourseShortByIdHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILoggerService logger,
        ICurrentUserService currentUserService,
        IStringLocalizer<CannotFindSharedResource> stringLocalizerCannotFind)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
        _currentUserService = currentUserService;
        _stringLocalizerCannotFind = stringLocalizerCannotFind;
    }

    public async Task<Result<CourseEditShortResponseDto>> Handle(GetCourseShortByIdQuery request, CancellationToken cancellationToken)
    {
        var userInfo = _currentUserService.TryGetUserId(out var userId)
            ? userId.ToString()
            : "anonymous";

        _logger.LogInformation($"Entered '{GetType().Name}' to get course short by Id: {request.CourseId} by UserId: {userInfo}");

        var course = await _repositoryWrapper.CoursesRepository.GetSingleOrDefaultAsync(
            x => x.Id == request.CourseId,
            cancellationToken: cancellationToken);

        if (course is null)
        {
            var errorMessage = _stringLocalizerCannotFind[
                nameof(CannotFindSharedResource_en.CannotFindCourseById),
                request.CourseId
            ].Value;

            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        var courseShortResponseDto = _mapper.Map<CourseEditShortResponseDto>(course);

        return Result.Ok(courseShortResponseDto);
    }
}
