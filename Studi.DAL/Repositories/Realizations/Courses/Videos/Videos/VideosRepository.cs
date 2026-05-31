using Studi.DAL.Entities.Courses.Videos.Videos;
using Studi.DAL.Persistence;
using Studi.DAL.Repositories.Interfaces.Courses.Videos.Videos;
using Studi.DAL.Repositories.Realizations.Base;

namespace Studi.DAL.Repositories.Realizations.Courses.Videos.Videos;

public class VideosRepository(StudiDbContext dbContext)
    : BaseRepository<Video>(dbContext), IVideosRepository;
