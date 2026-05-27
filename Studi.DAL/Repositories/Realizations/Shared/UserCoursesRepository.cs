using Studi.DAL.Entities.Shared;
using Studi.DAL.Persistence;
using Studi.DAL.Repositories.Interfaces.Shared;
using Studi.DAL.Repositories.Realizations.Base;

namespace Studi.DAL.Repositories.Realizations.Shared;

public class UserCoursesRepository(StudiDbContext dbContext)
    : BaseRepository<UserCourse>(dbContext), IUserCoursesRepository;
