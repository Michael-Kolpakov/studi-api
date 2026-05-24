using Teachio.BLL.Services.Interfaces;
using Teachio.DAL.Repositories.Interfaces.Base;

namespace Teachio.BLL.Services.Realizations;

public class CourseAccessService : ICourseAccessService
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public CourseAccessService(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<bool> HasAccessToCourseAsync(Guid courseId, Guid userId, CancellationToken cancellationToken = default)
    {
        var accessContext = await _repositoryWrapper.CoursesRepository.GetSingleOrDefaultProjectedAsync(
            course => new CourseAccessContext
            {
                IsEnrolled = course.WatchingUsers.Any(watchingUser => watchingUser.Id == userId)
            },
            course => course.Id == courseId,
            cancellationToken);

        return accessContext is not null && accessContext.IsEnrolled;
    }

    public async Task<bool> HasAccessToSectionAsync(Guid sectionId, Guid userId, CancellationToken cancellationToken = default)
    {
        var accessContext = await _repositoryWrapper.SectionsRepository.GetSingleOrDefaultProjectedAsync(
            section => new CourseAccessContext
            {
                IsEnrolled = section.Course!.WatchingUsers.Any(watchingUser => watchingUser.Id == userId)
            },
            section => section.Id == sectionId,
            cancellationToken);

        return accessContext is not null && accessContext.IsEnrolled;
    }

    public async Task<bool> HasAccessToVideoAsync(Guid videoId, Guid userId, CancellationToken cancellationToken = default)
    {
        var accessContext = await _repositoryWrapper.VideosRepository.GetSingleOrDefaultProjectedAsync(
            video => new CourseAccessContext
            {
                IsEnrolled = video.Section!.Course!.WatchingUsers.Any(watchingUser => watchingUser.Id == userId)
            },
            video => video.Id == videoId,
            cancellationToken);

        return accessContext is not null && accessContext.IsEnrolled;
    }

    private sealed class CourseAccessContext
    {
        public bool IsEnrolled { get; set; }
    }
}
