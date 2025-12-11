using Teachio.DAL.Repositories.Interfaces.Courses.Courses;
using Teachio.DAL.Repositories.Interfaces.Courses.Sections;
using Teachio.DAL.Repositories.Interfaces.Courses.Videos.VideoProgress;
using Teachio.DAL.Repositories.Interfaces.Courses.Videos.Videos;
using Teachio.DAL.Repositories.Interfaces.Users;

namespace Teachio.DAL.Repositories.Interfaces.Base;

public interface IRepositoryWrapper
{
    public ICoursesRepository CoursesRepository { get; }

    public ISectionsRepository SectionsRepository { get; }

    public IVideosRepository VideosRepository { get; }

    public IVideoProgressRepository VideoProgressRepository { get; }

    public IAppUsersRepository AppUsersRepository { get; }

    public int SaveChanges();

    public Task<int> SaveChangesAsync();
}
