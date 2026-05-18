using Teachio.DAL.Entities.Users.Users;
using Teachio.DAL.Persistence;
using Teachio.DAL.Repositories.Interfaces.Users;
using Teachio.DAL.Repositories.Realizations.Base;

namespace Teachio.DAL.Repositories.Realizations.Users;

public class AppUsersRepository(TeachioDbContext dbContext)
    : BaseRepository<AppUser>(dbContext), IAppUsersRepository;
