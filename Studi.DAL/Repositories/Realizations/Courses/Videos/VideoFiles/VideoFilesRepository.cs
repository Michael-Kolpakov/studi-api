using Studi.DAL.Entities.Courses.Videos.VideoFiles;
using Studi.DAL.Persistence;
using Studi.DAL.Repositories.Interfaces.Courses.Videos.VideoFiles;
using Studi.DAL.Repositories.Realizations.Base;

namespace Studi.DAL.Repositories.Realizations.Courses.Videos.VideoFiles;

public class VideoFilesRepository(StudiDbContext dbContext)
    : BaseRepository<VideoFile>(dbContext), IVideoFilesRepository;
