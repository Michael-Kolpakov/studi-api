using Teachio.DAL.Entities.Users;
using Teachio.DAL.Persistence;
using Teachio.DAL.Repositories.Interfaces.Users;
using Teachio.DAL.Repositories.Realizations.Base;

namespace Teachio.DAL.Repositories.Realizations.Users;

/// <summary>
/// Represents the <see cref="AppUsersRepository"/> type.
/// </summary>
public class AppUsersRepository(TeachioDbContext dbContext)
    : RepositoryBase<AppUser>(dbContext), IAppUsersRepository;
