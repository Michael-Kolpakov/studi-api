using Teachio.DAL.Persistence;
using Teachio.DAL.Repositories.Interfaces.Base;
using Teachio.DAL.Repositories.Interfaces.Courses.Courses;
using Teachio.DAL.Repositories.Interfaces.Courses.Sections;
using Teachio.DAL.Repositories.Interfaces.Courses.Videos.VideoProgress;
using Teachio.DAL.Repositories.Interfaces.Courses.Videos.Videos;
using Teachio.DAL.Repositories.Interfaces.Users;
using Teachio.DAL.Repositories.Realizations.Courses.Courses;
using Teachio.DAL.Repositories.Realizations.Courses.Sections;
using Teachio.DAL.Repositories.Realizations.Courses.Videos.VideoProgress;
using Teachio.DAL.Repositories.Realizations.Courses.Videos.Videos;
using Teachio.DAL.Repositories.Realizations.Users;

namespace Teachio.DAL.Repositories.Realizations.Base;

public class RepositoryWrapper : IRepositoryWrapper
{
    private readonly TeachioDbContext _dbContext;

    private ICoursesRepository? _coursesRepository;

    private ISectionsRepository? _sectionsRepository;

    private IVideosRepository? _videosRepository;

    private IVideoProgressRepository? _videoProgressRepository;

    private IAppUsersRepository? _appUsersRepository;

    public RepositoryWrapper(TeachioDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public ICoursesRepository CoursesRepository
        => _coursesRepository ??= new CoursesRepository(_dbContext);

    public ISectionsRepository SectionsRepository
        => _sectionsRepository ??= new SectionsRepository(_dbContext);

    public IVideosRepository VideosRepository
        => _videosRepository ??= new VideosRepository(_dbContext);

    public IVideoProgressRepository VideoProgressRepository
        => _videoProgressRepository ??= new VideoProgressRepository(_dbContext);

    public IAppUsersRepository AppUsersRepository
        => _appUsersRepository ??= new AppUsersRepository(_dbContext);

    public int SaveChanges()
    {
        return _dbContext.SaveChanges();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
