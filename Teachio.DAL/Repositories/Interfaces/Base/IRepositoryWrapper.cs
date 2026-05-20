using Teachio.DAL.Repositories.Interfaces.Courses.Courses;
using Teachio.DAL.Repositories.Interfaces.Courses.Sections;
using Teachio.DAL.Repositories.Interfaces.Courses.ThumbnailFiles;
using Teachio.DAL.Repositories.Interfaces.Courses.Videos.VideoFiles;
using Teachio.DAL.Repositories.Interfaces.Courses.Videos.VideoProgress;
using Teachio.DAL.Repositories.Interfaces.Courses.Videos.Videos;
using Teachio.DAL.Repositories.Interfaces.Shared;
using Teachio.DAL.Repositories.Interfaces.Users.AvatarFiles;
using Teachio.DAL.Repositories.Interfaces.Users.PendingRegistrations;
using Teachio.DAL.Repositories.Interfaces.Users.Users;

namespace Teachio.DAL.Repositories.Interfaces.Base;

public interface IRepositoryWrapper
{
    ICoursesRepository CoursesRepository { get; }

    IThumbnailFilesRepository ThumbnailFilesRepository { get; }

    ISectionsRepository SectionsRepository { get; }

    IVideosRepository VideosRepository { get; }

    IVideoFilesRepository VideoFilesRepository { get; }

    IVideoProgressRepository VideoProgressRepository { get; }

    IAppUsersRepository AppUsersRepository { get; }

    IUserCoursesRepository UserCoursesRepository { get; }

    IAvatarFilesRepository AvatarFilesRepository { get; }

    IPendingRegistrationsRepository PendingRegistrationsRepository { get; }

    int SaveChanges();

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
