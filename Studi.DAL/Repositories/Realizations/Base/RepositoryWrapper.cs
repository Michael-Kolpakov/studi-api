using Studi.DAL.Persistence;
using Studi.DAL.Repositories.Interfaces.Base;
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
using Studi.DAL.Repositories.Realizations.Courses.Courses;
using Studi.DAL.Repositories.Realizations.Courses.Sections;
using Studi.DAL.Repositories.Realizations.Courses.ThumbnailFiles;
using Studi.DAL.Repositories.Realizations.Courses.Videos.VideoFiles;
using Studi.DAL.Repositories.Realizations.Courses.Videos.VideoProgress;
using Studi.DAL.Repositories.Realizations.Courses.Videos.Videos;
using Studi.DAL.Repositories.Realizations.Shared;
using Studi.DAL.Repositories.Realizations.Users.AvatarFiles;
using Studi.DAL.Repositories.Realizations.Users.PendingRegistrations;
using Studi.DAL.Repositories.Realizations.Users.Users;

namespace Studi.DAL.Repositories.Realizations.Base;

public class RepositoryWrapper : IRepositoryWrapper
{
    private readonly StudiDbContext _dbContext;

    private ICoursesRepository? _coursesRepository;

    private IThumbnailFilesRepository? _thumbnailFilesRepository;

    private ISectionsRepository? _sectionsRepository;

    private IVideosRepository? _videosRepository;

    private IVideoFilesRepository? _videoFilesRepository;

    private IVideoProgressRepository? _videoProgressRepository;

    private IAppUsersRepository? _appUsersRepository;

    private IUserCoursesRepository? _userCoursesRepository;

    private IAvatarFilesRepository? _avatarFilesRepository;

    private IPendingRegistrationsRepository? _pendingRegistrationsRepository;

    public RepositoryWrapper(StudiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public ICoursesRepository CoursesRepository
        => _coursesRepository ??= new CoursesRepository(_dbContext);

    public IThumbnailFilesRepository ThumbnailFilesRepository
        => _thumbnailFilesRepository ??= new ThumbnailFilesRepository(_dbContext);

    public ISectionsRepository SectionsRepository
        => _sectionsRepository ??= new SectionsRepository(_dbContext);

    public IVideosRepository VideosRepository
        => _videosRepository ??= new VideosRepository(_dbContext);

    public IVideoFilesRepository VideoFilesRepository
        => _videoFilesRepository ??= new VideoFilesRepository(_dbContext);

    public IVideoProgressRepository VideoProgressRepository
        => _videoProgressRepository ??= new VideoProgressRepository(_dbContext);

    public IAppUsersRepository AppUsersRepository
        => _appUsersRepository ??= new AppUsersRepository(_dbContext);

    public IUserCoursesRepository UserCoursesRepository
        => _userCoursesRepository ??= new UserCoursesRepository(_dbContext);

    public IAvatarFilesRepository AvatarFilesRepository
        => _avatarFilesRepository ??= new AvatarFilesRepository(_dbContext);

    public IPendingRegistrationsRepository PendingRegistrationsRepository
        => _pendingRegistrationsRepository ??= new PendingRegistrationsRepository(_dbContext);

    public int SaveChanges()
    {
        return _dbContext.SaveChanges();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
