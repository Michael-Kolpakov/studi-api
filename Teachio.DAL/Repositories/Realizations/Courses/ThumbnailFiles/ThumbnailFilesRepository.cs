using Teachio.DAL.Entities.Courses.ThumbnailFiles;
using Teachio.DAL.Persistence;
using Teachio.DAL.Repositories.Interfaces.Courses.ThumbnailFiles;
using Teachio.DAL.Repositories.Realizations.Base;

namespace Teachio.DAL.Repositories.Realizations.Courses.ThumbnailFiles;

public class ThumbnailFilesRepository(TeachioDbContext dbContext)
    : BaseRepository<ThumbnailFile>(dbContext), IThumbnailFilesRepository;
