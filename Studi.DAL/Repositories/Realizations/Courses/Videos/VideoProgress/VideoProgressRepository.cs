using Studi.DAL.Persistence;
using Studi.DAL.Repositories.Interfaces.Courses.Videos.VideoProgress;
using Studi.DAL.Repositories.Realizations.Base;

namespace Studi.DAL.Repositories.Realizations.Courses.Videos.VideoProgress;

public class VideoProgressRepository(StudiDbContext dbContext)
    : BaseRepository<Entities.Courses.Videos.VideoProgress.VideoProgress>(dbContext), IVideoProgressRepository;
