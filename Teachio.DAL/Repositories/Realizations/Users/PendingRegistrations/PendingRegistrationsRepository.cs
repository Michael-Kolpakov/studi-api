using Teachio.DAL.Entities.Users.PendingRegistrations;
using Teachio.DAL.Persistence;
using Teachio.DAL.Repositories.Interfaces.Users.PendingRegistrations;
using Teachio.DAL.Repositories.Realizations.Base;

namespace Teachio.DAL.Repositories.Realizations.Users.PendingRegistrations;

public class PendingRegistrationsRepository(TeachioDbContext dbContext)
    : BaseRepository<PendingRegistration>(dbContext), IPendingRegistrationsRepository;
