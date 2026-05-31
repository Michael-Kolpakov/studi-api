using Studi.DAL.Repositories.Interfaces.Courses.Courses;
using Studi.DAL.Repositories.Interfaces.Courses.Sections;
using Studi.DAL.Repositories.Interfaces.Courses.ThumbnailFiles;
using Studi.DAL.Repositories.Interfaces.Courses.Videos.VideoFiles;
using Studi.DAL.Repositories.Interfaces.Courses.Videos.VideoProgress;
using Studi.DAL.Repositories.Interfaces.Courses.Videos.Videos;
using Studi.DAL.Repositories.Interfaces.Shared;
using Studi.DAL.Repositories.Interfaces.Users.AvatarFiles;
using Studi.DAL.Repositories.Interfaces.Users.PendingRegistrations;
using Studi.DAL.Repositories.Interfaces.Users.Users;

namespace Studi.DAL.Repositories.Interfaces.Base;

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
