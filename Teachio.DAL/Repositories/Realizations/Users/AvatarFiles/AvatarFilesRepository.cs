using Teachio.DAL.Entities.Users.AvatarFiles;
using Teachio.DAL.Persistence;
using Teachio.DAL.Repositories.Interfaces.Users.AvatarFiles;
using Teachio.DAL.Repositories.Realizations.Base;

namespace Teachio.DAL.Repositories.Realizations.Users.AvatarFiles;

public class AvatarFilesRepository(TeachioDbContext dbContext)
    : BaseRepository<AvatarFile>(dbContext), IAvatarFilesRepository;
