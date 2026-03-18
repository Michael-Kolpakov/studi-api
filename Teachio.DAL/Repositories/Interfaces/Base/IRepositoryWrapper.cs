using Teachio.DAL.Repositories.Interfaces.Courses.Courses;
using Teachio.DAL.Repositories.Interfaces.Courses.Sections;
using Teachio.DAL.Repositories.Interfaces.Courses.Videos.VideoProgress;
using Teachio.DAL.Repositories.Interfaces.Courses.Videos.Videos;
using Teachio.DAL.Repositories.Interfaces.Users;

namespace Teachio.DAL.Repositories.Interfaces.Base;

/// <summary>
/// Defines the contract for <see cref="IRepositoryWrapper"/>.
/// </summary>
public interface IRepositoryWrapper
{
    public ICoursesRepository CoursesRepository { get; }

    public ISectionsRepository SectionsRepository { get; }

    public IVideosRepository VideosRepository { get; }

    public IVideoProgressRepository VideoProgressRepository { get; }

    public IAppUsersRepository AppUsersRepository { get; }

    /// <summary>
    /// Persists pending changes.
    /// </summary>
    /// <returns>The result produced by this operation.</returns>
    public int SaveChanges();

    /// <summary>
    /// Persists pending changes.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and contains the operation result.</returns>
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
