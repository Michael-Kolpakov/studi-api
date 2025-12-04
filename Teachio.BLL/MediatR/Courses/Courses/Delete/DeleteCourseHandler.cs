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

namespace Teachio.BLL.MediatR.Courses.Courses.Delete;

public class DeleteCourseHandler : IRequestHandler<DeleteCourseCommand, Result<CourseResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    private readonly IStringLocalizer<CannotFindSharedResource> _stringLocalizerCannotFind;

    public DeleteCourseHandler(
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

    public async Task<Result<CourseResponseDto>> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Entered '{GetType().Name}' to delete a course with Id: {request.id}");

        // TODO: validate whether gained course really belongs to the user making the request (and perhaps remove check below)

        var course = await _repositoryWrapper.CoursesRepository.GetSingleOrDefaultAsync(
            x => x.Id == request.id,
            q => q
                .Include(c => c.Sections)
                    .ThenInclude(s => s.Videos)
                        .ThenInclude(v => v.VideoProgress));

        if (course is null)
        {
            var errorMessage = _stringLocalizerCannotFind[nameof(CannotFindSharedResource_en.CannotFindCourseById), request.id].Value;
            _logger.LogError(request, errorMessage);

            return Result.Fail(errorMessage);
        }

        // TODO: make sure whether we really delete all course dependent entities: Sections, Videos and VideoProgress

        // TODO: address Google Drive API (or CDN in the future) to delete thumbnail image

        var watchingUsersCount = await _repositoryWrapper.CoursesRepository.GetNavigationCollectionsCountAsync(
            course => course.WatchingUsers,
            x => x.Id == request.id);

        _repositoryWrapper.CoursesRepository.Delete(course);
        await _repositoryWrapper.SaveChangesAsync();

        var courseResponseDto = _mapper.Map<CourseResponseDto>(course);
        courseResponseDto.WatchingUsersCount = watchingUsersCount;

        return Result.Ok(courseResponseDto);
    }
}
