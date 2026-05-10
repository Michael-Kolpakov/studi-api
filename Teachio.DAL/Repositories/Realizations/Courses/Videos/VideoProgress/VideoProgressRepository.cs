using Teachio.DAL.Persistence;
using Teachio.DAL.Repositories.Interfaces.Courses.Videos.VideoProgress;
using Teachio.DAL.Repositories.Realizations.Base;

namespace Teachio.DAL.Repositories.Realizations.Courses.Videos.VideoProgress;

public class VideoProgressRepository(TeachioDbContext dbContext)
    : BaseRepository<Entities.Courses.Videos.VideoProgress.VideoProgress>(dbContext), IVideoProgressRepository;
