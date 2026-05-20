using Teachio.DAL.Entities.Shared;
using Teachio.DAL.Persistence;
using Teachio.DAL.Repositories.Interfaces.Shared;
using Teachio.DAL.Repositories.Realizations.Base;

namespace Teachio.DAL.Repositories.Realizations.Shared;

public class UserCoursesRepository(TeachioDbContext dbContext)
    : BaseRepository<UserCourse>(dbContext), IUserCoursesRepository;
