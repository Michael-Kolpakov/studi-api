using Studi.DAL.Entities.Courses.ThumbnailFiles;
using Studi.DAL.Persistence;
using Studi.DAL.Repositories.Interfaces.Courses.ThumbnailFiles;
using Studi.DAL.Repositories.Realizations.Base;

namespace Studi.DAL.Repositories.Realizations.Courses.ThumbnailFiles;

public class ThumbnailFilesRepository(StudiDbContext dbContext)
    : BaseRepository<ThumbnailFile>(dbContext), IThumbnailFilesRepository;
