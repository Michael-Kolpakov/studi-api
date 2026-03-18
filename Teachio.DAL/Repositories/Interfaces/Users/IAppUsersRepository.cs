using Teachio.DAL.Entities.Users;
using Teachio.DAL.Repositories.Interfaces.Base;

namespace Teachio.DAL.Repositories.Interfaces.Users;

/// <summary>
/// Defines the contract for <see cref="IAppUsersRepository"/>.
/// </summary>
public interface IAppUsersRepository : IRepositoryBase<AppUser>;
