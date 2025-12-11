using Teachio.DAL.Entities.Courses.Videos.Videos;
using Teachio.DAL.Persistence;
using Teachio.DAL.Repositories.Interfaces.Courses.Videos.Videos;
using Teachio.DAL.Repositories.Realizations.Base;

namespace Teachio.DAL.Repositories.Realizations.Courses.Videos.Videos;

public class VideosRepository(TeachioDbContext dbContext)
    : RepositoryBase<Video>(dbContext), IVideosRepository;
