using Studi.DAL.Entities.Users.PendingRegistrations;
using Studi.DAL.Persistence;
using Studi.DAL.Repositories.Interfaces.Users.PendingRegistrations;
using Studi.DAL.Repositories.Realizations.Base;

namespace Studi.DAL.Repositories.Realizations.Users.PendingRegistrations;

public class PendingRegistrationsRepository(StudiDbContext dbContext)
    : BaseRepository<PendingRegistration>(dbContext), IPendingRegistrationsRepository;
