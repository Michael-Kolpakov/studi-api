using Studi.DAL.Entities.Users.AvatarFiles;
using Studi.DAL.Persistence;
using Studi.DAL.Repositories.Interfaces.Users.AvatarFiles;
using Studi.DAL.Repositories.Realizations.Base;

namespace Studi.DAL.Repositories.Realizations.Users.AvatarFiles;

public class AvatarFilesRepository(StudiDbContext dbContext)
    : BaseRepository<AvatarFile>(dbContext), IAvatarFilesRepository;
