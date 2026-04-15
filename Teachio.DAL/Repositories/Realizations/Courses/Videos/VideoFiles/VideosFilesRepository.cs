using Teachio.DAL.Entities.Courses.Videos.VideoFiles;
using Teachio.DAL.Persistence;
using Teachio.DAL.Repositories.Interfaces.Courses.Videos.VideoFiles;
using Teachio.DAL.Repositories.Realizations.Base;

namespace Teachio.DAL.Repositories.Realizations.Courses.Videos.VideoFiles;

public class VideosFilesRepository(TeachioDbContext dbContext)
    : RepositoryBase<VideoFile>(dbContext), IVideoFilesRepository;
