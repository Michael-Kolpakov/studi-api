using Teachio.DAL.Repositories.Interfaces.Courses.Courses;
using Teachio.DAL.Repositories.Interfaces.Courses.Sections;
using Teachio.DAL.Repositories.Interfaces.Courses.Videos.VideoProgress;
using Teachio.DAL.Repositories.Interfaces.Courses.Videos.Videos;
using Teachio.DAL.Repositories.Interfaces.Users;

namespace Teachio.DAL.Repositories.Interfaces.Base;

public interface IRepositoryWrapper
{
    ICoursesRepository CoursesRepository { get; }

    ISectionsRepository SectionsRepository { get; }

    IVideosRepository VideosRepository { get; }

    IVideoProgressRepository VideoProgressRepository { get; }

    IAppUsersRepository AppUsersRepository { get; }

    int SaveChanges();

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
