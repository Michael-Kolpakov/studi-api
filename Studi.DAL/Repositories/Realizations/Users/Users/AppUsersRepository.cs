using Studi.DAL.Entities.Users.Users;
using Studi.DAL.Persistence;
using Studi.DAL.Repositories.Interfaces.Users.Users;
using Studi.DAL.Repositories.Realizations.Base;

namespace Studi.DAL.Repositories.Realizations.Users.Users;

public class AppUsersRepository(StudiDbContext dbContext)
    : BaseRepository<AppUser>(dbContext), IAppUsersRepository;
